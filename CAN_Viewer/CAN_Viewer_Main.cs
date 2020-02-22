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
    public partial class CAN_Viewer_Main : Form
    {
        // CAN file path
        private string logfile_path;

        // Database file path
        private string database_path;

        //Status strip label
        private ToolStripStatusLabel status_text = new ToolStripStatusLabel();

        public CAN_Viewer_Main()
        {
            InitializeComponent();
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
                openFileDialog.Filter = "txt files (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    database_path = openFileDialog.FileName;

                    // Update status bar with file name
                    status_text.Text = Path.GetFileName(database_path);

                    /* Opens file in notepad
                    Process.Start("notepad.exe", database_path);
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
    }
}
