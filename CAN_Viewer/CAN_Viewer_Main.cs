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
    public partial class CAN_Viewer_Main : Form
    {
        // Initial declarations of database and logfile objects, assuming only one of each will be loaded at any time
        database_file_t database_file = new database_file_t();
        logfile_t logfile = new logfile_t();

        // Initial declaration of canvas object with time zero and width zero, populated when logfile is loaded
        Canvas_t gui = new Canvas_t(0.0, 0.0);

        // Graphics object, initialized in main load
        Graphics g;

        //Status strip label
        private ToolStripStatusLabel status_text = new ToolStripStatusLabel();

        public CAN_Viewer_Main()
        {
            InitializeComponent();
        }

        // Logfile, which contains points
        public class logfile_t
        {
            public string path;

            public List<logfile_point_t> point_list;
            public int num_points;
        }
        // Direction enumeration for logfile point direction, not yet used
        enum Direction { Rx, Tx, unknown };
        // Logfile point, corresponds to single line in logfile
        public class logfile_point_t
        {
            public double timestamp;
            public int channel;
            public long id;
            public int direction;
            public int num_bytes;
            public int[] data;
            public int point_number;

            public int database_message_index; // Index of database message format, -1 if does not exist
            public List<logfile_signal_point_t> signal_point_list; // List of signals that are contained in logfile point
            public int num_signal_points;
        }
        // Logfile signal, corresponds to single graphical datapoint on GUI
        public class logfile_signal_point_t
        {
            public string name;
            public double value;
        }

        // Database file, which contains message formats
        public class database_file_t
        {
            public string path;

            public List<message_format_t> message_list;
            public int num_messages;
        }
        // Database file message format, which contains signal formats
        public class message_format_t
        {
            public string name;
            public long id;
            public int length; // Bytes
            public string sender;

            public List<signal_format_t> signal_list;
            public int num_signals;
        }
        // Endian enumeration for DBC signal format, not yet used
        enum Endian { little_endian, big_endian, unknown };
        // Database file signal format
        public class signal_format_t
        {
            public string name;
            public int start_bit;
            public int length;
            public double scale;
            public double offset;
            public double min;
            public double max;
            public string unit;
            public int endian;
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
                    // Populate logfile object
                    logfile.path = openFileDialog.FileName;
                    logfile.point_list = new List<logfile_point_t>();
                    logfile.num_points = 0;

                    // Update status bar with file name
                    status_text.Text = Path.GetFileName(logfile.path);

                    // Add logfile to treeview
                    TreeNode new_logfile_node = new TreeNode(Path.GetFileName(logfile.path));
                    treeView_tree.Nodes[0].Nodes.Add(new_logfile_node);
                    treeView_tree.Nodes[0].Expand();

                    // Populates database_file object with database data
                    using (StreamReader reader = new StreamReader(logfile.path))
                    {
                        // Continue reading if not at end
                        while (!reader.EndOfStream)
                        {
                            // Read next line
                            string curr_log_line = reader.ReadLine();
                            string[] log_line_words = curr_log_line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            // Add and populate new logfile point
                            logfile_point_t new_point = new logfile_point_t();

                            // Initialize signal_point list
                            new_point.signal_point_list = new List<logfile_signal_point_t>();
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
                            // Finds and stores index of database_file message_list that is used to decode curr_point
                            new_point.database_message_index = database_file.message_list.IndexOf(database_file.message_list.Find(x => x.id == new_point.id));

                            // Convert bytewise data representation into bit array
                            bool[] data_bit_array = new bool[new_point.num_bytes * 8];

                            for (int i=0; i< new_point.num_bytes; i++)
                            {
                                for (int j=0; j<8; j++)
                                {
                                    data_bit_array[(i * 8) + j] = Convert.ToBoolean(new_point.data[i] & (1 << j));
                                }
                            }

                            // If database message format not found for point
                            if (new_point.database_message_index == -1)
                            {
                                //MessageBox.Show("Not Decoding logfile line number <" + new_point.point_number + "> with Timestamp <" + new_point.timestamp + ">");
                            }
                            else
                            {
                                // Populate signal point list with all signals contained in logfile point
                                int num_signals = database_file.message_list[new_point.database_message_index].num_signals; // Number of signals in current point

                                for (int i = 0; i < num_signals; i++)
                                {
                                    // Calculate raw value using little endian
                                    int start_bit = database_file.message_list[new_point.database_message_index].signal_list[i].start_bit;
                                    int num_bits = database_file.message_list[new_point.database_message_index].signal_list[i].length;

                                    long raw_value = 0; // Raw little-endian value of signal

                                    for (int bit_array_index = start_bit; bit_array_index < start_bit + num_bits; bit_array_index++)
                                    {
                                        raw_value += Convert.ToInt64(data_bit_array[bit_array_index]) * Convert.ToInt64(Math.Pow(2, bit_array_index - start_bit));
                                    }
                                    //MessageBox.Show("Logfile Message #: " + new_point.point_number + " Timestamp: " + new_point.timestamp + " Signal Value: " + raw_value);

                                    // Apply scaling, and offset (note, endian is not considered because I don't know what it does)
                                    double scale = database_file.message_list[new_point.database_message_index].signal_list[i].scale;
                                    double offset = database_file.message_list[new_point.database_message_index].signal_list[i].offset;

                                    double final_value = (raw_value * scale) + offset;

                                    // Add new signal to signal list and store values in new signal
                                    logfile_signal_point_t new_signal_point = new logfile_signal_point_t();
                                    new_signal_point.name = database_file.message_list[new_point.database_message_index].signal_list[i].name;
                                    new_signal_point.value = final_value;
                                    new_point.signal_point_list.Add(new_signal_point);
                                    new_point.num_signal_points++;

                                    //MessageBox.Show("Logfile Message #: " + new_point.point_number + " Timestamp: " + new_point.timestamp + " Signal Name <" + database_file.message_list[new_point.database_message_index].signal_list[i].name + ">, value: " + final_value);
                                }
                                //MessageBox.Show("Decoding logfile line number <" + new_point.point_number + "> with Timestamp <" + new_point.timestamp + ">");
                            }

                            // Adds logfile point to point list
                            logfile.point_list.Add(new_point);
                            logfile.num_points++;

                            /*
                            string test = "";
                            for (int i=0; i<curr_point.num_bytes*8; i++)
                            {
                                test += Convert.ToInt32(data_bit_array[i]).ToString();
                                if ((i + 1) % 8 == 0)
                                    test += " ";
                            }
                            MessageBox.Show(test);
                            */

                            /*
                            if (curr_point.database_message_index != -1)
                            {
                                MessageBox.Show("Timestamp: " + curr_point.timestamp.ToString() + " Message Name: " + database_file.message_list[curr_point.database_message_index].name);
                            }
                            */

                            /*
                            if (new_point.num_bytes == 0)
                            {
                                MessageBox.Show(curr_log_line + "\n\n" + new_point.timestamp + " " + new_point.channel + " " + new_point.id + " " + new_point.direction + " " + new_point.num_bytes + " " + new_point.point_number);
                                foreach (int i in new_point.data)
                                    MessageBox.Show(i.ToString());
                            }
                            */
                        }

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

                        for (int i=0; i<end_index-start_index; i++)
                        {

                        }

                        //MessageBox.Show(start_index.ToString() + " " + end_index.ToString());
                    }

                    // MessageBox.Show(logfile.point_list.Find(x => x.id == 419366480).timestamp.ToString());

                    /* Opens file in notepad
                    Process.Start("notepad.exe", logfile_path);
                    */
                }
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
                    // Populate database_file object
                    database_file.path = openFileDialog.FileName;
                    database_file.message_list = new List<message_format_t>();
                    database_file.num_messages = 0;
                    
                    // Add database to treeview
                    TreeNode new_database_node = new TreeNode(Path.GetFileName(database_file.path));
                    treeView_tree.Nodes[1].Nodes.Add(new_database_node);
                    treeView_tree.Nodes[1].Expand();

                    /* Opens file in notepad */
                    //Process.Start("notepad.exe", database_file.path);

                    // Populates database_file object with database data
                    using (StreamReader reader = new StreamReader(database_file.path))
                    {
                        // Continue reading if not at end
                        while (!reader.EndOfStream)
                        {
                            // Read next line
                            string curr_message_line = reader.ReadLine();
                            string[] message_line_words = curr_message_line.Split(' ');

                            // If line is start of message/signal format
                            if (Equals(message_line_words[0], "BO_"))
                            {
                                // Parse message line and populate message_list with new item
                                message_format_t new_message = new message_format_t();
                                new_message.name = message_line_words[2];
                                new_message.id = Int64.Parse(message_line_words[1]) - (long)(8 * Math.Pow(16, 7)); // Last digit in hex is 8 too big for some reason
                                new_message.length = Int32.Parse(message_line_words[3].Remove(0, 1));
                                new_message.sender = message_line_words[4];
                                database_file.message_list.Add(new_message);

                                //MessageBox.Show(database_file.message_list[database_file.num_messages].name);

                                // Declare instance of signal list and initializes length
                                database_file.message_list[database_file.num_messages].signal_list = new List<signal_format_t>();
                                database_file.message_list[database_file.num_messages].num_signals = 0;

                                // Increment number of messages in message_list
                                database_file.num_messages++;

                                // Continue reading while still parsing new signal formats
                                if ((char)reader.Peek() == ' ') // If next character is space, according to .dbc format, means there is at least one signal format
                                {
                                    do
                                    {
                                        string curr_signal_line = reader.ReadLine();
                                        string[] signal_line_words = curr_signal_line.Split(' ');

                                        // Parse signal line and populate signal with new item
                                        signal_format_t new_signal = new signal_format_t();
                                        new_signal.name = signal_line_words[2];

                                        int ampersand_index = signal_line_words[4].IndexOf(@"@");
                                        int pipe_index = signal_line_words[4].IndexOf(@"|");
                                        new_signal.start_bit = Int32.Parse(signal_line_words[4].Remove(pipe_index));
                                        new_signal.length = Int32.Parse((signal_line_words[4].Remove(ampersand_index)).Substring(pipe_index + 1));

                                        int comma_index = signal_line_words[5].IndexOf(@",");
                                        int close_brace_index = signal_line_words[5].IndexOf(@")");
                                        new_signal.scale = Double.Parse((signal_line_words[5].Remove(comma_index)).Remove(0, 1));
                                        new_signal.offset = Double.Parse((signal_line_words[5].Remove(close_brace_index)).Substring(comma_index + 1));

                                        int pipe2_index = signal_line_words[6].IndexOf(@"|");
                                        int close_brace2_index = signal_line_words[6].IndexOf(@"]");
                                        int extra_space_index_compensation_amount = 0;
                                        if (close_brace2_index == -1) // DBC glitch where space is put before last "]"
                                        {
                                            new_signal.min = Double.Parse((signal_line_words[6].Remove(pipe2_index)).Remove(0, 1));
                                            new_signal.max = Double.Parse(signal_line_words[6].Substring(pipe2_index + 1));

                                            extra_space_index_compensation_amount = 1;
                                        }
                                        else
                                        {
                                            new_signal.min = Double.Parse((signal_line_words[6].Remove(pipe2_index)).Remove(0, 1));
                                            new_signal.max = Double.Parse((signal_line_words[6].Remove(close_brace2_index)).Substring(pipe2_index + 1));
                                        }

                                        int close_quote_index = signal_line_words[7 + extra_space_index_compensation_amount].LastIndexOf("\"");
                                        new_signal.unit = (signal_line_words[7 + extra_space_index_compensation_amount].Remove(close_quote_index)).Remove(0, 1);
                                        if (Equals(new_signal.unit, ""))
                                            new_signal.unit = "none";

                                        if (Equals(signal_line_words[4].Substring(ampersand_index), "@1+"))
                                            new_signal.endian = 0;
                                        else if (Equals(signal_line_words[4].Substring(ampersand_index), "@1-"))
                                            new_signal.endian = 1;
                                        else
                                            new_signal.endian = 2;

                                        database_file.message_list[database_file.num_messages - 1].signal_list.Add(new_signal);
                                        database_file.message_list[database_file.num_messages - 1].num_signals++;

                                        //MessageBox.Show(curr_signal_line + "\n\n" + new_signal.name + " " + new_signal.start_bit + " " + new_signal.length + " " + new_signal.scale + " " + new_signal.offset + " " + new_signal.min + " " + new_signal.max + " " + new_signal.unit + " " + new_signal.endian);

                                    } while ((char)reader.Peek() == ' ');
                                }
                            }
                        }
                    }
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

        // Canvas class
        public class Canvas_t
        {
            public double time_start; // Time at start of canvas
            public double time_end; // Time at end of canvas, can be same as start time

            //public List<Canvas_Signal_t> signal_list;

            public Canvas_t()
            {
                time_start = 0.0;
                time_end = 0.0;

                //signal_list = new List<Canvas_Signal_t>();
            }
            public Canvas_t(double time_start_initial, double time_end_initial)
            {
                if (time_start_initial <= time_end_initial) // Initial parameters are good
                {
                    time_start = time_start_initial;
                    time_end = time_end_initial;
                }
                else // Initial parameters are bad, revert to time_start with 0 width
                {
                    time_start = time_start_initial;
                    time_end = time_start_initial;
                }

                //signal_list = new List<Canvas_Signal_t>();
            }
            /*
            public Tuple<int, int> message_list_indices_between_timestamps(double start_time, double end_time)
            {
                Tuple<int, int> result = new Tuple<int, int>(0, 0);

                // Find start and end indices of logfile message list between two timeslices
                result.Item1 = logfile.point_list.IndexOf(logfile.point_list.Find(x => x.timestamp == logfile.point_list[0].timestamp));
            }
            */
        }
        /*
        // Canvas signal class
        public class Canvas_Signal_t
        {
            public string name;
            public double max_val;
            public double min_val;
            public string unit;

            public Point center;
            public int height;

            public List<Canvas_Point_t> point_list;

            public Canvas_Signal_t()
            {
                name = "";
                max_val = 0.0;
                min_val = 0.0;
                unit = "";

                center = new Point(0, 0);
                height = 0;

                point_list = new List<Canvas_Point_t>();
            }
            public Canvas_Signal_t(string name_, double max_val_, double min_val_, string unit_, Point center_, int height_)
            {
                // Check if max_val_ is less than min_val_, should not happen
                if (max_val_ < min_val_)
                    throw new ArgumentException("max_val_ less than min_val_");
                else
                {
                    max_val = max_val_;
                    min_val = min_val_;
                }
                // Check if center_ has any negative coordinates, should not happen
                if (center_.X < 0 || center_.Y < 0)
                    throw new ArgumentException("center_ coordinates are negative");
                else
                    center = center_;
                // Check if height_ is negative, should not happen
                if (height_ < 0)
                    throw new ArgumentException("height_ less than 0");
                else
                    height = height_;

                name = name_;
                unit = unit_;

                point_list = new List<Canvas_Point_t>();
            }
        }
        // Canvas point class
        public class Canvas_Point_t
        {
            public double x_abs; // Absolute x coordinate
            public double x_rel; // Relative x coordinate in current canvas
            public double y_rel; // Relative y coordinate in current canvas

            public double value; // Value of signal at this point
            public double timestamp; // Timestamp of signal at this point

            public Canvas_Point_t()
            {
                x_abs = 0.0;
                x_rel = 0.0;
                y_rel = 0.0;

                value = 0.0;
                timestamp = 0.0;
            }
            public Canvas_Point_t(double x_abs_, double x_rel_, double y_rel_, double value_, double timestamp_)
            {
                // Check if timestamp_ or x_abs_ are negative, should not happen
                if (x_abs_ < 0 || timestamp_ < 0)
                {
                    throw new ArgumentException("absolute X coordinate or timestamp initial value negative");
                }
                else
                {
                    x_abs = x_abs_;
                    timestamp = timestamp_;
                }

                x_rel = x_rel_;
                y_rel = y_rel_;
                value = value_;
            }
        }
        */
    }
}
