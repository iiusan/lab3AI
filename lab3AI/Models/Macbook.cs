using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3AI.Models
{
    public class Macbook
    {
        public Nume nume { get; set; }
        public TipProcesor tipProcesor { get; set; }
        public Fregventa fregventa { get; set; }
        public RAM ram { get; set; }
        public SSD ssd { get; set; }
        public An an { get; set; }
        public Rezolutie rezolutie { get; set; }
        public Pret pret = null;
        public bool testare { get; set; }
    }
}
