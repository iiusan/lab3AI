using lab3AI.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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
using System.Threading;

namespace lab3AI
{
    public partial class Form1 : Form
    {
        public static double rata = 0.02;

        private const int HEADER_COUNT = 7;
        private double _min = 999, _max = 6599;
        private const int nLimDate = 202;
        private const int nLimAsc = 14;

        private int nEpoch = 1200;
        private int cEpoch = 0;

        private bool reset = false;
        private List<MacbookDataView> _dataView = new List<MacbookDataView>();
        private List<MackbookCodifDataView> _dataCodifView = new List<MackbookCodifDataView>();
        private List<MackbookTestDataView> _dataTestView = new List<MackbookTestDataView>();
        List<int> _tapLines = new List<int>();
        private List<Macbook> _data = new List<Macbook>();
        private string _file = null;
        private FunctionSeries _func = new FunctionSeries();
        private PlotModel _modelPlot = new PlotModel();

        private List<Neuron> _hiddenLayer = new List<Neuron>();//layer h --1
        private List<Neuron> _hiddenLayer2 = new List<Neuron>();//layer h --1

        private Neuron _outputLayer;

        public Form1()
        {
            InitializeComponent();
            dGW_date.DataSource = _dataView;
            dGW_codif.DataSource = _dataCodifView;
            dGW_date.DataSource = _dataTestView;

            InitGraph();
            cB_an.SelectedIndex = 0;
            cB_diagonala.SelectedIndex = 0;
            cB_fregventa.SelectedIndex = 0;
            cB_nume.SelectedIndex = 0;
            cB_procesor.SelectedIndex = 0;
            cB_ram.SelectedIndex = 0;
            cB_ssd.SelectedIndex = 0;


            LearnInit();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;


            //_file = @"C:\Users\Ionut\Desktop\AI.csv";
            //LoadCsv();
        }


        private void InitGraph()
        {

            _modelPlot = new PlotModel { Title = "Grafic Eroare" };
            LinearAxis xA = new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 3000 };
           // LinearAxis yA = new LinearAxis { Position = AxisPosition.Left, Minimum = 1, Maximum = 500 };
            LinearAxis yA = new LinearAxis { Position = AxisPosition.Left, Minimum = 0.01, Maximum = 0.1 };
            _modelPlot.Axes.Add(xA);
            _modelPlot.Axes.Add(yA);

            _func = new FunctionSeries();
            
            _func.Color = OxyColor.FromRgb(20, 20, 20);

            _modelPlot.Series.Add(_func);
            
            plotView1.Model = _modelPlot;
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
                        diagonala = new Diagonala {  Value = Convert.ToDouble(values[1]) },
                        tipProcesor = new TipProcesor { Value = values[2] },
                        fregventa = new Fregventa { Value = Convert.ToDouble(values[3]) },
                        ram = new RAM { Value = Convert.ToDouble(values[4]) },
                        ssd = new SSD { Value = Convert.ToDouble(values[5]) },
                        an = new An { Value = Convert.ToDouble(values[6]) },
                        //rezolutie = new Rezolutie { Value = values[7] },
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
            InitGraph();
            cEpoch = 0;
            for (int i = 0; i < nLimAsc; ++i)
            {
                Neuron n = new Neuron(HEADER_COUNT);
                n.RandomizeWeights();
                _hiddenLayer.Add(n);
            }

            for (int i = 0; i < nLimAsc; ++i)
            {
                Neuron n = new Neuron(HEADER_COUNT);
                n.RandomizeWeights();
                _hiddenLayer2.Add(n);
            }
            _outputLayer = new Neuron(nLimAsc);
            _outputLayer.RandomizeWeights();
        }

        private void Learn(bool init)
        {
            if(init)
                LearnInit();
            for (int i = cEpoch; i < nEpoch; ++i, cEpoch++)
            {
                var e = DoEpoch();
                numericUpDown2.Value = Convert.ToDecimal(e);
                numericUpDown2.Refresh();
                if(e < 500)
                _func.Points.Add(new DataPoint(i, DoEpoch()));
                plotView1.Refresh();
            }
        }

