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
        public List<ChartArea> chart_areas;

        public Tuple<double, double> timeslice;

        public Chart_GUI(Chart chart_)
        {
            chart = chart_;
            chart.BackColor = Color.FromArgb(77, 77, 77); // Set chart color to white

            chart_areas = new List<ChartArea>();

            timeslice = new Tuple<double, double>(-1.0, 9.0); // Initialize timeslice to be -1.0 to 9.0 until logfile is displayed

            // Initially create one chart area, will be deleted and replaced with real data when logfile is parsed
            chart_areas.Add(new ChartArea("default"));
            chart.ChartAreas.Add(chart_areas[0]);

            stylize_chart_area(chart_areas[0]);
        }
        public int stylize_chart_area(ChartArea chart_area_)
        {
            chart_area_.BackColor = Color.Black; // Set chart_area to black

            chart_area_.AxisX.Enabled = AxisEnabled.True;
            chart_area_.AxisX.LineColor = Color.White;
            chart_area_.AxisX.InterlacedColor = Color.White;
            chart_area_.AxisX.MajorGrid.LineColor = Color.FromArgb(77, 77, 77);
            chart_area_.AxisX.LabelStyle.ForeColor = Color.White;

            chart_area_.AxisX.Minimum = timeslice.Item1;
            chart_area_.AxisX.Maximum = timeslice.Item2;
            chart_area_.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;

            return 1; // Not yet used
        }
    }
}
