namespace CAN_Viewer
{
    partial class CAN_Viewer_Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CAN_Viewer_Main));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.statusStrip_status = new System.Windows.Forms.StatusStrip();
            this.checkedListBox_signals = new System.Windows.Forms.CheckedListBox();
            this.menuStrip_menu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem_logfile = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem_database = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView_tree = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.menuStrip_menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip_status
            // 
            this.statusStrip_status.Location = new System.Drawing.Point(0, 538);
            this.statusStrip_status.Name = "statusStrip_status";
            this.statusStrip_status.Size = new System.Drawing.Size(984, 22);
            this.statusStrip_status.TabIndex = 0;
            this.statusStrip_status.Text = "statusStrip1";
            // 
            // checkedListBox_signals
            // 
            this.checkedListBox_signals.CheckOnClick = true;
            this.checkedListBox_signals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox_signals.FormattingEnabled = true;
            this.checkedListBox_signals.Location = new System.Drawing.Point(0, 0);
            this.checkedListBox_signals.Name = "checkedListBox_signals";
            this.checkedListBox_signals.Size = new System.Drawing.Size(230, 356);
            this.checkedListBox_signals.TabIndex = 1;
            this.checkedListBox_signals.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox_signals_ItemCheck);
            this.checkedListBox_signals.Click += new System.EventHandler(this.checkedListBox_signals_Click);
            // 
            // menuStrip_menu
            // 
            this.menuStrip_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip_menu.Location = new System.Drawing.Point(0, 0);
            this.menuStrip_menu.Name = "menuStrip_menu";
            this.menuStrip_menu.Size = new System.Drawing.Size(984, 24);
            this.menuStrip_menu.TabIndex = 2;
            this.menuStrip_menu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem_logfile,
            this.openToolStripMenuItem_database,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem_logfile
            // 
            this.openToolStripMenuItem_logfile.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem_logfile.Image")));
            this.openToolStripMenuItem_logfile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem_logfile.Name = "openToolStripMenuItem_logfile";
            this.openToolStripMenuItem_logfile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.openToolStripMenuItem_logfile.Size = new System.Drawing.Size(217, 22);
            this.openToolStripMenuItem_logfile.Text = "&Open CAN Logfile";
            this.openToolStripMenuItem_logfile.Click += new System.EventHandler(this.openToolStripMenuItem_logfile_Click);
            // 
            // openToolStripMenuItem_database
            // 
            this.openToolStripMenuItem_database.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem_database.Image")));
            this.openToolStripMenuItem_database.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem_database.Name = "openToolStripMenuItem_database";
            this.openToolStripMenuItem_database.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.openToolStripMenuItem_database.Size = new System.Drawing.Size(217, 22);
            this.openToolStripMenuItem_database.Text = "&Open Database File";
            this.openToolStripMenuItem_database.Click += new System.EventHandler(this.openToolStripMenuItem_database_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(214, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // treeView_tree
            // 
            this.treeView_tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_tree.Location = new System.Drawing.Point(0, 0);
            this.treeView_tree.Name = "treeView_tree";
            this.treeView_tree.Size = new System.Drawing.Size(230, 154);
            this.treeView_tree.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Left;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView_tree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.checkedListBox_signals);
            this.splitContainer1.Size = new System.Drawing.Size(230, 514);
            this.splitContainer1.SplitterDistance = 154;
            this.splitContainer1.TabIndex = 4;
            // 
            // chart
            // 
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.Location = new System.Drawing.Point(230, 24);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(754, 514);
            this.chart.TabIndex = 6;
            this.chart.Text = "chart1";
            this.chart.Click += new System.EventHandler(this.chart_Click);
            // 
            // CAN_Viewer_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 560);
            this.Controls.Add(this.chart);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip_status);
            this.Controls.Add(this.menuStrip_menu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "CAN_Viewer_Main";
            this.Text = "CAN Viewer v0.1";
            this.Load += new System.EventHandler(this.CAN_Viewer_Main_Load);
            this.menuStrip_menu.ResumeLayout(false);
            this.menuStrip_menu.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip_status;
        private System.Windows.Forms.CheckedListBox checkedListBox_signals;
        private System.Windows.Forms.MenuStrip menuStrip_menu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem_database;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem_logfile;
        private System.Windows.Forms.TreeView treeView_tree;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
    }
}

