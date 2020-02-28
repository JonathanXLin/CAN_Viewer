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
        public Chart chart;
        public List<Tuple<ChartArea, Series>> signals;

        public Timeslice timeslice;

        public NiceScale nicescale; // NiceScale instance to configure nice looking axis values

        public Chart_GUI(Chart chart_)
        {
            chart = chart_;
            chart.BackColor = Color.FromArgb(77, 77, 77); // Set chart color to white

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
        public int update_timeslice_data(Logfile logfile_)
        {
            

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

            chart_area_.AxisX.Minimum = timeslice.start;
            chart_area_.AxisX.Maximum = timeslice.end;
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

                // Update each chart area to new timeslice
                foreach (ChartArea current_chart_area in chart.ChartAreas)
                {
                    nicescale.set_parameters(this);

                    current_chart_area.AxisX.Minimum = nicescale.get_nice_min();
                    current_chart_area.AxisX.Maximum = nicescale.get_nice_max();
                    current_chart_area.AxisX.Interval = nicescale.get_tick_spacing();
                }
            }
            // Zoom out
            else if (e.Delta < 0)
            {
                // Update timeslice
                timeslice.start -= timeslice_width * 0.1 * left_bound_adjustment_weight;
                timeslice.end += timeslice_width * 0.1 * right_bound_adjustment_weight;

                // Update each chart area to new timeslice
                foreach (ChartArea current_chart_area in chart.ChartAreas)
                {
                    nicescale.set_parameters(this);

                    current_chart_area.AxisX.Minimum = nicescale.get_nice_min();
                    current_chart_area.AxisX.Maximum = nicescale.get_nice_max();
                    current_chart_area.AxisX.Interval = nicescale.get_tick_spacing();
                }
            }

            //MessageBox.Show(timeslice.start.ToString() + " " + timeslice.end.ToString());
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
