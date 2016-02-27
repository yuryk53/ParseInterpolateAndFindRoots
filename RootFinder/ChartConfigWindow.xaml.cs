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

namespace RootFinder
{
    /// <summary>
    /// Interaction logic for ChartConfigWindow.xaml
    /// </summary>
    public partial class ChartConfigWindow : Window
    {
        public event EventHandler<ChartEventArgs> ChartConfigChanged;
        double xFrom, xTo, yFrom, yTo, dx;
        public ChartConfigWindow(double xFrom, double xTo, double yFrom, double yTo, double dx, bool bindToFx, bool bindToGx)
        {
            InitializeComponent();
            this.xFrom = xFrom;
            this.xTo = xTo;
            this.yFrom = yFrom;
            this.yTo = yTo;
            this.dx = dx;
            chkBindYFx.IsChecked = bindToFx;
            chkBindYGx.IsChecked = bindToGx;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtXFrom.Text = this.xFrom.ToString();
            txtXTo.Text = this.xTo.ToString();
            txtYFrom.Text = this.yFrom.ToString();
            txtYTo.Text = this.yTo.ToString();
            sldrAcc.Value = -(int)Math.Log10(dx);

        }

        private void ButtonOK_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.xFrom = double.Parse(txtXFrom.Text);
                this.yFrom = double.Parse(txtYFrom.Text);
                this.xTo = double.Parse(txtXTo.Text);
                this.yTo = double.Parse(txtYTo.Text);
                this.dx = 1.0 / Math.Pow(10, sldrAcc.Value);

                if(ChartConfigChanged!=null)
                    ChartConfigChanged(this, new ChartEventArgs(this.xFrom, this.xTo, this.yFrom, this.yTo, this.dx, (bool)chkBindYFx.IsChecked, (bool)chkBindYGx.IsChecked));
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonCancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chkBindYFx_Checked(object sender, RoutedEventArgs e)
        {
            txtYFrom.IsEnabled = txtYTo.IsEnabled = !(((bool)chkBindYFx.IsChecked) || ((bool)chkBindYGx.IsChecked));
        }

        private void chkBindYGx_Checked(object sender, RoutedEventArgs e)
        {
            txtYFrom.IsEnabled = txtYTo.IsEnabled = !(((bool)chkBindYFx.IsChecked) || ((bool)chkBindYGx.IsChecked));
        }
    }
}
