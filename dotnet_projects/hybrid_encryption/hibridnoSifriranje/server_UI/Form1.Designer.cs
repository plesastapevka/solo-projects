namespace server_UI
{
    partial class Form1
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
            this.start = new System.Windows.Forms.Button();
            this.info = new System.Windows.Forms.RichTextBox();
            this.clientButton = new System.Windows.Forms.Button();
            this.fileChoose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(12, 415);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(75, 23);
            this.start.TabIndex = 1;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // info
            // 
            this.info.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.info.ForeColor = System.Drawing.Color.White;
            this.info.Location = new System.Drawing.Point(12, 12);
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(776, 397);
            this.info.TabIndex = 2;
            this.info.Text = "";
            // 
            // clientButton
            // 
            this.clientButton.Location = new System.Drawing.Point(93, 415);
            this.clientButton.Name = "clientButton";
            this.clientButton.Size = new System.Drawing.Size(75, 23);
            this.clientButton.TabIndex = 3;
            this.clientButton.Text = "Connect";
            this.clientButton.UseVisualStyleBackColor = true;
            this.clientButton.Click += new System.EventHandler(this.clientButton_Click);
            // 
            // fileChoose
            // 
            this.fileChoose.Location = new System.Drawing.Point(713, 415);
            this.fileChoose.Name = "fileChoose";
            this.fileChoose.Size = new System.Drawing.Size(75, 23);
            this.fileChoose.TabIndex = 4;
            this.fileChoose.Text = "Choose file";
            this.fileChoose.UseVisualStyleBackColor = true;
            this.fileChoose.Click += new System.EventHandler(this.fileChoose_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.fileChoose);
            this.Controls.Add(this.clientButton);
            this.Controls.Add(this.info);
            this.Controls.Add(this.start);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.RichTextBox info;
        private System.Windows.Forms.Button clientButton;
        private System.Windows.Forms.Button fileChoose;
    }
}

