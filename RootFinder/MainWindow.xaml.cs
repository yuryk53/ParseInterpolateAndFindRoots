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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;    //create new xml
using Microsoft.Win32; //dialog boxes (Open, Save)
using System.Data;  //DataTable
using Microsoft.VisualBasic; //input box
using System.Xml.Serialization; //create new xml

using Polynom;  //lagrange polynom for interpolation
using ExprTree; //expression tree generation


//generating PDF report
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing.Layout;
using System.Diagnostics;

namespace RootFinder
{
    struct SearchInterval
    {
        public double From;
        public double To;
        public SearchInterval(string interval) //in format [a,b]
        {
            interval = interval.Replace("[", string.Empty).Replace("]", string.Empty);
            string[] borders = interval.Split(',');

            try
            {
                From = double.Parse(borders[0]);
                To = double.Parse(borders[1]);
            }
            catch (IndexOutOfRangeException)
            {
                throw new FormatException("Interval format is invalid!");
            }
        }
        public override string ToString()
        {
            return string.Format("[{0},{1}]",From,To);
        }
    }

    struct ChartData
    {
        public double XFrom { get; set; }
        public double XTo { get; set; }
        public double? YFrom { get; set; }
        public double? YTo { get; set; }
        public double Dx { get; set; }
        public ChartData(double xFrom, double xTo, double yFrom, double yTo, double dx) : this()
        {
            XFrom = xFrom;
            XTo = xTo;
            YFrom = yFrom;
            YTo = yTo;
            Dx = dx;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Serializable]
    public partial class MainWindow : Window
    {
        ExprTree.Expression _fx=null;
        ExprTree.Expression _gx=null;
        ExprTree.Expression Fx
        {
            get
            {
                if (_fx != null)
                {
                    return _fx;   
                }
                else
                {
                    try
                    {
                        _fx = new ExprTree.Expression(txtFx.Text, "x");
                    }
                    catch (ArgumentException)
                    {
                        _fx = null;//throw new ArgumentException("Expression F(x) is empty!");
                    }
                    return _fx;
                }
            }
            set 
            {
                _fx = value;
                menShowChart.IsEnabled = this.IsDataLoaded;
                if(BindYToFx)
                    this.chartData.YFrom = this.chartData.YTo = null; //new function => new range
            }
        }
        ExprTree.Expression Gx
        {
            get
            {
                if (_gx != null)
                {
                    return _gx; 
                }
                else
                {
                    if (Lp != null)
                        _gx = new ExprTree.Expression(Lp.ToString(), "x");
                    else throw new NullReferenceException("Interpolation points were not loaded!");
                    menShowChart.IsEnabled = this.IsDataLoaded;
                    return _gx;
                }
            }
            set 
            { 
                _gx = value;
                menShowChart.IsEnabled = this.IsDataLoaded;
                if(BindYToGx)
                    this.chartData.YFrom = this.chartData.YTo = null; //new function => new range
            }
        }

        bool IsDataLoaded
        {
            get
            {

                try
                {
                    return (Fx.ExpressionString != null) && (Gx.ExpressionString != null);
                }
                catch (NullReferenceException)
                {
                    return false;
                }
            }
        }
        bool BindYToFx { get; set; }
        bool BindYToGx { get; set; }
        event EventHandler RootsCalculated = null;
        event EventHandler StartRootsCalc = null;

        LagrangePolynom Lp = null;
        List<double> roots = null;


        DataTable pointsDt = new DataTable("Interpolation points");
        
        Nullable<SearchInterval> rootFindInterval = null;
        double eps = 1e-5; //root finding epsilon (accuracy)

        event EventHandler<string> FileNameChanged;
        string _fileName = string.Empty;
        string FileName  //current XML file name
        {
            get { return _fileName; }
            set 
            { 
                if (_fileName != value) 
                    FileNameChanged(this, value); 
                _fileName = value; 
            } 
        }

        ChartData chartData = new ChartData();

        public MainWindow()
        {
            InitializeComponent();

            FileNameChanged += fileNameChanged;
            pointGrid.DataContextChanged += pointGridDataContextChanged;
            RootsCalculated += RootsCalculatedEvent;
            StartRootsCalc += StartRootsCalcEvent;

            DataColumn dcX = new DataColumn("X", typeof(double));
            DataColumn dcY = new DataColumn("Y", typeof(double));
            pointsDt.Columns.Add(dcX);
            pointsDt.Columns.Add(dcY);
        }

