using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3AI.Models
{
    public static class Utils
    {
        public static double Output(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        public static double Derivative(double x)
        {
            return x * (1 - x);
        }
    }

}
