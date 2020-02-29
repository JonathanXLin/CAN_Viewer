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
    public class Database_Set
    {
        public List<Database> databases;

        public Database_Set()
        {
            databases = new List<Database>();
        }
    }

    // Database file, which contains message formats
    public class Database
    {
        public string path;

        public List<Message> message_list;
        public int num_messages;

        public Database()
        {
            message_list = new List<Message>();
        }

        public int parse(string file_path)
        {
            // Populate database_file object
            path = file_path;
            num_messages = 0;

            /* Opens file in notepad */
            //Process.Start("notepad.exe", path);

            // Populates database_file object with database data
            using (StreamReader reader = new StreamReader(path))
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
                        Message new_message = new Message();
                        new_message.name = message_line_words[2]; 
                        new_message.id = Int64.Parse(message_line_words[1]) - (long)(8 * Math.Pow(16, 7)); // Last digit in hex is 8 too big for some reason
                        new_message.length = Int32.Parse(message_line_words[3].Remove(0, 1));
                        new_message.sender = message_line_words[4];
                        message_list.Add(new_message);

                        //MessageBox.Show(message_list[num_messages].name);

                        // Declare instance of signal list and initializes length
                        message_list[num_messages].num_signals = 0;

                        // Increment number of messages in message_list
                        num_messages++;

                        // Continue reading while still parsing new signal formats
                        if ((char)reader.Peek() == ' ') // If next character is space, according to .dbc format, means there is at least one signal format
                        {
                            do
                            {
                                string curr_signal_line = reader.ReadLine();
                                string[] signal_line_words = curr_signal_line.Split(' ');

                                // Parse signal line and populate signal with new item
                                Signal new_signal = new Signal();
                                new_signal.name = signal_line_words[2];

                                //if (!Convert.ToBoolean(string.Compare(message_line_words[2], "HOURS")))
                                //{
                                //    MessageBox.Show(curr_signal_line + " |||||| " + new_signal.name);
                                //}

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

                                message_list[num_messages - 1].signal_list.Add(new_signal);
                                message_list[num_messages - 1].num_signals++;

                                //MessageBox.Show(curr_signal_line + "\n\n" + new_signal.name + " " + new_signal.start_bit + " " + new_signal.length + " " + new_signal.scale + " " + new_signal.offset + " " + new_signal.min + " " + new_signal.max + " " + new_signal.unit + " " + new_signal.endian);

                            } while ((char)reader.Peek() == ' ');
                        }
                    }
                }
            }

            return 1;// Not used, but should return 1 if success
        }
    }

    // Database file message format, which contains signal formats
    public class Message
    {
        public string name;
        public long id;
        public int length; // Bytes
        public string sender;

        public List<Signal> signal_list;
        public int num_signals;

        public Message()
        {
            signal_list = new List<Signal>();
        }
    }

    // Endian enumeration for DBC signal format, not yet used
    //enum Endian { little_endian, big_endian, unknown };

    // Database file signal format
    public class Signal
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
}
