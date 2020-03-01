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
    public class Logfile
    {
        public string path;

        public List<Logfile_Point> point_list;
        public int num_points; // Can find size of list from method in list, FLAG TO FIX MULTIPLE INSTANCES

        public List<string> unique_signals; // All signals stored only once here
        public int num_unique_signals;

        // Direction enumeration for logfile point direction
        //enum Direction { Rx, Tx, unknown };

        public Logfile()
        {
            point_list = new List<Logfile_Point>();
            unique_signals = new List<string>();
        }

        public int parse(string file_path, Database_Set database_set)
        {
            // Populate logfile object
            path = file_path;
            num_points = 0;
            num_unique_signals = 0;

            // Populates database_ object with database data
            using (StreamReader reader = new StreamReader(path))
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

                        foreach (Database current_database in database_set.databases)
                        {
                            if (current_database.message_list.IndexOf(current_database.message_list.Find(x => x.id == new_point.id)) != -1)
                            {
                                new_point.database_index = database_set.databases.IndexOf(current_database);
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
                            int num_signals = database_set.databases[new_point.database_index].message_list[new_point.database_message_index].num_signals; // Number of signals in current point

                            for (int i = 0; i < num_signals; i++)
                            {
                                // Calculate raw value using little endian
                                int start_bit = database_set.databases[new_point.database_index].message_list[new_point.database_message_index].signal_list[i].start_bit;
                                int num_bits = database_set.databases[new_point.database_index].message_list[new_point.database_message_index].signal_list[i].length;

                                long raw_value = 0; // Raw little-endian value of signal

                                for (int bit_array_index = start_bit; bit_array_index < start_bit + num_bits; bit_array_index++)
                                {
                                    raw_value += Convert.ToInt64(data_bit_array[bit_array_index]) * Convert.ToInt64(Math.Pow(2, bit_array_index - start_bit));
                                }
                                //MessageBox.Show("Logfile Message #: " + new_point.point_number + " Timestamp: " + new_point.timestamp + " Signal Value: " + raw_value);

                                // Apply scaling, and offset (note, endian is not considered because I don't know what it does)
                                double scale = database_set.databases[new_point.database_index].message_list[new_point.database_message_index].signal_list[i].scale;
                                double offset = database_set.databases[new_point.database_index].message_list[new_point.database_message_index].signal_list[i].offset;

                                double final_value = (raw_value * scale) + offset;

                                // Add new signal to signal list and store values in new signal
                                Logfile_Signal_Point new_signal_point = new Logfile_Signal_Point();
                                new_signal_point.name = database_set.databases[new_point.database_index].message_list[new_point.database_message_index].signal_list[i].name;
                                new_signal_point.value = final_value;
                                new_point.signal_point_list.Add(new_signal_point);
                                new_point.num_signal_points++;

                                //MessageBox.Show("Logfile Message #: " + new_point.point_number + " Timestamp: " + new_point.timestamp + " Signal Name <" + database_.message_list[new_point.database_message_index].signal_list[i].name + ">, value: " + final_value);

                                // Check if signal is new signal and if so, add to unique_signal list
                                if (unique_signals.Exists(x => x == new_signal_point.name) == false)
                                {
                                    unique_signals.Add(new_signal_point.name);
                                    num_unique_signals++;

                                    //MessageBox.Show("Added: " + new_signal_point.name);
                                }
                            }
                            //MessageBox.Show("Decoding logfile line number <" + new_point.point_number + "> with Timestamp <" + new_point.timestamp + ">");
                        }

                        // Adds logfile point to point list
                        point_list.Add(new_point);
                        num_points++;

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
                            MessageBox.Show("Timestamp: " + curr_point.timestamp.ToString() + " Message Name: " + database_.message_list[curr_point.database_message_index].name);
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
                }
                //MessageBox.Show(start_index.ToString() + " " + end_index.ToString());
            }

            // MessageBox.Show(point_list.Find(x => x.id == 419366480).timestamp.ToString());

            /* Opens file in notepad
            Process.Start("notepad.exe", logfile_path);
            */

            return 1;// Not used, but should return 1 if success
        }

        // Populate checkedListBox with all logfile signals
        public int update_CheckedListBox(CheckedListBox checkListBox)
        {
            unique_signals = unique_signals.OrderBy(name => name).ToList();
            foreach (string signal_name in unique_signals)
            {
                checkListBox.Items.Add(signal_name);
            }

            return 1;// Not used, but should return 1 if success
        }
    }

    // Logfile point, corresponds to single line in logfile
    public class Logfile_Point
    {
        public double timestamp;
        public int channel;
        public long id;
        public int direction;
        public int num_bytes;
        public int[] data;
        public int point_number;

        public int database_index; // Index of database that contains point's format, -1 if does not exist
        public int database_message_index; // Index of message in database that contains point's format, -1 if does not exist

        public List<Logfile_Signal_Point> signal_point_list; // List of signals that are contained in logfile point
        public int num_signal_points;

        public Logfile_Point()
        {
            signal_point_list = new List<Logfile_Signal_Point>();
        }
    }

    // Logfile signal, corresponds to single graphical datapoint on GUI
    public class Logfile_Signal_Point
    {
        public string name;
        public double value;
    }
}
