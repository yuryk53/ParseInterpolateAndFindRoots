/*Copyright Yura Bilyk, 2015*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootFinder
{
    public class DrawArgs
    {
        public double From { get; set; }
        public double To { get; set; }
        public int UIWidth { get; set; }
        public int UIHeight { get; set; }
        public Func<double, double>[] F { get; set; }
        public VisualTargetPresentationSource visualTargetPS = null;
    }
}
