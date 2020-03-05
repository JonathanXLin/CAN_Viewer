namespace CAN_Viewer
{
    partial class Loading_Bar_Parser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Loading_Bar_Parser));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.label_FileName = new System.Windows.Forms.Label();
            this.label_Status = new System.Windows.Forms.Label();
            this.parser = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 49);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(382, 23);
            this.progressBar.TabIndex = 0;
            // 
            // button_Cancel
            // 
            this.button_Cancel.Location = new System.Drawing.Point(319, 81);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 1;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // label_FileName
            // 
            this.label_FileName.AutoSize = true;
            this.label_FileName.Location = new System.Drawing.Point(12, 25);
            this.label_FileName.Name = "label_FileName";
            this.label_FileName.Size = new System.Drawing.Size(60, 13);
            this.label_FileName.TabIndex = 2;
            this.label_FileName.Text = "[File Name]";
            // 
            // label_Status
            // 
            this.label_Status.AutoSize = true;
            this.label_Status.Location = new System.Drawing.Point(351, 25);
            this.label_Status.Name = "label_Status";
            this.label_Status.Size = new System.Drawing.Size(43, 13);
            this.label_Status.TabIndex = 3;
            this.label_Status.Text = "[Status]";
            // 
            // parser
            // 
            this.parser.WorkerReportsProgress = true;
            this.parser.WorkerSupportsCancellation = true;
            this.parser.DoWork += new System.ComponentModel.DoWorkEventHandler(this.parser_DoWork);
            this.parser.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.parser_ProgressChanged);
            this.parser.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.parser_RunWorkerCompleted);
            // 
            // Loading_Bar_Parser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 116);
            this.Controls.Add(this.label_Status);
            this.Controls.Add(this.label_FileName);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.progressBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Loading_Bar_Parser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Working...";
            this.Load += new System.EventHandler(this.Loading_Bar_Worker_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Label label_FileName;
        private System.Windows.Forms.Label label_Status;
        private System.ComponentModel.BackgroundWorker parser;
    }
}