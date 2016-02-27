using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Threading;
using System.Windows.Threading;

using System.Windows.Media.Imaging; 

namespace RootFinder
{
    public struct Extremum
    {
        public double Max { get; set; }
        public double Min { get; set; }
    }

    /// <summary>
    /// Interaction logic for Diagram.xaml
    /// </summary>
    public partial class DiagramWindow : Window
    {
        public ManualResetEvent manualResetEvent = new ManualResetEvent(false); //pausing
        private static AutoResetEvent s_event = new AutoResetEvent(false);
        CancellationTokenSource cancelToken = new CancellationTokenSource();
        double dx = 0.0001; //will be changed during graph plot

        //session variables
        double xFrom, xTo;
        Nullable<double> yFrom=null, yTo=null;
        Func<double, double>[] F;
        Color[] lineColors = new Color[] { Colors.Red, Colors.Green };

        public DiagramWindow(double xFrom, double xTo, double? yFrom=null, double? yTo=null, double dx=1e-4)
        {
            InitializeComponent();
            this.xFrom = xFrom;
            this.xTo = xTo;
            this.yFrom = yFrom;
            this.yTo = yTo;
            this.dx = dx;
            lblMousePos.Content = "x: 0; y: 0";
        }

        public static Extremum FuncFindMinMax(Func<double, double>[] fArr, double from, double to, double dx = 0.01) //returns positive value
        {
            double min;
            double max;
            try
            {
                min = max = fArr[0](from);
            }
            catch (DivideByZeroException)
            {
                min = double.MaxValue;
                max = double.MinValue;
            }

            foreach (Func<double, double> f in fArr)
            {
                for (double x = from + dx; x <= to; x += dx)
                {
                    double fx;
                    try
                    {
                        fx = f(x);
                    }
                    catch (DivideByZeroException)
                    {
                        continue;
                    }
                    if (min > fx)
                        min = fx;
                    if (max < fx)
                        max = fx;
                }
            }

            return new Extremum { Max = max, Min = min };
        }
        

