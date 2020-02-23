﻿using System;
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
        // CAN file path
        private string logfile_path;

        database_file_t database_file = new database_file_t();

        //Status strip label
        private ToolStripStatusLabel status_text = new ToolStripStatusLabel();

        public CAN_Viewer_Main()
        {
            InitializeComponent();
        }

        public class database_file_t
        {
            public string path;

            public List<message_format_t> message_list;
            public int num_messages;
        }
        public class message_format_t
        {
            public string name;
            public long id;
            public int length; // Bytes
            public string sender;

            public List<signal_format_t> signal_list;
            public int num_signals;
        }

        // Endian enumeration for DBC signal format
        enum Endian { little_endian, big_endian, unknown };

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

            status_text.Size = new System.Drawing.Size(109, 17);
            status_text.Text = "none";
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
                    // Get the path of specified file
                    logfile_path = openFileDialog.FileName;

                    // Update status bar with file name
                    status_text.Text = Path.GetFileName(logfile_path);

                    /* Opens file in notepad
                    Process.Start("notepad.exe", logfile_path);
                    */

                    /* Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
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
                    // Get the path of specified file
                    database_file.path = openFileDialog.FileName;

                    // Declare instance of message list and initializes length
                    database_file.message_list = new List<message_format_t>();
                    database_file.num_messages = 0;

                    // Update status bar with file name
                    status_text.Text = Path.GetFileName(logfile_path) + " | " + Path.GetFileName(database_file.path);

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
                                new_message.id = Int64.Parse(message_line_words[1]);
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
                                            new_signal.endian = 1;
                                        else if (Equals(signal_line_words[4].Substring(ampersand_index), "@1-"))
                                            new_signal.endian = 2;
                                        else
                                            new_signal.endian = 3;

                                        database_file.message_list[database_file.num_messages - 1].signal_list.Add(new_signal);
                                        database_file.message_list[database_file.num_messages - 1].num_signals++;

                                        //MessageBox.Show(curr_signal_line + "\t\n" + new_signal.name + " " + new_signal.start_bit + " " + new_signal.length + " " + new_signal.scale + " " + new_signal.offset + " " + new_signal.min + " " + new_signal.max + " " + new_signal.unit + " " + new_signal.endian);

                                    } while ((char)reader.Peek() == ' ');
                                }
                            }
                        }
                    }

                    /* Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                    */
                }
            }
        }
    }
}