        private void DoEpoch2()
        {
            for (int i = 0; i < nLimDate; i++)
            {
                if (_tapLines.Contains(i))
                    continue;
                foreach (var neuron in _hiddenLayer)
                {
                    neuron._inputs = _data[i].ToDoubleArray();
                }

                foreach (var neuron in _hiddenLayer2)
                {
                    neuron._inputs = _hiddenLayer.ToDoubleArray();
                }
                _outputLayer._inputs = _hiddenLayer2.ToDoubleArray();

                _outputLayer._error = Utils.Derivative(_outputLayer.output) * (_data[i].pret.NormalizedValue - _outputLayer.output);
                Debug.WriteLine(Math.Abs(_outputLayer._error).ToString("F9"));
                _outputLayer.adjustWeights();

                for (int w = 0; w < _hiddenLayer2.Count; ++w)
                {
                    _hiddenLayer2[w]._error = Utils.Derivative(_hiddenLayer2[w].output) * _outputLayer._error * _outputLayer._weights[w];
                }

                for (int w = 0; w < _hiddenLayer.Count; ++w)
                {
                    _hiddenLayer[w]._error = Utils.Derivative(_hiddenLayer[w].output) * _hiddenLayer2.Sum(x => x._error * x._weights[w]);// _hiddenLayer2[w]._error * _hiddenLayer2[w]._weights[w];
                }

                foreach (var neuron in _hiddenLayer)
                {
                    neuron.adjustWeights();
                }

            }
        }

        private double DoEpoch()
        {
            List<double> er = new List<double>();
            double gError = 0;
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


              
                er.Add( Math.Abs(_data[i].pret.NormalizedValue - _outputLayer.output));
                 // er.Add( Math.Abs(_data[i].pret.Value - (_min + ((_max - _min) * _outputLayer.output))));
               // er.Add(Math.Abs((_max - _min) * _outputLayer.output));
                Debug.WriteLine(Math.Abs(_outputLayer._error).ToString("F9"));
                _outputLayer.adjustWeights();

                for(int w =0; w < _hiddenLayer.Count; ++w)
                {
                     _hiddenLayer[w]._error = Utils.Derivative(_hiddenLayer[w].output) * _outputLayer._error * _outputLayer._weights[w];
                     //_hiddenLayer[w]._error = Math.Pow(_outputLayer._error * _outputLayer._weights[w], 2) / 2;
                }

                foreach (var neuron in _hiddenLayer)
                {
                    neuron.adjustWeights();
                }
            }
            return er.Average();

        }

    


        private void button2_Click(object sender, EventArgs e)
        {
            // Learn(true);
            button2.Enabled = false;
            button3.Enabled = false;
            numericUpDown1.Enabled = false;
            button5.Enabled = true;
            reset = true;
            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Learn(false);
            button2.Enabled = false;
            button3.Enabled = false;
            numericUpDown1.Enabled = false;
            button5.Enabled = true;
            reset = false;
            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }
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

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            rata = Convert.ToDouble(numericUpDown1.Value);
        }

        private void button4_Click(object sender, EventArgs e)
        {

            Macbook m = new Macbook
            {
                nume = new Nume { Value = cB_nume.Text },
                diagonala = new Diagonala { Value = Convert.ToDouble(cB_diagonala.Text) },
                tipProcesor = new TipProcesor { Value = cB_procesor.Text },
                fregventa = new Fregventa { Value = Convert.ToDouble(cB_fregventa.Text) },
                ram = new RAM { Value = Convert.ToDouble(cB_ram.Text) },
                ssd = new SSD { Value = Convert.ToDouble(cB_ssd.Text) },
                an = new An { Value = Convert.ToDouble(cB_an.Text) }

            };
          
            tB_pret.Text = CalcPrice(m).ToString("F0");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (reset)
                LearnInit();
            for (int i = cEpoch; ShouldLearn(); ++i, cEpoch++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                var e1 = DoEpoch();
                worker.ReportProgress(0,new Tuple<double, double>(e1, i));
                //numericUpDown2.Value = Convert.ToDecimal(e1);
                //numericUpDown2.Refresh();
                //if (e1 < 500)
                //    _func.Points.Add(new DataPoint(i, e1));
                //plotView1.Refresh();
            }
            backgroundWorker1.CancelAsync();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                backgroundWorker1.CancelAsync();
            }
        }

        private bool ShouldLearn()
        {
            try
            {
                if (radioButton2.Checked)//eroare
                {
                    if (_func.Points.Count() != 0)
                        return _func.Points.Last().Y < Convert.ToDouble(numericUpDown3.Value) ? false : true;
                    return true;
                }
                return cEpoch >= Convert.ToInt32(numericUpDown4.Value) ? false : true;
            }
            catch { }
            return true;
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var e1 = e.UserState as Tuple<double, double>;
            numericUpDown2.Value = Convert.ToDecimal(e1.Item1);
            numericUpDown2.Refresh();
            if (e1.Item1 < 500)
                _func.Points.Add(new DataPoint(e1.Item2, e1.Item1));
            plotView1.Refresh();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button2.Enabled = true;
            button3.Enabled = true;
            numericUpDown1.Enabled = true;
            button5.Enabled = false;
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
                    Diagonala = _dataView[_tapLines[i]].Diagonala,
                    Fregventa = _dataView[_tapLines[i]].Fregventa,
                    Ram = _dataView[_tapLines[i]].Ram,
                   // Rezolutie = _dataView[_tapLines[i]].Rezolutie,
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
