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
    public class Chart_GUI
    {
        // Chart instance being worked on
        public Chart chart;
        // List of complete series in entire logfile
        public List<Series> series;
        // Logfile being displayed
        public Logfile logfile;
        // Database set being used
        public Database_Set database_set;
        // CheckedListBox of unique signals that are to be displayed, updated in constructor and update_timeslice_data
        public CheckedListBox checked_list_box;
        // Current timeslice to be displayed, arbitrarily set in constructor and updated in update_timeslice_data
        public Timeslice timeslice;

        // Chart area width, instantiated in constructor updated on form_resize_event
        public int chart_area_width;

        // NiceScale object whose methods are used to configure x axes to look nice
        public NiceScale nicescale; // NiceScale instance to configure nice looking axis values

        public Chart_GUI(Chart chart_, CheckedListBox checked_list_box_)
        {
            chart = chart_;
            chart.BackColor = Color.FromArgb(77, 77, 77); // Set chart color to white

            series = new List<Series>();

            logfile = new Logfile();
            database_set = new Database_Set();

            checked_list_box = checked_list_box_;

            timeslice.start = -1.0; // Initialize timeslice to be -1.0 to 9.0 until logfile is displayed
            timeslice.end = 9.0;

            chart_area_width = Convert.ToInt32(chart.ChartAreas[0].Position.Width);

            nicescale = new NiceScale(this); // Instantiate nicescale object
        }
        public int update_logfile(CheckedListBox checked_list_box_)
        {
            if (logfile == null || database_set == null)
                return 0;
            else
            {
                checked_list_box = checked_list_box_;

                // Populate list of series, one series for each unique signal
                foreach (string unique_signal_name in logfile.unique_signals)
                {
                    Series new_series = new Series(unique_signal_name);
                    //MessageBox.Show(new_series.Name);

                    // Stylize new series
                    new_series.Enabled = true;
                    new_series.ChartArea = ""; // Set to zero length string for it to not be plotted

                    new_series.ChartType = SeriesChartType.Line;
                    new_series.MarkerStyle = MarkerStyle.Square;
                    new_series.MarkerColor = Color.White;
                    new_series.Color = Color.White;
                    new_series.LabelBackColor = Color.White;

                    series.Add(new_series);
                }

                // For every signal in every message, add the data to that signal's series
                foreach (Logfile_Point point in logfile.point_list)
                {
                    foreach (Logfile_Signal_Point signal_point in point.signal_point_list)
                    {
                        series.Find(x => !Convert.ToBoolean(string.Compare(signal_point.name, x.Name))).Points.AddXY(point.timestamp, signal_point.value);
                    }
                }

                // Set timeslice initially to max width of logfile
                Timeslice max_timeslice;
                max_timeslice.start = 0;
                max_timeslice.end = Convert.ToInt32(logfile.point_list[logfile.point_list.Count - 1].timestamp * 1.1);

                update_timeslice_data(max_timeslice);
                logfile.update_CheckedListBox(checked_list_box);

                return 1;
            }
        }
        // Same as function below except timeslice is set to entire logfile width plus some padding
        public int set_initial_timeslice_data()
        {
            if (logfile == null)
                return 0;

            // Set initial gui window to entire logfile timeslice, with some padding
            if (logfile.num_points != 0)
            {
                double initial_time_start = logfile.point_list[0].timestamp - 0.1 * (logfile.point_list[logfile.num_points - 1].timestamp - logfile.point_list[0].timestamp);
                double initial_time_end = logfile.point_list[logfile.num_points - 1].timestamp + 0.1 * (logfile.point_list[logfile.num_points - 1].timestamp - logfile.point_list[0].timestamp);

                Timeslice initial_timeslice = new Timeslice { start = initial_time_start, end = initial_time_end };

                update_timeslice_data(initial_timeslice);
            }
            else
                MessageBox.Show("Logfile empty");

            return 1; // Not yet used
        }
        // Updates chart areas to show data in new timeslice
        public int update_timeslice_data(Timeslice timeslice_new)
        {
            if (logfile == null)
                return 0;

            //// Clear chart, will be repopulated later
            //chart.ChartAreas.Clear();

            // Update timeslice and checked_list_box
            timeslice = timeslice_new;

            //// Create list of ChartAreas which will be populated later
            //List<ChartArea> chart_areas = new List<ChartArea>();

            //// Populate list of ChartAreas with all signals selected in checked_list_box, which are arranged alphabetically by default
            //for (int i = 0; i < checked_list_box.CheckedItems.Count; i++)
            //{
            //    ChartArea new_chart_area = new ChartArea(checked_list_box.CheckedItems[i].ToString());

            //    chart_areas.Add(new_chart_area);
            //}

            //// Find start and end indices of set of logfile points that are encompassed by timeslice_new
            //int start_index = logfile.point_list.IndexOf(logfile.point_list.Find(x => x.timestamp >= timeslice.start)); // Index of first signal greater than timeslice start
            //int end_index = logfile.point_list.IndexOf(logfile.point_list.FindLast(x => x.timestamp <= timeslice.end)); // Index of last signal less than timeslice end

            //MessageBox.Show(start_index.ToString() + " " + end_index.ToString() + " | " + "0" + " " + (logfile.point_list.Count - 1).ToString());

            //// Create sublist of points from list of points in logfile
            //List<Logfile_Point> timeslice_points;
            //if (start_index != -1 && end_index != -1 && end_index > start_index)
            //    timeslice_points = logfile.point_list.GetRange(start_index, end_index - start_index);
            //else
            //    timeslice_points = new List<Logfile_Point>();

            //// At each Logfile_Point within timeslice, for each of its signals, add the data to the series of that signal
            //foreach (Logfile_Point current_point in timeslice_points)
            //{
            //    foreach (Logfile_Signal_Point current_signal_point in current_point.signal_point_list)
            //    {
            //        int current_signal_name_index_in_checked_list_box = checked_list_box.Items.IndexOf(current_signal_point.name);

            //        // If current signal's box is checked
            //        if (checked_list_box.GetItemChecked(current_signal_name_index_in_checked_list_box) == true)
            //        {
            //            if (series.Find(series_of_signal => !Convert.ToBoolean(string.Compare(series_of_signal.Name, current_signal_point.name))) != null)
            //            {
            //                series.Find(series_of_signal => !Convert.ToBoolean(string.Compare(series_of_signal.Name, current_signal_point.name))).Points.AddXY(current_point.timestamp, current_signal_point.value);
            //                //MessageBox.Show(current_point.timestamp.ToString() + " " + current_signal_point.value.ToString());
            //            }
            //        }
            //    }
            //}

            //// Add now-populated chart_areas and series lists to signals list
            //if (chart_areas.Count != series.Count)
            //    throw new ArgumentException("size of chart_area and series not same");

            //for (int i = 0; i < chart_areas.Count; i++)
            //    add_signal_from_series(chart_areas[i], series[i]);

            //for (int i = 0; i < chart_areas.Count; i++)
            //{
            //    chart_areas[i].BackColor = Color.Black; // Set chart_area to black

            //    // X axis stylize
            //    chart_areas[i].AxisX.Enabled = AxisEnabled.True;
            //    chart_areas[i].AxisX.LineColor = Color.White;
            //    chart_areas[i].AxisX.InterlacedColor = Color.White;
            //    chart_areas[i].AxisX.MajorGrid.LineColor = Color.FromArgb(77, 77, 77);
            //    chart_areas[i].AxisX.LabelStyle.ForeColor = Color.White;

            //    // Find nice x axis parameters, need to do this after refresh because need new chart area width
            //    nicescale.set_parameters(this);

            //    chart_areas[i].AxisX.Minimum = nicescale.get_nice_min();
            //    chart_areas[i].AxisX.Maximum = nicescale.get_nice_max();
            //    chart_areas[i].AxisX.Interval = nicescale.get_tick_spacing();

            //    // Y axis stylize
            //    chart_areas[i].AxisY.Enabled = AxisEnabled.True;
            //    chart_areas[i].AxisY.LineColor = Color.White;
            //    chart_areas[i].AxisY.InterlacedColor = Color.White;
            //    chart_areas[i].AxisY.MajorGrid.LineColor = Color.FromArgb(77, 77, 77);
            //    chart_areas[i].AxisY.LabelStyle.ForeColor = Color.White;

            //    chart_areas[i].AxisY.Minimum = double.NaN; // Autoscales y axis
            //    chart_areas[i].AxisY.Maximum = double.NaN;
            //    chart_areas[i].AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;

            //    chart.ChartAreas.Add(chart_areas[i]);
            //}

            // Find nice x axis parameters, need to do this after refresh because need new chart area width
            nicescale.set_parameters(this);

            foreach (ChartArea chart_area in chart.ChartAreas)
            {
                chart_area.AxisX.Minimum = nicescale.get_nice_min();
                chart_area.AxisX.Maximum = nicescale.get_nice_max();
                chart_area.AxisX.Interval = nicescale.get_tick_spacing();
            }

            // Refresh chart
            chart.Refresh();

            return 1; // Not yet used
        }
        public void update_displayed_chart_areas(List<string> checked_items)
        {
            // Clear all existing chart areas, all series' chart areas, and series in chart
            chart.ChartAreas.Clear();
            foreach (Series current_series in series)
                current_series.ChartArea = "";
            chart.Series.Clear();

            foreach (string checked_signal_name in checked_items)
            {
                // Declare new chart area for each signal checked and add to chart
                ChartArea chart_area = new ChartArea();
                chart_area.Name = checked_signal_name;
                chart_area.BackColor = Color.Black; // Set chart_area to black

                // X axis stylize
                chart_area.AxisX.Enabled = AxisEnabled.True;
                chart_area.AxisX.LineColor = Color.White;
                chart_area.AxisX.InterlacedColor = Color.White;
                chart_area.AxisX.MajorGrid.LineColor = Color.FromArgb(77, 77, 77);
                chart_area.AxisX.LabelStyle.ForeColor = Color.White;

                // Find nice x axis parameters, need to do this after refresh because need new chart area width
                nicescale.set_parameters(this);

                chart_area.AxisX.Minimum = nicescale.get_nice_min();
                chart_area.AxisX.Maximum = nicescale.get_nice_max();
                chart_area.AxisX.Interval = nicescale.get_tick_spacing();

                // Y axis stylize
                chart_area.AxisY.Enabled = AxisEnabled.True;
                chart_area.AxisY.LineColor = Color.White;
                chart_area.AxisY.InterlacedColor = Color.White;
                chart_area.AxisY.MajorGrid.LineColor = Color.FromArgb(77, 77, 77);
                chart_area.AxisY.LabelStyle.ForeColor = Color.White;

                chart_area.AxisY.Minimum = double.NaN; // Autoscales y axis
                chart_area.AxisY.Maximum = double.NaN;
                chart_area.AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;

                // Add chart area to chart
                chart.ChartAreas.Add(chart_area);

                // Assign chart area to series of signal checked
                int series_index = series.IndexOf(series.Find(x => !Convert.ToBoolean(string.Compare(x.Name, checked_signal_name))));

                if (series_index != -1)
                {
                    // Stylize new series
                    series[series_index].Enabled = true;
                    series[series_index].ChartArea = ""; // Set to zero length string for it to not be plotted

                    series[series_index].ChartType = SeriesChartType.Line;
                    series[series_index].MarkerStyle = MarkerStyle.Square;
                    series[series_index].MarkerColor = Color.White;
                    series[series_index].Color = Color.White;
                    series[series_index].LabelBackColor = Color.White;

                    // Add chart area to series
                    series[series_index].ChartArea = chart_area.Name;

                    // Add series to chart
                    chart.Series.Add(series[series_index]);
                }
            }

            // Refresh chart to display changes
            chart.Refresh();
        }
        public void mouse_wheel_event(object sender, MouseEventArgs e)
        {
            // Timeslice bounds increased/decreased by 10% of edge of gui window to current mouse position when middle mouse wheel is moved
            double timeslice_width = timeslice.end - timeslice.start;
            double left_bound_adjustment_weight = (e.X - (chart.Location.X)) / Convert.ToDouble(chart.Width);
            double right_bound_adjustment_weight = 1 - left_bound_adjustment_weight;

            // Zoom in
            if (e.Delta > 0)
            {
                // Update timeslice
                timeslice.start += timeslice_width * 0.1 * left_bound_adjustment_weight;
                timeslice.end -= timeslice_width * 0.1 * right_bound_adjustment_weight;
            }
            // Zoom out
            else if (e.Delta < 0)
            {
                // Update timeslice
                timeslice.start -= timeslice_width * 0.1 * left_bound_adjustment_weight;
                timeslice.end += timeslice_width * 0.1 * right_bound_adjustment_weight;
            }

            update_timeslice_data(timeslice);
            //MessageBox.Show(timeslice.start.ToString() + " " + timeslice.end.ToString());
        }
        // Sets initial timeslice to be width of all data with 10% padding on each side
        public int initialize_mouse_wheel_event(Form form_)
        {
            form_.MouseWheel += new MouseEventHandler(mouse_wheel_event);

            return 1; // Not yet used
        }
        public void form_resize_event(object sender, EventArgs e)
        {
            // Update chart area width
            if (chart.ChartAreas.Count != 0)
                chart_area_width = Convert.ToInt32(chart.ChartAreas[0].Position.Width);

            // Explicitly update timeslice data, which only changes x axis nice parameters
            update_timeslice_data(timeslice);
        }
        public int initialize_form_resize_event(Form form_)
        {
            form_.Resize += new EventHandler(form_resize_event);

            return 1; // Not yet used
        }
    }

    // Timeslice struct
    public struct Timeslice
    {
        public double start;
        public double end;
    }

    // Adapted from principles described in "Graphics Gems, Volume 1" by Andrew S. Glassner
    public class NiceScale
    {
        private Timeslice timeslice;
        private int maxTicks;
        private double tickSpacing;
        private double range;
        private double niceMin;
        private double niceMax;

        public NiceScale(Chart_GUI chart_gui_) // If there is a chart area to nice scale
        {
            if (chart_gui_.chart.ChartAreas.Count != 0)
            {
                set_parameters(chart_gui_);
            }
        }

        private void calculate(Chart_GUI chart_gui_)
        {
            range = nice_num(timeslice.end - timeslice.start, false);
            tickSpacing = nice_num(range / (maxTicks - 1), true);
            niceMin = Math.Floor(timeslice.start / tickSpacing) * tickSpacing;
            niceMax = Math.Ceiling(timeslice.end / tickSpacing) * tickSpacing;
        }

        private double nice_num(double range, bool round)
        {
            double exponent;
            double fraction;
            double niceFraction;

            exponent = Math.Floor(Math.Log10(range));
            fraction = range / Math.Pow(10, exponent);

            if (round)
            {
                if (fraction < 1.5)
                    niceFraction = 1;
                else if (fraction < 3)
                    niceFraction = 2;
                else if (fraction < 7)
                    niceFraction = 5;
                else
                    niceFraction = 10;
            }
            else
            {
                if (fraction <= 1)
                    niceFraction = 1;
                else if (fraction <= 2)
                    niceFraction = 2;
                else if (fraction <= 5)
                    niceFraction = 5;
                else
                    niceFraction = 10;
            }

            return niceFraction * Math.Pow(10, exponent);
        }

        public void set_parameters(Chart_GUI chart_gui_)
        {
            timeslice = chart_gui_.timeslice;
            maxTicks = chart_gui_.chart_area_width / 5; // Max number of ticks is one tick every 5 distance

            calculate(chart_gui_);
        }

        public double get_nice_min()
        {
            return niceMin;
        }

        public double get_nice_max()
        {
            return niceMax;
        }

        public double get_tick_spacing()
        {
            return tickSpacing;
        }
    }
}
