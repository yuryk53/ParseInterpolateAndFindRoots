using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace RootFinder
{
    public class ChartEventArgs : EventArgs
    {
        public Point From { get; set; }
        public Point To { get; set; }
        public double Dx { get; set; }
        public bool BindToFx { get; set; }
        public bool BindToGx { get; set; }

        public ChartEventArgs(double xFrom, double xTo, double yFrom, double yTo, double dx, bool bindToFx, bool bindToGx)
        {
            this.From = new Point(xFrom, yFrom);
            this.To = new Point(xTo, yTo);
            this.Dx = dx;
            this.BindToFx = bindToFx;
            this.BindToGx = bindToGx;
        }
    }
}
