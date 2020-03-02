using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CAN_Viewer
{
    public partial class Logfile_Parser : Form
    {
        private Logfile logfile;
        public List<Series> series;

        public Logfile_Parser(Logfile logfile_, List<Series> series_)
        {
            InitializeComponent();

            logfile = logfile_;
            series = series_;

            if (parser.IsBusy != true)
            {
                // Start the asynchronous operation.
                parser.RunWorkerAsync();
            }
        }

        private void Logfile_Parser_Load(object sender, EventArgs e)
        {
            label_fileName.Text = logfile.path;
            label_Action.Text = "Parsing...";
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            if (parser.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                parser.CancelAsync();
            }
        }

        private void parser_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            long num_points = logfile.point_list.Count;
            long curr_point = 0;

            // For every signal in every message, add the data to that signal's series
            foreach (Logfile_Point point in logfile.point_list)
            {
                foreach (Logfile_Signal_Point signal_point in point.signal_point_list)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        series.Find(x => !Convert.ToBoolean(string.Compare(signal_point.name, x.Name))).Points.AddXY(point.timestamp, signal_point.value);
                        //MessageBox.Show(series.Find(x => !Convert.ToBoolean(string.Compare(signal_point.name, x.Name))).Name);
                    }
                }

                curr_point++;
                worker.ReportProgress(Convert.ToInt32(100 * Convert.ToDouble(curr_point)/num_points));
            }
        }

        private void parser_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void parser_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                label_Action.Text = "Cancelled";
            }
            else if (e.Error != null)
            {
                label_Action.Text = "Error: " + e.Error.Message;
            }
            else
            {
                label_Action.Text = "Done";
            }

            this.Close();
        }
    }
}