        private void DrawGraph(DrawArgs args, Action EndCallback = null)
        {
            double from = args.From, to = args.To;

            //while (!Dispatcher.Invoke<bool>(() => drawCanvas.IsLoaded))
            //{
            //}

            int width = args.UIWidth;//(int)drawCanvas.ActualWidth;
            int height = args.UIHeight;//(int)drawCanvas.ActualHeight;


            DrawingVisual drawingVisual = new DrawingVisual();
            

            using (DrawingContext c = drawingVisual.RenderOpen())
            {
                //Brushes
                SolidColorBrush textBrush = new SolidColorBrush(Colors.Black);
                Pen axisPen = new Pen(Brushes.Black, 1);
                //Pen graphPen = new Pen(Brushes.Red, 1);
                const int FontSize = 12;


                double xFrom, xTo;
                double yFrom, yTo;

               // Func<double, double> f = args.F;

                this.xFrom = xFrom = from;
                this.xTo = xTo = to;
                Extremum extremum = FuncFindMinMax(args.F, xFrom, xTo);
                if (this.yFrom == null || this.yTo == null)
                {
                    this.yFrom = yFrom = -Math.Abs(1.5 * Math.Max(Math.Abs(extremum.Min), Math.Abs(extremum.Max))); //чтобы хотя бы часть графика попала в область отображения
                    this.yTo = yTo = -yFrom;
                }
                else
                {
                    yFrom = this.yFrom.Value;
                    yTo = this.yTo.Value;
                }

                //інтервал повинен бути симетричним (потрібно для рисування осей координат)
                if (-xFrom != xTo)
                {
                    double radius = Math.Max(Math.Abs(xFrom), Math.Abs(xTo));
                    xFrom = -radius;
                    xTo = radius;
                }


                //Scales
                double xScale = width / (xTo - xFrom);
                double yScale = height / (yTo - yFrom);

                //Origin
                int x0 = width / 2,
                    y0 = height / 2;

                //draw horizontal axis
                c.DrawLine(axisPen, new Point(0, y0), new Point(width, y0));
                //draw verical axis
                c.DrawLine(axisPen, new Point(x0, 0), new Point(x0, height));

                //draw horizontal divisions
                int N = 10;                    //number of divisions

                double valueX = (xTo - xFrom) / N, //ціна поділки
                    divHeight = 2;             //висота поділки
                int stepX = width / N;          //крок по вісі Х (від поділки до поділки)

                double divCounter = xFrom;


                for (int x = x0 % stepX; x < width; x += stepX, divCounter += valueX)
                {
                    c.DrawLine(axisPen, new Point(x, y0 - divHeight), new Point(x, y0 + divHeight));

                    FormattedText formattedText = new FormattedText(Math.Round(divCounter, 4) + "",
                                                 new System.Globalization.CultureInfo("en-us"),
                                                 FlowDirection.LeftToRight,
                                                 new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                                                 FontSize,
                                                 textBrush);
                    c.DrawText(formattedText, new Point(x - FontSize, y0 + divHeight)); //!!!!!!!!FontSize
                }

                //draw vertical divisions
                double valueY = (yTo - yFrom) / N;
                double divWidth = divHeight; //division width (height for horizontal axis)
                int stepY = height / N;

                divCounter = yTo;
                for (int y = y0 % stepY; y < height; y += stepY, divCounter -= valueY)
                {
                    if (y == y0) continue;
                    c.DrawLine(axisPen, new Point(x0 - divWidth, y), new Point(x0 + divWidth, y));

                    int divCounterLength = divCounter.ToString().Length; //кількість символів у числовому записі поділки

                    FormattedText formattedText = new FormattedText(Math.Round(divCounter, 4) + "",
                                                 new System.Globalization.CultureInfo("en-us"),
                                                 FlowDirection.LeftToRight,
                                                 new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                                                 FontSize,
                                                 textBrush);
                    c.DrawText(formattedText, new Point(x0 - ((Math.Truncate(yTo)).ToString().Length + 5) * 8 - 2 * divWidth, y - FontSize)); //!!!!!!!!FontSize //* FontSize - divWidth * 2
                }
                

                //draw desired graph
                int colorIndex = 0;
                foreach (Func<double, double> f in args.F)
                {
                    Pen graphPen = new Pen(new SolidColorBrush(this.lineColors[colorIndex]), 1); //set line color
                    colorIndex++;

                    xFrom = from;
                    xTo = to;
                    //dx = valueX / 10000.0;//CalculateDX(f);//0.00001;
                    double xPrev = xFrom, yPrev = f(xFrom);
                    float xCorrection = x0 % stepX;
                    float yCorrection = y0 % stepY;

                    Dispatcher.Invoke(() =>
                    {
                        progressBar.Value = xFrom;
                        progressBar.Minimum = xFrom;
                        progressBar.Maximum = xTo;
                    });
                    for (double x = xFrom + dx; x < xTo; x += dx)
                    {
                        double fx;
                        try
                        {
                            fx = f(x);
                        }
                        catch (DivideByZeroException)
                        { continue; }

                        if (fx > 2 * yTo || fx < 2 * yFrom) //если график ушел на бесконечность
                        {
                            while (fx > 2 * yTo || fx < 2 * yFrom) //пропустить этот участок
                            {
                                manualResetEvent.WaitOne();
                                cancelToken.Token.ThrowIfCancellationRequested();

                                if (x > xTo)
                                    break;
                                x += dx;
                                try
                                {
                                    fx = f(x);
                                }
                                catch (DivideByZeroException)
                                {
                                    continue;
                                }
                            }
                            xPrev = x;
                            yPrev = fx;
                        }

                        //if (Math.Abs(fx - yPrev) < 0.02) //avoid drawing the same point multiple times = пропустить очень-очень близкие пары значений
                        //    continue;

                        if (ArePointsEqual(PointToScreen(
                            new Point(xPrev, yPrev), extremum.Max, extremum.Min, width, height),
                            PointToScreen(new Point(x, fx), extremum.Max, extremum.Min,width,height), 1))
                                continue;
                        

                        manualResetEvent.WaitOne();
                        cancelToken.Token.ThrowIfCancellationRequested();


                        c.DrawLine(graphPen, new Point(xPrev * xScale + x0, -yPrev * yScale + y0), new Point(x * xScale + x0, -fx * yScale + y0));

                        Dispatcher.Invoke(() => progressBar.Value = x);

                        xPrev = x;
                        yPrev = fx;

                    }
                }
            }

            manualResetEvent.WaitOne();
            cancelToken.Token.ThrowIfCancellationRequested();

            if (EndCallback != null)
                EndCallback();


            args.visualTargetPS.RootVisual = drawingVisual; //updating visual tree 

        }

