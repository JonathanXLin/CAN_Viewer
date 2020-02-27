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
    class Chart_GUI
    {
        public Chart chart;
        public List<Tuple<ChartArea, Series>> signals;

        public Tuple<double, double> timeslice;

        public Chart_GUI(Chart chart_)
        {
            chart = chart_;
            chart.BackColor = Color.FromArgb(77, 77, 77); // Set chart color to white

            signals = new List<Tuple<ChartArea, Series>();

            timeslice = new Tuple<double, double>(-1.0, 9.0); // Initialize timeslice to be -1.0 to 9.0 until logfile is displayed

            // Initially create one signal area, will be deleted and replaced with real data when logfile is parsed
            signals.Add(new Tuple<ChartArea, Series>(new ChartArea("default"), new Series());
            chart.ChartAreas.Add(chart_areas[0]);

            stylize_chart_area(chart_areas[0]);
        }
        // Adds new chart area and series to signals list and chart object
        public int add_signal(ChartArea chartArea_, Series series_)
        {
            signals.Add(new Tuple<ChartArea, Series>(chartArea_, series_));
            chart.ChartAreas.Add(chartArea_);
            chart.Series.Add(series_);

            return 1; // Not yet used
        }
        public int stylize_chart_area(ChartArea chart_area_)
        {
            chart_area_.BackColor = Color.Black; // Set chart_area to black

            // X axis stylize
            chart_area_.AxisX.Enabled = AxisEnabled.True;
            chart_area_.AxisX.LineColor = Color.White;
            chart_area_.AxisX.InterlacedColor = Color.White;
            chart_area_.AxisX.MajorGrid.LineColor = Color.FromArgb(77, 77, 77);
            chart_area_.AxisX.LabelStyle.ForeColor = Color.White;

            chart_area_.AxisX.Minimum = timeslice.Item1;
            chart_area_.AxisX.Maximum = timeslice.Item2;
            chart_area_.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;

            // Y axis stylize
            chart_area_.AxisY.Enabled = AxisEnabled.True;
            chart_area_.AxisY.LineColor = Color.White;
            chart_area_.AxisY.InterlacedColor = Color.White;
            chart_area_.AxisY.MajorGrid.LineColor = Color.FromArgb(77, 77, 77);
            chart_area_.AxisY.LabelStyle.ForeColor = Color.White;

            chart_area_.AxisY.Minimum = timeslice.Item1;
            chart_area_.AxisY.Maximum = timeslice.Item2;
            chart_area_.AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;

            return 1; // Not yet used
        }
    }
}
