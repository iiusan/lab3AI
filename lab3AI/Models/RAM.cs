using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3AI.Models
{
    public class RAM
    {
        private double _value;
        private double _min = 8, _max = 16;
        public double Value { get { return _value; } set { _value = value; } }
        public double NormalizedValue { get { return (_value - _min) / (_max - _min); } }
    }
}
