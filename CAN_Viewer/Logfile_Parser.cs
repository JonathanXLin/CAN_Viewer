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
        public Chart_GUI chart_gui;

        public Logfile_Parser(Chart_GUI chart_gui_)
        {
            InitializeComponent();

            chart_gui = chart_gui_;
        }

        private void Logfile_Parser_Load(object sender, EventArgs e)
        {
            label_fileName.Text = Path.GetFileName(chart_gui.logfile.path);
            label_Action.Text = "Parsing...";

            // Parse logfile
            chart_gui.logfile.parse(chart_gui.database_set);
            // Update logfile in chart
            chart_gui.update_logfile(chart_gui.checked_list_box);

            label_Action.Text = "Done";

            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            
        }
    }
}