        private void StartRootsCalcEvent(object sender, EventArgs e)
        {
            btnReport.IsEnabled = false;
            btnFindRoots.IsEnabled = false;
            expSummary.IsEnabled = false;
            expSummary.IsExpanded = false;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        private void RootsCalculatedEvent(object sender, EventArgs e)
        {
            PrepareSummary();
            txtFx.BorderBrush = new SolidColorBrush(Colors.Green);

            btnReport.IsEnabled = true;
            btnFindRoots.IsEnabled = true;
            expSummary.IsEnabled = true;
            expSummary.IsExpanded = true;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void pointGridDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckRootFindInterval();
            this.chartData = new ChartData
            {
                XFrom = this.rootFindInterval.Value.From,
                XTo = this.rootFindInterval.Value.To,
                Dx = 1e-4
            };

            menEditXMLTable.IsEnabled = true;
            btnReport.IsEnabled = false;
        }

        private void fileNameChanged(object sender, string e)
        {
            btnSaveChangesToXML.IsEnabled = false;

            pointGrid.IsReadOnly = true;
            pointGrid.BorderBrush = new SolidColorBrush(Colors.Gray);
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            //TODO: some progress interruption/saving staff
            this.Close();
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The program was created by Yura Bilyk, 2015\n NTU'KhPI'");
        }

        private void ButtonLoadXML_Click(object sender, RoutedEventArgs e)
        {
            LoadTableFromXML();
        }

        private void MenuOpenXML_Click(object sender, RoutedEventArgs e)
        {
            LoadTableFromXML();
        }

        private void LoadTableFromXML()
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "XML | *.xml";

            if (openDlg.ShowDialog() == true)
            {
                try
                {
                    Lp = LagrangePolynom.ReadFromXML(openDlg.FileName);
                }
                catch (InvalidOperationException)
                { 
                    MessageBox.Show("Invalid XML file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); 
                    return; 
                }

                Gx = new ExprTree.Expression(Lp.ToString(), "x");
                this.FileName = openDlg.FileName;

                //show the values in the point grid
                pointsDt.Clear();
                foreach (Point p in Lp.GetPointCollection())
                {
                    DataRow newRow = pointsDt.NewRow();
                    newRow["X"] = p.X;
                    newRow["Y"] = p.Y;
                    pointsDt.Rows.Add(newRow);
                }
                pointGrid.DataContext = pointsDt;
                pointGrid.UpdateLayout();
            }
        }

        private void ButtonPolynomShow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show(Lp.ToString());
            }
            catch
            {
                MessageBox.Show("You have to load interpolation points table first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuSrcInterval_Click(object sender, RoutedEventArgs e)
        {

            if (Lp != null) //data was loaded
            {
                this.rootFindInterval = new SearchInterval
                {
                    From = (double)pointsDt.Rows[0]["X"],
                    To = (double)pointsDt.Rows[pointsDt.Rows.Count - 1]["X"]
                };
            }


            string interval = Interaction.InputBox(
                string.Format("Current interval is: {0}\nInput your interval in the following format: [a,b].",
                               this.rootFindInterval == null ? "UNDEFINED" : this.rootFindInterval.ToString()),
                "Interval configuration");

            try
            {
                if (interval != string.Empty)
                    this.rootFindInterval = new SearchInterval(interval);
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid interval format!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuAccuracy_Click(object sender, RoutedEventArgs e)
        {
            string input = Interaction.InputBox(
                string.Format("Current value for Epsilon is {0}\nInput your value and press OK:", this.eps), 
                "Accuracy configuration");

            try
            {
                if (input != string.Empty)
                    this.eps = double.Parse(input);
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid accuracy format!\nYou should use correct floating point number\nComma should be used as decimal separator.",
                    "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuEditXMLTable_Click(object sender, RoutedEventArgs e)
        {
            if (pointsDt.Rows.Count>0)
            {
                pointGrid.IsReadOnly = false;
                ColorAnimation borderColorAnimation = new ColorAnimation
                {
                    From = Colors.Red,
                    To = Colors.Gray,
                    Duration = new Duration(TimeSpan.FromSeconds(1))
                };

                pointGrid.BorderBrush = new SolidColorBrush();
                pointGrid.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, borderColorAnimation);
            }
        }

        private void ButtonSaveTableToXML_Click(object sender, RoutedEventArgs e)
        {
            List<Point> interpolationPoints = new List<Point>();
            DataTableReader dr = new DataTableReader(pointsDt);
            while (dr.Read())
            {
                interpolationPoints.Add(new Point((double)dr["X"], (double)dr["Y"]));
            }
            Lp = new LagrangePolynom(interpolationPoints.ToArray());
            Lp.SaveToXML(this.FileName);

            btnSaveChangesToXML.IsEnabled = false;
            pointGrid.IsReadOnly = true;
        }

        private void pointGridCellEdit_Click(object sender, DataGridCellEditEndingEventArgs e)
        {
            btnSaveChangesToXML.IsEnabled = true;
        }

        private async void ButtonFindRoots_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Lp == null) throw new NullReferenceException("Interpolation points were not loaded!");

                //Fx = new ExprTree.Expression(txtFx.Text, "x"); //it is compiled after it's input in txtFx_LostKeybFocus
                //Gx = new ExprTree.Expression(Lp.ToString(), "x");
                ExprTree.Expression Zx = Fx - Gx; //solve for f(x)=(gx) => f(x)-g(x)=0

                CheckRootFindInterval();

                StartRootsCalc(this, EventArgs.Empty);
                this.roots = await Zx.FindRootsBruteAsync(this.rootFindInterval.Value.From, this.rootFindInterval.Value.To, 1e-5, this.eps);
                
                if (this.RootsCalculated != null)
                    RootsCalculated(this, EventArgs.Empty);

            }
            catch (NullReferenceException ex)
            {
                if (ex.Message == "Interpolation points were not loaded!")
                {
                    pointGrid.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else throw;
            }
            catch (Exception ex)
            {
                txtFx.BorderBrush = new SolidColorBrush(Colors.Red);
                MessageBox.Show(ex.Message);
            }
        }

        private void CheckRootFindInterval()
        {
            if (this.rootFindInterval == null)
            {

                if (MessageBox.Show(
                    string.Format("The search interval is UNDEFINED. It will be set to {0} by default!",
                    new SearchInterval
                    {
                        From = (double)pointsDt.Rows[0]["X"],
                        To = (double)pointsDt.Rows[pointsDt.Rows.Count - 1]["X"]
                    }),
                    "Interval UNDEFINED",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                {
                    this.rootFindInterval = new SearchInterval
                    {
                        From = (double)pointsDt.Rows[0]["X"],
                        To = (double)pointsDt.Rows[pointsDt.Rows.Count - 1]["X"]
                    };
                }
            }
            else
            {
                this.rootFindInterval = new SearchInterval
                {
                    From = (double)pointsDt.Rows[0]["X"],
                    To = (double)pointsDt.Rows[pointsDt.Rows.Count - 1]["X"]
                };
            }
        }

        private void PrepareSummary()
        {
            lblSrcInterval.Content = this.rootFindInterval.Value.ToString();
            lblEps.Content = this.eps;
            lblInterpError.Content = CalcInterpolationError();
            lblRoots.Content = this.roots.Count;
            lblLagPower.Content = this.Lp.Power;
        }

        private double CalcInterpolationError() //Maximum interp error among interpolation points
        {
            double error = 0;
            Point[] interpolationPoints = Lp.GetPointCollection();
            Func<double, double> gx = Gx.CompileDynamicMethod();

            for (int i = 0; i < interpolationPoints.Length; i++)
            {
                double gxValue = gx(interpolationPoints[i].X);
                if (gxValue != interpolationPoints[i].Y)
                {
                    error = Math.Max(gxValue, error);
                }
            }
            return error;
        }

        private void ButtonShowAllRoots_Click(object sender, RoutedEventArgs e)
        {
            if (roots.Count < 20)
                MessageBox.Show(string.Join("\n", this.roots));
            else MessageBox.Show("Too many root entries! Try to save a report or increase an accuracy!");
        }

        private void ButtonCreateXML_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "XML | *.xml";

            if (saveDlg.ShowDialog() == true)
            {
                this.FileName = saveDlg.FileName;

                pointsDt.Clear();
                pointGrid.DataContext = pointsDt;
                pointGrid.IsReadOnly = false;
                pointGrid.UpdateLayout();
            } 
        }

        private void MenuShowChart_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Func<double, double> fx = this.Fx.CompileDynamicMethod();
                Func<double, double> gx = this.Gx.CompileDynamicMethod();
                if (this.chartData.YTo==null || this.chartData.YFrom==null) //chart data y-values were not initialized
                {
                    Parallel.Invoke(() =>
                    {
                        Extremum extremum = DiagramWindow.FuncFindMinMax(new Func<double, double>[] { fx, gx }, this.chartData.XFrom, this.chartData.XTo);
                        this.chartData.YFrom = -Math.Abs(1.5 * Math.Max(Math.Abs(extremum.Min), Math.Abs(extremum.Max)));
                        this.chartData.YTo = -chartData.YFrom;
                    });
                }

                DiagramWindow diagWnd= new DiagramWindow(chartData.XFrom, chartData.XTo, chartData.YFrom, chartData.YTo, chartData.Dx);

                diagWnd.DrawGraph(new Func<double, double>[] { fx, gx }, this.chartData.XFrom, this.chartData.XTo);
                

                diagWnd.Owner = this;
                diagWnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                diagWnd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuConvertFuncToXML_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExprTree.Expression expr = new ExprTree.Expression(txtFx.Text, "x");
            }
            catch (Exception)
            {
                MessageBox.Show("Input function is incorrect!");
                txtFx.BorderBrush = new SolidColorBrush(Colors.Red);
                txtFx.SelectAll();
                return;
            }
            txtFx.BorderBrush = new SolidColorBrush(Colors.Green);

            //converts F(x) to xml format
            FunctionConvertXMLWindow convertWnd = new FunctionConvertXMLWindow();

            convertWnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            convertWnd.Owner = this;
            convertWnd.Show();
        }

        private void MenuChartConfig_Clicked(object sender, RoutedEventArgs e)
        {
            double xFrom = this.chartData.XFrom;
            double xTo = this.chartData.XTo;
            Func<double,double> fx = this.Fx.CompileDynamicMethod();
            Func<double,double> gx = this.Gx.CompileDynamicMethod();
            Extremum extremum = DiagramWindow.FuncFindMinMax(new Func<double,double>[]{fx,gx}, xFrom, xTo);
            double yFrom = this.chartData.YFrom ?? -Math.Abs(1.5 * Math.Max(Math.Abs(extremum.Min), Math.Abs(extremum.Max)));
            double yTo = -yFrom;

            ChartConfigWindow configWnd = new ChartConfigWindow(xFrom, xTo, yFrom, yTo, this.chartData.Dx, this.BindYToFx, this.BindYToGx);

            configWnd.ChartConfigChanged += delegate (object o, ChartEventArgs eArgs)
            {
                this.chartData.XFrom = eArgs.From.X;
                this.chartData.XTo = eArgs.To.X;
                this.chartData.YFrom = eArgs.From.Y;
                this.chartData.YTo = eArgs.To.Y;
                this.chartData.Dx = eArgs.Dx;
                this.BindYToFx = eArgs.BindToFx;
                this.BindYToGx = eArgs.BindToGx;

                if (this.BindYToFx && this.BindYToGx)
                    extremum = DiagramWindow.FuncFindMinMax(new Func<double, double>[] { fx, gx }, xFrom, xTo);
                else if (this.BindYToFx)
                    extremum = DiagramWindow.FuncFindMinMax(new Func<double, double>[] { fx }, xFrom, xTo);
                else if (this.BindYToGx)
                    extremum = DiagramWindow.FuncFindMinMax(new Func<double, double>[] { gx }, xFrom, xTo);
                
                this.chartData.YFrom = -Math.Abs(1.5 * Math.Max(Math.Abs(extremum.Min), Math.Abs(extremum.Max)));
                this.chartData.YTo = -this.chartData.YFrom;
            };

            

            configWnd.Owner = this;
            configWnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            configWnd.Show();
        }

        private void txtFx_LostKeybFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                this.Fx = new ExprTree.Expression(txtFx.Text, "x");
                btnReport.IsEnabled = false;
            }
            catch (ArgumentException)
            {
                this.Fx = null;
            }
        }

