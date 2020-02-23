﻿namespace CAN_Viewer
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
            this.menuStrip_menu.SuspendLayout();
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
            this.checkedListBox_signals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox_signals.FormattingEnabled = true;
            this.checkedListBox_signals.Location = new System.Drawing.Point(8, 207);
            this.checkedListBox_signals.Name = "checkedListBox_signals";
            this.checkedListBox_signals.Size = new System.Drawing.Size(185, 319);
            this.checkedListBox_signals.TabIndex = 1;
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
            this.treeView_tree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView_tree.Location = new System.Drawing.Point(8, 30);
            this.treeView_tree.Name = "treeView_tree";
            this.treeView_tree.Size = new System.Drawing.Size(185, 168);
            this.treeView_tree.TabIndex = 3;
            // 
            // CAN_Viewer_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 560);
            this.Controls.Add(this.treeView_tree);
            this.Controls.Add(this.checkedListBox_signals);
            this.Controls.Add(this.statusStrip_status);
            this.Controls.Add(this.menuStrip_menu);
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "CAN_Viewer_Main";
            this.Text = "CAN Viewer v0.1";
            this.Load += new System.EventHandler(this.CAN_Viewer_Main_Load);
            this.menuStrip_menu.ResumeLayout(false);
            this.menuStrip_menu.PerformLayout();
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
    }
}

