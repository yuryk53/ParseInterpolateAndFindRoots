using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExprTree;
using Polynom;

namespace ParseInterpolateAndFindRoots
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("x^2-4:\n");
            Expression x2 = new Expression("x^2", "x");
            Expression four = new Expression("4");
            PrintRoots(x2-four);

            //x^2-x-2 => x1=-1; x2=2
            Console.WriteLine("x^2-x-2:\n");
            Expression x = new Expression("x", "x");
            Expression two = new Expression("2");
            PrintRoots(x2 - x - two);

            
            //Using L'argrange's polynom
            Console.WriteLine("x^2:\n");
            Expression lagrange = new Expression(LagrangePolynom.ReadFromXML("testPol_1.xml").ToString(), "x");
            PrintRoots(lagrange); //x^2 => root: 0

            Console.WriteLine("Sin:\n");
            Expression sin = new Expression(LagrangePolynom.ReadFromXML("sin.xml").ToString(), "x");
            PrintRoots(sin);

            Console.WriteLine("Chebyshev tan:\n");
            Expression ch_Tan = new Expression(LagrangePolynom.ReadFromXML("chebyshevTan.xml").ToString(), "x");
            PrintRoots(ch_Tan);

            Console.WriteLine("Tan:\n");
            Expression tan = new Expression(LagrangePolynom.ReadFromXML("tan.xml").ToString(), "x");
            PrintRoots(tan);

            Console.WriteLine("x^8:\n");
            Expression x8 = new Expression(LagrangePolynom.ReadFromXML("x8.xml").ToString(), "x");
            PrintRoots(x8);

        }

        private static void PrintRoots(Expression e)
        {
            List<double> roots = e.FindRootsBrute(-10, 10);  //+2 -2
            Console.WriteLine("Roots: \n{0}\n\n", string.Join("\n", roots));
        }


    }
}