        private void MenuReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Func<double, double> fx = this.Fx.CompileDynamicMethod();
                Func<double, double> gx = this.Gx.CompileDynamicMethod();
                if (this.chartData.YTo == null || this.chartData.YFrom == null) //chart data y-values were not initialized
                {
                    Parallel.Invoke(() =>
                    {
                        Extremum extremum = DiagramWindow.FuncFindMinMax(new Func<double, double>[] { fx, gx }, this.chartData.XFrom, this.chartData.XTo);
                        this.chartData.YFrom = -Math.Abs(1.5 * Math.Max(Math.Abs(extremum.Min), Math.Abs(extremum.Max)));
                        this.chartData.YTo = -chartData.YFrom;
                    });
                }

                DiagramWindow diagWnd = new DiagramWindow(chartData.XFrom, chartData.XTo, chartData.YFrom, chartData.YTo, chartData.Dx);
                diagWnd.Owner = this;

                RenderTargetBitmap rtb = diagWnd.DrawGraphToImage(new Func<double, double>[] { fx, gx }, width:800, height:800);
                diagWnd.Close();

                PngBitmapEncoder png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(rtb));
                using (Stream stm = File.Create("diagram.png"))
                {
                    png.Save(stm);
                }

                //generate pdf--------------------------------------------------------
                PdfDocument doc = new PdfDocument();
                doc.Info.Title = "RootFinder Report";

