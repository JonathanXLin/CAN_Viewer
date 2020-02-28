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
        // List of signals that are contained in current timeslice
        public List<Tuple<ChartArea, Series>> signals;
        // Logfile being displayed
        public Logfile logfile;
        // CheckedListBox of unique signals that are to be displayed, updated in constructor and update_timeslice_data
        public CheckedListBox checked_list_box;
        // Current timeslice to be displayed, arbitrarily set in constructor and updated in update_timeslice_data
        public Timeslice timeslice;

        // NiceScale object whose methods are used to configure x axes to look nice
        public NiceScale nicescale; // NiceScale instance to configure nice looking axis values

        public Chart_GUI(Chart chart_, Logfile logfile_, CheckedListBox checked_list_box_)
        {
            chart = chart_;
            chart.BackColor = Color.FromArgb(77, 77, 77); // Set chart color to white

            logfile = logfile_;

            checked_list_box = checked_list_box_;

            signals = new List<Tuple<ChartArea, Series>>();

            timeslice.start = -1.0; // Initialize timeslice to be -1.0 to 9.0 until logfile is displayed
            timeslice.end = 9.0;

            nicescale = new NiceScale(this); // Instantiate nicescale object

            // Initially create one signal area, will be deleted and replaced with real data when logfile is parsed
            ChartArea default_chart_area = new ChartArea("default");

            Series default_series = new Series();
            for (int i = 0; i < 5; i++)
                default_series.Points.AddXY(i + 1, i + 1);

            add_signal_from_series(default_chart_area, default_series);
        }
        public int update_logfile(Logfile logfile_)
        {
            if (logfile_ == null)
                return 0;
            else
            {
                logfile = logfile_;
                return 1;
            }
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

            update_timeslice_data(timeslice, checked_list_box);
            //MessageBox.Show(timeslice.start.ToString() + " " + timeslice.end.ToString());
        }
        // Sets initial timeslice to be width of all data with 10% padding on each side
        public int set_initial_timeslice_data(CheckedListBox checked_list_box_initial)
        {
            // Set initial gui window to entire logfile timeslice, with some padding
            if (logfile.num_points != 0)
            {
                double initial_time_start = logfile.point_list[0].timestamp - 0.1 * (logfile.point_list[logfile.num_points - 1].timestamp - logfile.point_list[0].timestamp);
                double initial_time_end = logfile.point_list[logfile.num_points - 1].timestamp + 0.1 * (logfile.point_list[logfile.num_points - 1].timestamp - logfile.point_list[0].timestamp);

                Timeslice initial_timeslice = new Timeslice { start = initial_time_start, end = initial_time_end };

                update_timeslice_data(initial_timeslice, checked_list_box_initial);
            }
            else
                MessageBox.Show("Logfile empty");

            return 1; // Not yet used
        }
        public int update_timeslice_data(Timeslice timeslice_new, CheckedListBox checked_list_box_new)
        {
            if (logfile == null)
                return 0;

            // Clear chart, will be repopulated later
            chart.ChartAreas.Clear();
            chart.Series.Clear();

            // Clear existing signal list, repopulated later
            signals.Clear();

            // Update timeslice and checked_list_box
            timeslice = timeslice_new;
            checked_list_box = checked_list_box_new;

            // Create list of ChartAreas and Series which will be populated later, once they are populated, they are iteratively added to series with add_signal_from_series()
            List<ChartArea> chart_areas = new List<ChartArea>();
            List<Series> series = new List<Series>();

            // Populate list of ChartAreas and Series with all signals selected in checked_list_box, which are arranged alphabetically by default
            for (int i = 0; i < checked_list_box.CheckedItems.Count; i++)
            {
                ChartArea new_chart_area = new ChartArea(checked_list_box.CheckedItems[i].ToString());
                Series new_series = new Series(checked_list_box.CheckedItems[i].ToString());

                chart_areas.Add(new_chart_area);
                series.Add(new_series);

                //MessageBox.Show(checked_list_box.CheckedItems[i].ToString());
            }

            // Find start and end indices of set of logfile points that are encompassed by timeslice_new
            int start_index = logfile.point_list.IndexOf(logfile.point_list.Find(x => x.timestamp >= timeslice.start)); // Index of first signal greater than timeslice start
            int end_index = logfile.point_list.IndexOf(logfile.point_list.FindLast(x => x.timestamp <= timeslice.end)); // Index of last signal less than timeslice end

            //MessageBox.Show(start_index.ToString() + " " + end_index.ToString() + " | " + "0" + " " + (logfile.point_list.Count - 1).ToString());

            // Create sublist of points from list of points in logfile
            List<Logfile_Point> timeslice_points;
            if (start_index != -1 && end_index != -1 && end_index > start_index)
                timeslice_points = logfile.point_list.GetRange(start_index, end_index - start_index);
            else
                timeslice_points = new List<Logfile_Point>();

            // At each Logfile_Point within timeslice, for each of its signals, add the data to the series of that signal
            foreach (Logfile_Point current_point in timeslice_points)
            {
                foreach (Logfile_Signal_Point current_signal_point in current_point.signal_point_list)
                {
                    int current_signal_name_index_in_checked_list_box = checked_list_box.Items.IndexOf(current_signal_point.name);

                    // If current signal's box is checked
                    if (checked_list_box.GetItemChecked(current_signal_name_index_in_checked_list_box) == true)
                    {
                        Series series_of_found_signal_data = series.Find(series_of_signal => Convert.ToBoolean(string.Compare(series_of_signal.Name, current_signal_point.name)));

                        if (series_of_found_signal_data != null)
                            series_of_found_signal_data.Points.Add(current_point.timestamp, current_signal_point.value);
                    }
                }
            }

            // Add now-populated chart_areas and series lists to signals list
            if (chart_areas.Count != series.Count)
                throw new ArgumentException("size of chart_area and series not same");

            for (int i = 0; i < chart_areas.Count; i++)
                add_signal_from_series(chart_areas[i], series[i]);

            // Refresh chart
            chart.Refresh();

            return 1; // Not yet used
        }
        // Adds new chart area and series to signals list and chart object
        public int add_signal_from_series(ChartArea chart_area_, Series series_)
        {
            chart_area_.BackColor = Color.Black; // Set chart_area to black

            // X axis stylize
            chart_area_.AxisX.Enabled = AxisEnabled.True;
            chart_area_.AxisX.LineColor = Color.White;
            chart_area_.AxisX.InterlacedColor = Color.White;
            chart_area_.AxisX.MajorGrid.LineColor = Color.FromArgb(77, 77, 77);
            chart_area_.AxisX.LabelStyle.ForeColor = Color.White;

            // Update each chart area to new timeslice
            foreach (ChartArea current_chart_area in chart.ChartAreas)
            {
                nicescale.set_parameters(this);

                current_chart_area.AxisX.Minimum = nicescale.get_nice_min();
                current_chart_area.AxisX.Maximum = nicescale.get_nice_max();
                current_chart_area.AxisX.Interval = nicescale.get_tick_spacing();
            }

            chart_area_.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;

            // Y axis stylize
            chart_area_.AxisY.Enabled = AxisEnabled.True;
            chart_area_.AxisY.LineColor = Color.White;
            chart_area_.AxisY.InterlacedColor = Color.White;
            chart_area_.AxisY.MajorGrid.LineColor = Color.FromArgb(77, 77, 77);
            chart_area_.AxisY.LabelStyle.ForeColor = Color.White;

            chart_area_.AxisY.Minimum = double.NaN; // Autoscales y axis
            chart_area_.AxisY.Maximum = double.NaN;
            chart_area_.AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;

            // Series stylize
            series_.Enabled = true;
            series_.ChartArea = chart_area_.Name;

            series_.ChartType = SeriesChartType.Line;
            series_.MarkerStyle = MarkerStyle.Square;
            series_.MarkerColor = Color.White;
            series_.Color = Color.White;
            series_.LabelBackColor = Color.White;

            signals.Add(new Tuple<ChartArea, Series>(chart_area_, series_));
            chart.ChartAreas.Add(chart_area_);
            chart.Series.Add(series_);

            return 1; // Not yet used
        }
        public int initialize_mouse_wheel_event(Form form_)
        {
            form_.MouseWheel += new MouseEventHandler(mouse_wheel_event);

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

        public NiceScale(Chart_GUI chart_gui_)
        {
            timeslice = chart_gui_.timeslice;
            calculate(chart_gui_);
        }

        private void calculate(Chart_GUI chart_gui_)
        {
            range = nice_num(timeslice.end - timeslice.start, false);
            tickSpacing = nice_num(range / (maxTicks - 1), true);
            niceMin = Math.Floor(timeslice.start / tickSpacing) * tickSpacing;
            niceMax = Math.Ceiling(timeslice.end / tickSpacing) * tickSpacing;

            maxTicks = Convert.ToInt32(chart_gui_.chart.ChartAreas[0].Position.Width) / 5; // Max number of ticks is one tick every 5 distance
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
