using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int database_message_index; // Index of database message format, -1 if does not exist
        public List<Logfile_Signal_Point> signal_point_list; // List of signals that are contained in logfile point
        public int num_signal_points;
    }

    // Logfile signal, corresponds to single graphical datapoint on GUI
    public class Logfile_Signal_Point
    {
        public string name;
        public double value;
    }
}