                PdfPage page1 = doc.AddPage();

                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page1);

                // Create a font
                XFont font = new XFont("Times New Roman", 14, XFontStyle.Regular);

                //generate a report---------------------------------------------------------------------------------------------------------------
                StringBuilder report = new StringBuilder();
                string task = "Finding roots of equations using dynamic expression tree generation and function interpolation\r\n" +
                               "\tUsed algorithms: Shunting-yard algorithm and Lagrange interpolation.\r\n\r\n";
                report.Append(task);
                report.AppendFormat("Search interval: {0}\r\n", lblSrcInterval.Content);
                report.AppendFormat("Eps.: {0}\r\n", lblEps.Content);
                report.AppendFormat("Interpolation error: {0}\r\n", lblInterpError.Content);
                report.AppendFormat("Number of roots: {0}\r\n", lblRoots.Content);
                report.AppendFormat("Power of Lagrange polynomial: {0}\r\n", lblLagPower.Content);
                report.AppendFormat("Roots are shown on the page #2\r\n", lblInterpError.Content);

                //generate a report---------------------------------------------------------------------------------------------------------------


                XTextFormatter tf = new XTextFormatter(gfx);

                XRect rect = new XRect(10, 10, page1.Width - 10, 150);
                gfx.DrawRectangle(XBrushes.SeaShell, rect);
                //tf.Alignment = ParagraphAlignment.Left;
                tf.DrawString(report.ToString(), font, XBrushes.Black, rect, XStringFormats.TopLeft);

