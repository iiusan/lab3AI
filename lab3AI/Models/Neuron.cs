using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3AI.Models
{
    public class Neuron
    {
        public int INPUT = 7;

        public double[] _inputs = new double[99999];
        public double[] _weights = new double[99999];
        public double _error;

        private double _biasWeight;

        private Random r = new Random();

        public Neuron(int inputMax)
        {
            INPUT = inputMax;
        }

        public double output
        {
            get {
                double sum = 0;
                for (int i = 0; i < INPUT; ++i)
                    sum += _weights[i] * _inputs[i];
                return Utils.Output(sum + _biasWeight);
            }
        }

        public void RandomizeWeights()
        {
            for(int i = 0; i < INPUT; ++i)
                _weights[i] = r.NextDouble();
            _biasWeight = r.NextDouble();
        }

        public void adjustWeights()
        {
            for (int i = 0; i < INPUT; ++i)
                _weights[i] += _error * _inputs[i];
            _biasWeight += _error;
        }
    }

}
