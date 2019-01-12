using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3AI.Models
{
    public class Rezolutie
    {
        private string _value;
      //  private double _min = 1299, _max = 2599;
        public string Value { get { return _value; } set { _value = value; } }
       // public double NormalizedValue { get { return (_value - _min) / (_max - _min); } }
        public double NormalizedValue { get { return 1; } }
    }
}
