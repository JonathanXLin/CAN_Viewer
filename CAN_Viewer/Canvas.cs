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
    public class Canvas
    {
        public double time_start; // Time at start of canvas
        public double time_end; // Time at end of canvas, can be same as start time

        List<Label> time_tickmarkers;

        //public List<Canvas_Signal_t> signal_list;

        public Canvas()
        {
            time_start = 0.0;
            time_end = 0.0;

            time_tickmarkers = new List<Label>();

            //signal_list = new List<Canvas_Signal_t>();
        }
        public Canvas(double time_start_initial, double time_end_initial)
        {
            if (time_start_initial <= time_end_initial) // Initial parameters are good
            {
                time_start = time_start_initial;
                time_end = time_end_initial;
            }
            else // Initial parameters are bad, revert to window from -1s to 9s
            {
                time_start = -1.0;
                time_end = 9.0;
            }

            time_tickmarkers = new List<Label>();
        }
        public void update_time_tickmarks(Panel canvas_)
        {
            // Delete all tickmarkers and clear list
            foreach (Label label_to_remove in time_tickmarkers)
                canvas_.Controls.Remove(label_to_remove);
            time_tickmarkers.Clear();

            // Populate list with new tickmarkers
            for (int i = 0; i < 10; i++)
            {
                Label new_label = new Label();
                new_label.Text = "test";
                new_label.ForeColor = Color.White;
                new_label.Location = new Point(Convert.ToInt32(((i + 1) / 10.0) * canvas_.Width) - (new_label.Width / 2), canvas_.Height - 20);
                new_label.AutoSize = true;
                //MessageBox.Show(tickmark[i].Location.X.ToString());
                canvas_.Controls.Add(new_label);

                time_tickmarkers.Add(new_label);
            }
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
