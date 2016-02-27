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

using ExprTree;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.IO;

namespace RootFinder
{
    /// <summary>
    /// Interaction logic for FunctionConvertXMLWindow.xaml
    /// </summary>
    public partial class FunctionConvertXMLWindow : Window
    {
        public FunctionConvertXMLWindow()
        {
            InitializeComponent();
        }

        private void ButtonConvertToXML_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] range = txtRange.Text.Trim(new char[] { ']', '[' }).Split(',');
                double from = double.Parse(range[0]);
                double to = double.Parse(range[1]);
                int pointsCount = int.Parse(txtCount.Text);

                Point[] pointsToXML = GeneratePoints(from, to, pointsCount);

                SavePointsToXML(pointsToXML);

                this.Close();
            }
            catch (Exception ex)
            {
                StringBuilder msg = new StringBuilder();
                do
                {
                    msg.AppendLine(ex.Message);
                    if(ex.InnerException != null)
                        ex = ex.InnerException;
                } while (ex.InnerException != null);
                MessageBox.Show(msg.ToString());
            }
        }

        private Point[] GeneratePoints(double from, double to, int pointsCount)
        {
            ExprTree.Expression expr = new ExprTree.Expression(lblFunc.Content.ToString(), "x");
            Func<double, double> f = expr.CompileDynamicMethod();

            //generate points array
            Point[] pointsToXML = new Point[pointsCount];
            for (int i = 1; i <= pointsCount; i++)
            {
                //Chebyshev points
                double Xi = 0.5 * (from + to) + 0.5 * (to - from) * Math.Cos((2.0 * i - 1) / (2.0 * pointsToXML.Length) * Math.PI);

                pointsToXML[pointsCount-i] = new Point(Xi, f(Xi));
            }
            return pointsToXML;
        }

        private void SavePointsToXML(Point[] pointsToXML)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "XML | *.xml";
            saveDlg.AddExtension = true;

            if (saveDlg.ShowDialog() == true)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Point[]));
                xmlSerializer.Serialize(new FileStream(saveDlg.FileName, FileMode.Create), pointsToXML);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblFunc.Content = ((MainWindow)this.Owner).txtFx.Text;
        }
    }
}
