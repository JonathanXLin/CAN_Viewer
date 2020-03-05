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

namespace CAN_Viewer
{
    public partial class Loading_Bar_Parser : Form
    {
        public Chart_GUI chart_gui;

        public Loading_Bar_Parser(Chart_GUI chart_gui_)
        {
            InitializeComponent();

            chart_gui = chart_gui_;

            label_FileName.Text = Path.GetFileName(chart_gui.logfile.path);
        }

        private void Loading_Bar_Worker_Load(object sender, EventArgs e)
        {
            parser.RunWorkerAsync();
        }

        private void parser_DoWork(object sender, DoWorkEventArgs e)
        {
            // Populate logfile object
            chart_gui.logfile.num_points = 0;
            chart_gui.logfile.num_unique_signals = 0;

            // Find number of messages in file
            string[] lastLine = File.ReadLines(chart_gui.logfile.path).Last().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int num_bytes_last_line = int.Parse(lastLine[4]);
            int num_messages = int.Parse(lastLine[5 + num_bytes_last_line]);

            // Keep track of progress percentage
            int progress_percentage = 0;
            int progress_percentage_prev = 0;

            // Populates database_ object with database data
            using (StreamReader reader = new StreamReader(chart_gui.logfile.path))
            {
                // Continue reading if not at end
                while (!reader.EndOfStream)
                {
                    // Read next line
                    string curr_log_line = reader.ReadLine();
                    string[] log_line_words = curr_log_line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Checks if it not trigger line, if is, skip
                    if (Convert.ToBoolean(string.Compare(log_line_words[1], "Trigger")))
                    {
                        // Add and populate new logfile point
                        Logfile_Point new_point = new Logfile_Point();

                        // Initialize signal_point list
                        new_point.num_signal_points = 0;

                        new_point.timestamp = Double.Parse(log_line_words[0]);
                        new_point.channel = Int32.Parse(log_line_words[1]);
                        int index_of_x = log_line_words[2].IndexOf(@"x");

                        if (index_of_x != -1) // Valid IDs end in x
                            new_point.id = Int64.Parse(log_line_words[2].Remove(index_of_x, 1));
                        else
                            new_point.id = Int64.Parse(log_line_words[2]);

                        if (Equals(log_line_words[3], "Rx"))
                            new_point.direction = 0;
                        else if (Equals(log_line_words[3], "Tx"))
                            new_point.direction = 1;
                        else
                            new_point.direction = 2;

                        new_point.num_bytes = int.Parse(log_line_words[4]);

                        new_point.data = new int[new_point.num_bytes];

                        for (int i = 0; i < new_point.num_bytes; i++)
                            new_point.data[i] = int.Parse(log_line_words[5 + i]);

                        new_point.point_number = int.Parse(log_line_words[5 + new_point.num_bytes]);
                        //MessageBox.Show(new_point.point_number.ToString());

                        // Matches point with corresponding message_format in database file
                        // Finds and stores database and database message indices used by point
                        new_point.database_index = -1; // These are initially set to not found, but if found in any database, will be updated
                        new_point.database_message_index = -1; // These are initially set to not found, but if found in any database, will be updated

                        foreach (Database current_database in chart_gui.database_set.databases)
                        {
                            if (current_database.message_list.IndexOf(current_database.message_list.Find(x => x.id == new_point.id)) != -1)
                            {
                                new_point.database_index = chart_gui.database_set.databases.IndexOf(current_database);
                                new_point.database_message_index = current_database.message_list.IndexOf(current_database.message_list.Find(x => x.id == new_point.id));
                            }
                        }

                        // Convert bytewise data representation into bit array
                        bool[] data_bit_array = new bool[new_point.num_bytes * 8];

                        for (int i = 0; i < new_point.num_bytes; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                data_bit_array[(i * 8) + j] = Convert.ToBoolean(new_point.data[i] & (1 << j));
                            }
                        }

                        // If database message format not found for point
                        if (new_point.database_index == -1 || new_point.database_message_index == -1)
                        {
                            //MessageBox.Show("Not Decoding logfile line number <" + new_point.point_number + "> with Timestamp <" + new_point.timestamp + ">");
                        }
                        else
                        {
                            // Populate signal point list with all signals contained in logfile point
                            int num_signals = chart_gui.database_set.databases[new_point.database_index].message_list[new_point.database_message_index].num_signals; // Number of signals in current point

                            for (int i = 0; i < num_signals; i++)
                            {
                                // Calculate raw value using little endian
                                int start_bit = chart_gui.database_set.databases[new_point.database_index].message_list[new_point.database_message_index].signal_list[i].start_bit;
                                int num_bits = chart_gui.database_set.databases[new_point.database_index].message_list[new_point.database_message_index].signal_list[i].length;

                                long raw_value = 0; // Raw little-endian value of signal

                                for (int bit_array_index = start_bit; bit_array_index < start_bit + num_bits; bit_array_index++)
                                {
                                    raw_value += Convert.ToInt64(data_bit_array[bit_array_index]) * Convert.ToInt64(Math.Pow(2, bit_array_index - start_bit));
                                }
                                //MessageBox.Show("Logfile Message #: " + new_point.point_number + " Timestamp: " + new_point.timestamp + " Signal Value: " + raw_value);

                                // Apply scaling, and offset (note, endian is not considered because I don't know what it does)
                                double scale = chart_gui.database_set.databases[new_point.database_index].message_list[new_point.database_message_index].signal_list[i].scale;
                                double offset = chart_gui.database_set.databases[new_point.database_index].message_list[new_point.database_message_index].signal_list[i].offset;

                                double final_value = (raw_value * scale) + offset;

                                // Add new signal to signal list and store values in new signal
                                Logfile_Signal_Point new_signal_point = new Logfile_Signal_Point();
                                new_signal_point.name = chart_gui.database_set.databases[new_point.database_index].message_list[new_point.database_message_index].signal_list[i].name;
                                new_signal_point.value = final_value;
                                new_point.signal_point_list.Add(new_signal_point);
                                new_point.num_signal_points++;

                                // Check if signal is new signal and if so, add to unique_signal list
                                if (chart_gui.logfile.unique_signals.Exists(x => x == new_signal_point.name) == false)
                                {
                                    chart_gui.logfile.unique_signals.Add(new_signal_point.name);
                                    chart_gui.logfile.num_unique_signals++;
                                }
                            }
                        }

                        // Adds logfile point to point list
                        chart_gui.logfile.point_list.Add(new_point);
                        chart_gui.logfile.num_points++;

                        // Track progress and update progress bar
                        progress_percentage = (chart_gui.logfile.num_points) * 100 / num_messages;
                        if (progress_percentage > progress_percentage_prev)
                            parser.ReportProgress(progress_percentage);
                        progress_percentage_prev = progress_percentage;

                        // Checks if cancelled
                        if (parser.CancellationPending)
                        {
                            e.Cancel = true;

                            // Reverts all changes to logfile
                            chart_gui.logfile = new Logfile();

                            return;
                        }
                    }
                }
            }
        }

        private void parser_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            label_Status.Text = e.ProgressPercentage.ToString() + "%";
        }

        private void parser_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            if (parser.WorkerSupportsCancellation)
            {
                parser.CancelAsync();
                label_Status.Text = "Cancelled";
                this.Enabled = false;
            }
        }
    }
}
