namespace CAN_Viewer
{
    partial class Logfile_Parser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Logfile_Parser));
            this.label_fileName = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.label_Action = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_fileName
            // 
            this.label_fileName.AutoSize = true;
            this.label_fileName.Location = new System.Drawing.Point(9, 22);
            this.label_fileName.Name = "label_fileName";
            this.label_fileName.Size = new System.Drawing.Size(60, 13);
            this.label_fileName.TabIndex = 0;
            this.label_fileName.Text = "[File Name]";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 49);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(384, 23);
            this.progressBar.TabIndex = 1;
            // 
            // button_Cancel
            // 
            this.button_Cancel.Location = new System.Drawing.Point(321, 89);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 2;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // label_Action
            // 
            this.label_Action.AutoSize = true;
            this.label_Action.Location = new System.Drawing.Point(336, 22);
            this.label_Action.Name = "label_Action";
            this.label_Action.Size = new System.Drawing.Size(43, 13);
            this.label_Action.TabIndex = 3;
            this.label_Action.Text = "[Action]";
            // 
            // Logfile_Parser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 124);
            this.Controls.Add(this.label_Action);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label_fileName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Logfile_Parser";
            this.Text = "Logfile_Parser";
            this.Load += new System.EventHandler(this.Logfile_Parser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_fileName;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Label label_Action;
    }
}