                XImage img = XImage.FromFile("diagram.png");
                
                gfx.DrawImage(img, (page1.Width-img.PixelWidth/1.5)/2.0, 250, img.PixelWidth/1.5, img.PixelHeight/1.5);

                //page#2 - roots
                const int stringsPerPage = 50;
                int k=0;
                for (; k < this.roots.Count; k += stringsPerPage)
                {
                    StringBuilder rootsStr = new StringBuilder();
                    if(k==0)
                        rootsStr.AppendLine("Roots found:\r\n");

                    for (int i = 0; i < stringsPerPage && i+k<this.roots.Count; i++)
                    {
                        rootsStr.AppendLine(string.Format("{0}", this.roots[i+k]));
                    }
                    PdfPage page2 = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page2);
                    gfx.DrawRectangle(XBrushes.White, rect);
                    tf = new XTextFormatter(gfx);
                    rect = new XRect(10, 10, page2.Width - 10, page2.Height - 10);
                    tf.DrawString(rootsStr.ToString(), font, XBrushes.Black, rect, XStringFormats.TopLeft);
                }

                using (FileStream fStream = new FileStream("report.pdf", FileMode.Create))
                {
                    doc.Save(fStream);
                }
                Process.Start("report.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("HELP.chm");
            }
            catch
            {
                MessageBox.Show("Help file read error!");
            }
        }

        private void btnSaveFExpr(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "XML | *.xml";
            saveDlg.AddExtension = true;

            if (saveDlg.ShowDialog() == true)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(string));
                xmlSerializer.Serialize(new FileStream(saveDlg.FileName, FileMode.Create), txtFx.Text);
            }
        }

        private void btnOpenFExpr(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "XML | *.xml";

            if (openDlg.ShowDialog() == true)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(string));
                try
                {
                    txtFx.Text = (string)xmlSerializer.Deserialize(new FileStream(openDlg.FileName, FileMode.Open));
                    txtFx.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
