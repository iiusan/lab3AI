using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3AI.Models
{
    public class Fregventa
    {
        private double _value;
        private double _min = 1.2, _max = 2.9;
        public double Value { get { return _value; } set { _value = value; } }
        public double NormalizedValue { get { return ((_value - _min) / (_max - _min)).ToOneDecimalDouble(); } }
    }
}
