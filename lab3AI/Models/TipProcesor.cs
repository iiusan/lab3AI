using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3AI.Models
{
    public class TipProcesor
    {
        private string _value;
        public string Value { get { return _value; } set { _value = value; } }
        public double NormalizedValue { get { return _value == "i3" ? 0 : (_value == "i5" ? 0.5 : 1); } }
    }
}
