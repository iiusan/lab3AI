using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3AI.Models
{
    public class Nume
    {
        private string _value;
        public string Value { get { return _value; } set { _value = value; } }
        public double NormalizedValue { get { return _value == "macbook" ? 0 : (_value == "air" ? 0.5 : 1); } }
    }
}
