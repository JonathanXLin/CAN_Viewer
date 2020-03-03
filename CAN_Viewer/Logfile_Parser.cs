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
using System.IO;

namespace CAN_Viewer
{
    public partial class Logfile_Parser : Form
    {
        public Logfile logfile;
        public Database_Set database_set;
        public Chart_GUI chart_gui;
        public CheckedListBox checked_list_box;

        public Logfile_Parser(Chart_GUI chart_gui_, Database_Set database_set_, CheckedListBox checked_list_box_)
        {
            InitializeComponent();

            chart_gui = chart_gui_;
            database_set = database_set_;
            checked_list_box = checked_list_box_;

            if (parser.IsBusy != true)
            {
                // Start the asynchronous operation
                parser.RunWorkerAsync();
            }
        }

        private void Logfile_Parser_Load(object sender, EventArgs e)
        {
            label_fileName.Text = Path.GetFileName(logfile.path);
            label_Action.Text = "Parsing...";
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            if (parser.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation
                parser.CancelAsync();
            }
        }

        private void parser_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            logfile.parse(database_set);
            chart_gui.update_logfile(checked_list_box);

            /*
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

            if (!Convert.ToBoolean(curr_point % 50))
                worker.ReportProgress(Convert.ToInt32(100 * Convert.ToDouble(curr_point) / num_points));
            */
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
