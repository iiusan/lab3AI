using lab3AI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3AI
{
    public static class ExtensionMethods
    {
        public static MacbookDataView ToMacbookDataView(this Macbook data)
        {
            return new MacbookDataView
            {
                Nume = data.nume.Value,
                Diagonala = data.diagonala.Value.ToString(),
                An = data.an.Value.ToString(),
                Fregventa = data.fregventa.Value.ToString(),
                Pret = data.pret.Value.ToString(),
                Ram = data.ram.Value.ToString(),
               // Rezolutie = data.rezolutie.Value,
                Ssd = data.ssd.Value.ToString(),
                TipProcesor = data.tipProcesor.Value
            };
        }

        public static double[] ToDoubleArray(this Macbook value)
        {
            return new double[] { value.nume.NormalizedValue, value.diagonala.NormalizedValue, value.tipProcesor.NormalizedValue,
                value.fregventa.NormalizedValue, value.ram.NormalizedValue, value.ssd.NormalizedValue, value.an.NormalizedValue, };
               // value.rezolutie.NormalizedValue};
        }

        public static double ToOneDecimalDouble(this double value)
        {
            //return Convert.ToDouble(value.ToString("F1"));
            return value;
        }

        public static double[] ToDoubleArray(this List<Neuron> value)
        {
            List<double> d = new List<double>();
            foreach(var v in value)
            {
                d.Add(v.output);
            }

            return d.ToArray();
        }

        public static MackbookCodifDataView ToMacbookCofifDataView(this Macbook data)
        {
            return new MackbookCodifDataView
            {
                Nume = data.nume.NormalizedValue,
                Diagonala = data.diagonala.NormalizedValue,
                An = data.an.NormalizedValue,
                Fregventa = data.fregventa.NormalizedValue,
                Pret = data.pret.NormalizedValue,
                Ram = data.ram.NormalizedValue,
               // Rezolutie = data.rezolutie.NormalizedValue,
                Ssd = data.ssd.NormalizedValue,
                TipProcesor = data.tipProcesor.NormalizedValue
            };
        }
    }
}