        private Point PointToScreen(Point p, double fMax, double fMin, double UIWidth, double UIHeight)
        {
            double xRatio = UIWidth / (xTo - xFrom); //точек функции в одной точке экрана
            double yRatio = UIHeight / (fMax - fMin);

            return new Point(p.X * xRatio, p.Y * yRatio);
        }

        private bool ArePointsEqual(Point p1, Point p2, double delta=0)
        {
            return ((int)Math.Abs(p1.X - p2.X)<=delta) && ((int)Math.Abs(p2.Y- p2.Y)<=delta);
        }

        public void DrawGraph(Func<double,double>[] f, double from, double to)
        {
            cancelToken = new CancellationTokenSource();

            manualResetEvent.Set();

            this.xFrom = from;
            this.xTo = to;
            this.F = f;
            UpdateDrawingHostAsync(f, from, to);
        }
        public void DrawGraph(Func<double, double>[] f)
        {
            DrawGraph(f, this.xFrom, this.xTo);
        }

        public RenderTargetBitmap DrawGraphToImage(Func<double, double>[] f, int width, int height)
        {
            cancelToken = new CancellationTokenSource();

            manualResetEvent.Set();

            HostVisual host = new HostVisual();

            DrawArgs drawArgs = new DrawArgs
            {
                From = this.xFrom,
                To = this.xTo,
                UIWidth = width,
                UIHeight = height,
                F = f,
                visualTargetPS = new VisualTargetPresentationSource(host),
            };
            DrawGraph(drawArgs);

            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(drawArgs.visualTargetPS.RootVisual);

            return rtb;
        }

        private void EndOfDraw()
        {
            Dispatcher.Invoke(() =>
            {
                manualResetEvent.Set();
                progressBar.Value = progressBar.Minimum;
            });
        }

        private async void UpdateDrawingHostAsync(Func<double,double>[] f, double from, double to)
        {
            HostVisual host = new HostVisual();

            Thread worker = new Thread(new ParameterizedThreadStart(MediaWorkerThread));
            worker.SetApartmentState(ApartmentState.STA);
            worker.IsBackground = true;
            worker.Start(host);


            s_event.WaitOne();          //wait for worker-thread dispatcher creation

            Dispatcher workerDispatcher = Dispatcher.FromThread(worker);

            try
            {
                await workerDispatcher.InvokeAsync(() => //поставить метод в очердь деспетчеру вторичного потока и не задерживать UI thread
                    {
                        while (!Dispatcher.Invoke<bool>(() => drawCanvas.IsLoaded));    //wait until drawCanvas.IsLoaded

                        DrawGraph(new DrawArgs
                                  {
                                      From = from,
                                      To = to,
                                      UIWidth = (int)drawCanvas.ActualWidth,
                                      UIHeight = (int)drawCanvas.ActualHeight,
                                      F = f,
                                      visualTargetPS = new VisualTargetPresentationSource(host),
                                  }, EndOfDraw);

                    });
            }
            catch (OperationCanceledException)
            {
                EndOfDraw();
                return;
            }

            drawingHost.Child = host;
        }

        private void MediaWorkerThread(object arg)
        {
            // Force the creation of the Dispatcher for this thread, and then
            Dispatcher d = Dispatcher.CurrentDispatcher;
            // signal that we are running.
            s_event.Set();
            
            Thread.CurrentThread.Name = "WorkerDrawingThread";

            // This is the central processing loop for WPF. It makes message pump possible for the worker thread.
            Dispatcher.Run();
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (drawCanvas.IsLoaded)
            {
                cancelToken.Cancel();
                lock (cancelToken)
                {
                    cancelToken = new CancellationTokenSource();
                }
                UpdateDrawingHostAsync(this.F, this.xFrom, this.xTo);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition((UIElement)sender);

            //Scales
            double xScale = drawCanvas.ActualWidth / (2*Math.Max(Math.Abs(this.xTo), Math.Abs(this.xFrom)));
            double yScale = drawCanvas.ActualHeight / (2 * Math.Max(Math.Abs(this.yTo.Value), Math.Abs(this.yFrom.Value)));
            //Origin
            int x0 = (int)drawCanvas.ActualWidth / 2,
                y0 = (int)drawCanvas.ActualHeight / 2;

            lblMousePos.Content = string.Format("x: {0}; y: {1}", Math.Round((pt.X-x0) / xScale, 2), Math.Round(-(pt.Y-y0)/yScale, 2));

        }
    }


}
