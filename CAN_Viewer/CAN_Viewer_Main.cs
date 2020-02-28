using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;

namespace CAN_Viewer
{
    public partial class CAN_Viewer_Main : Form
    {
        // Initial declarations of database set and logfile objects
        Database_Set database_set = new Database_Set();
        Logfile logfile = new Logfile();

        // Initial declaration of canvas object with time zero and width zero, populated when logfile is loaded
        Canvas gui = new Canvas(0.0, 0.0);

        // Initial declaration of Chart_GUI object
        Chart_GUI chart_gui;

        // Graphics object, initialized in main load
        Graphics g;

        //Status strip label
        private ToolStripStatusLabel status_text = new ToolStripStatusLabel();

        public CAN_Viewer_Main()
        {
            InitializeComponent();
            //this.MouseWheel += new MouseEventHandler(canvas_MouseWheel);
        }

        private void CAN_Viewer_Main_Load(object sender, EventArgs e)
        {
            // Initialize status strip with "none" text
            statusStrip_status.GripStyle = ToolStripGripStyle.Hidden;
            statusStrip_status.Items.AddRange(new ToolStripItem[] { status_text });

            statusStrip_status.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip_status.ShowItemToolTips = true;
            statusStrip_status.SizingGrip = false;
            statusStrip_status.Stretch = false;
            statusStrip_status.TabIndex = 0;

            logfile.path = "[no logfile selected]";

            status_text.Size = new System.Drawing.Size(109, 17);
            status_text.Text = logfile.path;

            // Initialize treeview with logfile and database root nodes
            TreeNode root_logfile = new TreeNode("CAN Logfiles");
            treeView_tree.Nodes.Add(root_logfile);

            TreeNode root_database = new TreeNode("CAN Databases");
            treeView_tree.Nodes.Add(root_database);

            // Initialize graphics object
            g = canvas.CreateGraphics();

            // Initialize gui timeslice to be 10s
            gui.time_start = -1.0;
            gui.time_end = 9.0;
            gui.update_time_tickmarks(canvas);

            // Set Chart_GUI chart instance to default chart in designer
            chart_gui = new Chart_GUI(chart);

            // Initialize mouse wheel event in chart_gui
            chart_gui.initialize_mouse_wheel_event(this);
        }

        private void openToolStripMenuItem_logfile_Click(object sender, EventArgs e)
        {
            // Open file dialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Parse logfile
                    logfile.path = openFileDialog.FileName;
                    logfile.parse(logfile.path, database_set);

                    // Update status bar text
                    status_text.Text = Path.GetFileName(logfile.path);

                    // Add to treeview
                    TreeNode new_logfile_node = new TreeNode(Path.GetFileName(logfile.path));
                    treeView_tree.Nodes[0].Nodes.Add(new_logfile_node);
                    treeView_tree.Nodes[0].Expand();
                }
            }

            // Populate checkedListBox with all logfile signals
            logfile.update_CheckedListBox(checkedListBox_signals);

            // Set initial gui window to entire logfile timeslice, with some padding
            if (logfile.num_points != 0)
            {
                gui.time_start = logfile.point_list[0].timestamp - 0.1 * (logfile.point_list[logfile.num_points - 1].timestamp - logfile.point_list[0].timestamp);
                gui.time_end = logfile.point_list[logfile.num_points - 1].timestamp + 0.1 * (logfile.point_list[logfile.num_points - 1].timestamp - logfile.point_list[0].timestamp);
            }
            else
                MessageBox.Show("Logfile empty");

            // Find first and last indices of logfile point_list in gui timeslice
            int start_index = logfile.point_list.IndexOf(logfile.point_list.Find(x => x.timestamp >= gui.time_start));
            int end_index = logfile.point_list.IndexOf(logfile.point_list.FindLast(x => x.timestamp <= gui.time_end));

            if (end_index - start_index > 0)
                for (int i = 0; i < end_index - start_index; i++)
                {

                }
        }

        private void openToolStripMenuItem_database_Click(object sender, EventArgs e)
        {
            // Open file dialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "dbc files (*.dbc)|*.dbc";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Create new database and add to database_set
                    Database new_database = new Database();
                    new_database.path = openFileDialog.FileName;
                    new_database.parse(new_database.path);

                    database_set.databases.Add(new_database);

                    // Add new database to treeview
                    TreeNode new_database_node = new TreeNode(Path.GetFileName(new_database.path));
                    treeView_tree.Nodes[1].Nodes.Add(new_database_node);
                    treeView_tree.Nodes[1].Expand();
                }
            }
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            
        }

        // Sandbox area for testing/debugging stuff
        private void canvas_Click(object sender, EventArgs e)
        {
            /*
            Pen pen = new Pen(Color.White);

            Point[] line_points =
            {
                new Point(0, 0),
                new Point(canvas.Width/2, canvas.Height/2)
            };

            g.DrawLines(pen, line_points);
            */
        }
        private void canvas_MouseWheel(object sender, MouseEventArgs e)
        {
            canvas.Focus();

            // Timeslice bounds increased/decreased by 10% of edge of gui window to current mouse position when middle mouse wheel is moved
            double timeslice_width = gui.time_end - gui.time_start;
            double left_bound_adjustment_weight = e.X / canvas.Width;
            double right_bound_adjustment_weight = 1 - left_bound_adjustment_weight;

            if (e.Delta > 0)
            {
                gui.time_start += timeslice_width * 0.1 * left_bound_adjustment_weight;
                gui.time_end -= timeslice_width * 0.1 * right_bound_adjustment_weight;
            }
            else if (e.Delta < 0)
            {
                gui.time_start -= timeslice_width * 0.1 * left_bound_adjustment_weight;
                gui.time_end += timeslice_width * 0.1 * right_bound_adjustment_weight;
            }

            //MessageBox.Show(gui.time_start.ToString() + " " + gui.time_end.ToString());

            // Update gui tickmarkers
            gui.update_time_tickmarks(canvas);
        }

        private void CAN_Viewer_Main_Resize(object sender, EventArgs e)
        {
            g.Clear(Color.Black);
            this.Invalidate();

            gui.update_time_tickmarks(canvas);
        }
    }
}
