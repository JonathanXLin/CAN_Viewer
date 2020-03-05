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
        // Initial declaration of Chart_GUI object
        Chart_GUI chart_gui;

        //Status strip label
        private ToolStripStatusLabel status_text = new ToolStripStatusLabel();

        public CAN_Viewer_Main()
        {
            InitializeComponent();
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

            status_text.Size = new System.Drawing.Size(109, 17);
            status_text.Text = "[no logfile selected]";

            // Initialize treeview with logfile and database root nodes
            TreeNode root_logfile = new TreeNode("CAN Logfiles");
            treeView_tree.Nodes.Add(root_logfile);

            TreeNode root_database = new TreeNode("CAN Databases");
            treeView_tree.Nodes.Add(root_database);

            // Set Chart_GUI chart instance to default chart in designer
            chart_gui = new Chart_GUI(chart, checkedListBox_signals);

            // Initialize mouse wheel event in chart_gui
            chart_gui.initialize_mouse_wheel_event(this);

            // Initialize form resize event in chart_gui
            chart_gui.initialize_form_resize_event(this);
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
                    // Check if duplicate
                    if (chart_gui.database_set.databases.Exists(x => !Convert.ToBoolean(string.Compare(x.path, openFileDialog.FileName))))
                        MessageBox.Show("Database already included!");
                    else
                    {
                        // Create new database and add to database_set
                        Database new_database = new Database();
                        new_database.path = openFileDialog.FileName;
                        new_database.parse(new_database.path);

                        chart_gui.database_set.databases.Add(new_database);

                        // Add new database to treeview
                        TreeNode new_database_node = new TreeNode(Path.GetFileName(new_database.path));
                        treeView_tree.Nodes[1].Nodes.Add(new_database_node);
                        treeView_tree.Nodes[1].Expand();
                    }
                    
                }
            }
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
                    // Check if same as currently existing
                    if (!Convert.ToBoolean(string.Compare(chart_gui.logfile.path, openFileDialog.FileName)))
                        MessageBox.Show("Same logfile already loaded!");
                    else
                    {
                        // Set logfile path, will be parsed in Logfile_Parser
                        chart_gui.logfile.path = openFileDialog.FileName;

                        // Update status bar text
                        status_text.Text = Path.GetFileName(chart_gui.logfile.path);

                        // Add to treeview
                        TreeNode new_logfile_node = new TreeNode(Path.GetFileName(chart_gui.logfile.path));
                        treeView_tree.Nodes[0].Nodes.Add(new_logfile_node);
                        treeView_tree.Nodes[0].Expand();

                        // Populate checkedListBox with all logfile signals
                        chart_gui.logfile.update_CheckedListBox(checkedListBox_signals);

                        //// Update now updated logfile in chart_gui
                        //if (chart_gui.update_logfile(logfile, database_set, checkedListBox_signals) == 0)
                        //    throw new ArgumentException("chart_gui cannot be updated with null logfile argument");

                        //Logfile_Parser parser = new Logfile_Parser(chart_gui, database_set, checkedListBox_signals);
                        //parser.Show();

                        // Set checked list box
                        chart_gui.checked_list_box = checkedListBox_signals;

                        // Parse logfile
                        chart_gui.logfile.parse(chart_gui.database_set);
                        // Update logfile in chart
                        chart_gui.update_logfile(chart_gui.checked_list_box);

                        // Set initial chart to show timeslice of entire logfile
                        chart_gui.set_initial_timeslice_data();
                    }
                }
            }

            /*
            // Set initial gui window to entire logfile timeslice, with some padding
            if (logfile.num_points != 0)
            {
                gui.time_start = logfile.point_list[0].timestamp - 0.1 * (logfile.point_list[logfile.num_points - 1].timestamp - logfile.point_list[0].timestamp);
                gui.time_end = logfile.point_list[logfile.num_points - 1].timestamp + 0.1 * (logfile.point_list[logfile.num_points - 1].timestamp - logfile.point_list[0].timestamp);
            }
            else
                MessageBox.Show("Logfile empty");
            */
        }

        // Sandbox area for testing/debugging stuff, not used
        private void chart_Click(object sender, EventArgs e)
        {
            if (chart_gui.logfile != null)
            {
                Timeslice max_timeslice;
                max_timeslice.start = 0;
                max_timeslice.end = chart_gui.logfile.point_list[chart_gui.logfile.point_list.Count - 1].timestamp;

                chart_gui.update_timeslice_data(max_timeslice);

                //    using (StreamWriter file = new StreamWriter("test.txt"))
                //    {
                //        foreach (Logfile_Point point in logfile.point_list)
                //        {
                //            /*
                //            string raw_data = "";
                //            for (int i = 0; i < point.num_bytes; i++)
                //                raw_data += point.data[i].ToString() + " ";
                //            file.WriteLine("Message Number: " + point.point_number + " Timestamp: " + point.timestamp + " Raw Data: " + raw_data);

                //            foreach (Logfile_Signal_Point signal_point in point.signal_point_list)
                //            {
                //                file.WriteLine("\t" + signal_point.name + ": " + signal_point.value);
                //            }
                //            */

                //            int index = point.signal_point_list.IndexOf(point.signal_point_list.Find(signal => !Convert.ToBoolean(string.Compare(signal.name, "Total_Engine_Hours"))));

                //            if (index != -1)
                //            {
                //                string raw_data = "";
                //                for (int i = 0; i < point.num_bytes; i++)
                //                    raw_data += point.data[i].ToString() + " ";
                //                file.WriteLine("Message Number: " + point.point_number + " Timestamp: " + point.timestamp + " Raw Data: " + raw_data);

                //                file.WriteLine("\t" + point.signal_point_list[index].name + ":\t\t\t" + point.signal_point_list[index].value);
                //            }
                //        }
                //    }
            }

            /*
            for (int i=0; i<checkedListBox_signals.CheckedItems.Count; i++)
            {
                MessageBox.Show(checkedListBox_signals.CheckedItems[i].ToString());
            }
            */
        }

        private void checkedListBox_signals_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            List<string> checked_items = new List<string>();
            foreach (var item in checkedListBox_signals.CheckedItems)
                checked_items.Add(item.ToString());

            if (e.NewValue == CheckState.Checked)
                checked_items.Add(checkedListBox_signals.Items[e.Index].ToString());
            else
                checked_items.Remove(checkedListBox_signals.Items[e.Index].ToString());

            chart_gui.update_displayed_chart_areas(checked_items);

            //string message = "";
            //foreach (string checked_name in checkedItems)
            //    message += checked_name + " ";
            //MessageBox.Show(message);
        }

        // Sandbox area for testing/debugging stuff, not used
        private void checkedListBox_signals_Click(object sender, EventArgs e)
        {
            
        }
    }
}
