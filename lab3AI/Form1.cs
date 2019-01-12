using lab3AI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab3AI
{
    public partial class Form1 : Form
    {

        private double _min=1299, _max=2599;
        private const int nLimDate = 16;
        private const int nLimAsc = 2* nLimDate;

        private int nEpoch = 20000;

        private List<MacbookDataView> _dataView = new List<MacbookDataView>();
        private List<MackbookCodifDataView> _dataCodifView = new List<MackbookCodifDataView>();
        private List<MackbookTestDataView> _dataTestView = new List<MackbookTestDataView>();
        List<int> _tapLines = new List<int>();
        private List<Macbook> _data = new List<Macbook>();
        private string _file = null;

        private List<Neuron> _hiddenLayer = new List<Neuron>();//layer h --1
        //private List<Neuron> _outputLayer = new List<Neuron>();//1 neuron pr output layer
        private Neuron _outputLayer;

        public Form1()
        {
            InitializeComponent();
            dGW_date.DataSource = _dataView;
            dGW_codif.DataSource = _dataCodifView;
            dGW_date.DataSource = _dataTestView;

            _file = @"C:\Users\Ionut\Desktop\AI.csv";
            LoadCsv();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                _file = file.FileName;
                LoadCsv();
            }
        }

        private void LoadCsv()
        {
            using (var reader = new StreamReader(_file))
            {
                _data.Clear();
                _dataView.Clear();
                _dataCodifView.Clear();
                _tapLines.Clear();

                int i = 0;
                if (!reader.EndOfStream)
                    reader.ReadLine();// header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    Macbook m = new Macbook
                    {
                        nume = new Nume { Value = values[0] },
                        tipProcesor = new TipProcesor { Value = values[1] },
                        fregventa = new Fregventa { Value = Convert.ToDouble(values[2]) },
                        ram = new RAM { Value = Convert.ToDouble(values[3]) },
                        ssd = new SSD { Value = Convert.ToDouble(values[4]) },
                        an = new An { Value = Convert.ToDouble(values[5]) },
                        rezolutie = new Rezolutie { Value = values[6] },
                        pret = new Pret { Value = Convert.ToDouble(values[7]) },
                        testare = Convert.ToBoolean(values[8])

                    };
                    _data.Add(m);
                    _dataView.Add(m.ToMacbookDataView());
                    _dataCodifView.Add(m.ToMacbookCofifDataView());
                    if (m.testare)
                        _tapLines.Add(i);
                    ++i;
                }
            }

            var binding = new BindingSource();
            binding.DataSource = _dataView;
            dGW_date.DataSource = binding;


            var bindingCod = new BindingSource();
            bindingCod.DataSource = _dataCodifView;
            dGW_codif.DataSource = bindingCod;

            //tap lines
            DataGridViewCellStyle tapped = new DataGridViewCellStyle();
            tapped.BackColor = Color.Orange;
            tapped.ForeColor = Color.White;



        }

        private void dGW_codif_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (int t in _tapLines)
            {
                dGW_codif.Rows[t].DefaultCellStyle.BackColor = Color.LightPink;
            }
        }


        private void LearnInit()
        {
            for (int i = 0; i < nLimAsc; ++i)
            {
                Neuron n = new Neuron(7);
                n.RandomizeWeights();
                _hiddenLayer.Add(n);
            }
            _outputLayer = new Neuron(nLimAsc);
            _outputLayer.RandomizeWeights();
        }

        private void Learn(bool init)
        {
            if(init)
                LearnInit();
            for (int i = 1; i < nEpoch; ++i)
                DoEpoch();
        }


        private void DoEpoch()
        {
            for (int i = 0; i < nLimDate; i++)
            {
                if (_tapLines.Contains(i))
                    continue;
                foreach (var neuron in _hiddenLayer)
                {
                    neuron._inputs = _data[i].ToDoubleArray();
                }
                _outputLayer._inputs = _hiddenLayer.ToDoubleArray();
                
                _outputLayer._error = Utils.Derivative(_outputLayer.output) * (_data[i].pret.NormalizedValue - _outputLayer.output);
                Debug.WriteLine(Math.Abs(_outputLayer._error).ToString("F9"));
                _outputLayer.adjustWeights();

                for(int w =0; w < _hiddenLayer.Count; ++w)
                {
                    _hiddenLayer[w]._error = Utils.Derivative(_hiddenLayer[w].output) * _outputLayer._error * _outputLayer._weights[w];
                }

                foreach (var neuron in _hiddenLayer)
                {
                    neuron.adjustWeights();
                }
               
            }
        }

    


        private void button2_Click(object sender, EventArgs e)
        {
            Learn(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Learn(false);
        }

        private double CalcPrice(Macbook mackbook)
        {
            foreach (var neuron in _hiddenLayer)
            {
                neuron._inputs = mackbook.ToDoubleArray();
            }
            _outputLayer._inputs = _hiddenLayer.ToDoubleArray();
            return _min + ((_max - _min) * _outputLayer.output); 
        }

        //testare
        private void btn_test_Click(object sender, EventArgs e)
        {
            _dataTestView.Clear();
            for(int i = 0; i < _tapLines.Count; ++i)
            {
                _dataTestView.Add(new MackbookTestDataView
                {
                    An = _dataView[_tapLines[i]].An,
                    Nume = _dataView[_tapLines[i]].Nume,
                    Fregventa = _dataView[_tapLines[i]].Fregventa,
                    Ram = _dataView[_tapLines[i]].Ram,
                    Rezolutie = _dataView[_tapLines[i]].Rezolutie,
                    Ssd = _dataView[_tapLines[i]].Ssd,
                    TipProcesor = _dataView[_tapLines[i]].TipProcesor,
                    PretCSV = _data[_tapLines[i]].pret.Value.ToString(),
                    PretRetea = CalcPrice(_data[_tapLines[i]]).ToString("F0")
                });
            }

            var binding = new BindingSource();
            binding.DataSource = _dataTestView;
            dGW_test.DataSource = binding;
        }
    }
}
