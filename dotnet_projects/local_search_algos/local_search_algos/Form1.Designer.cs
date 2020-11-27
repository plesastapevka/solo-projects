namespace local_search_algos
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
            this.startState = new System.Windows.Forms.Button();
            this.startAlgo = new System.Windows.Forms.Button();
            this.hillClimb = new System.Windows.Forms.RadioButton();
            this.simAnneal = new System.Windows.Forms.RadioButton();
            this.beamSearch = new System.Windows.Forms.RadioButton();
            this.genAlgo = new System.Windows.Forms.RadioButton();
            this.sizeBox = new System.Windows.Forms.ComboBox();
            this.heuristic = new System.Windows.Forms.Label();
            this.stepCount = new System.Windows.Forms.Label();
            this.stateInfo = new System.Windows.Forms.Label();
            this.algoChoice = new System.Windows.Forms.Label();
            this.parameters = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.heuCount = new System.Windows.Forms.Label();
            this.stepCounter = new System.Windows.Forms.Label();
            this.info = new System.Windows.Forms.Label();
            this.chessCanvas = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.chessCanvas)).BeginInit();
            this.SuspendLayout();
            // 
            // startState
            // 
            this.startState.Location = new System.Drawing.Point(473, 40);
            this.startState.Name = "startState";
            this.startState.Size = new System.Drawing.Size(439, 25);
            this.startState.TabIndex = 1;
            this.startState.Text = "Generate starting state";
            this.startState.UseVisualStyleBackColor = true;
            this.startState.Click += new System.EventHandler(this.startState_Click);
            // 
            // startAlgo
            // 
            this.startAlgo.Location = new System.Drawing.Point(473, 71);
            this.startAlgo.Name = "startAlgo";
            this.startAlgo.Size = new System.Drawing.Size(439, 25);
            this.startAlgo.TabIndex = 2;
            this.startAlgo.Text = "Start algorithm";
            this.startAlgo.UseVisualStyleBackColor = true;
            this.startAlgo.Click += new System.EventHandler(this.startAlgo_Click);
            // 
            // hillClimb
            // 
            this.hillClimb.AutoSize = true;
            this.hillClimb.Location = new System.Drawing.Point(473, 183);
            this.hillClimb.Name = "hillClimb";
            this.hillClimb.Size = new System.Drawing.Size(81, 17);
            this.hillClimb.TabIndex = 3;
            this.hillClimb.TabStop = true;
            this.hillClimb.Text = "Hill Climbing";
            this.hillClimb.UseVisualStyleBackColor = true;
            this.hillClimb.CheckedChanged += new System.EventHandler(this.hillClimb_CheckedChanged);
            // 
            // simAnneal
            // 
            this.simAnneal.AutoSize = true;
            this.simAnneal.Location = new System.Drawing.Point(560, 183);
            this.simAnneal.Name = "simAnneal";
            this.simAnneal.Size = new System.Drawing.Size(121, 17);
            this.simAnneal.TabIndex = 4;
            this.simAnneal.TabStop = true;
            this.simAnneal.Text = "Simulated Annealing";
            this.simAnneal.UseVisualStyleBackColor = true;
            this.simAnneal.CheckedChanged += new System.EventHandler(this.simAnneal_CheckedChanged);
            // 
            // beamSearch
            // 
            this.beamSearch.AutoSize = true;
            this.beamSearch.Location = new System.Drawing.Point(687, 183);
            this.beamSearch.Name = "beamSearch";
            this.beamSearch.Size = new System.Drawing.Size(118, 17);
            this.beamSearch.TabIndex = 5;
            this.beamSearch.TabStop = true;
            this.beamSearch.Text = "Local Beam Search";
            this.beamSearch.UseVisualStyleBackColor = true;
            this.beamSearch.CheckedChanged += new System.EventHandler(this.beamSearch_CheckedChanged);
            // 
            // genAlgo
            // 
            this.genAlgo.AutoSize = true;
            this.genAlgo.Location = new System.Drawing.Point(811, 183);
            this.genAlgo.Name = "genAlgo";
            this.genAlgo.Size = new System.Drawing.Size(108, 17);
            this.genAlgo.TabIndex = 6;
            this.genAlgo.TabStop = true;
            this.genAlgo.Text = "Genetic Algorithm";
            this.genAlgo.UseVisualStyleBackColor = true;
            this.genAlgo.CheckedChanged += new System.EventHandler(this.genAlgo_CheckedChanged);
            // 
            // sizeBox
            // 
            this.sizeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sizeBox.FormattingEnabled = true;
            this.sizeBox.Items.AddRange(new object[] {
            "4x4",
            "5x5",
            "6x6",
            "7x7",
            "8x8",
            "9x9",
            "10x10",
            "11x11",
            "12x12"});
            this.sizeBox.Location = new System.Drawing.Point(468, 441);
            this.sizeBox.Name = "sizeBox";
            this.sizeBox.Size = new System.Drawing.Size(70, 21);
            this.sizeBox.TabIndex = 7;
            this.sizeBox.SelectedIndexChanged += new System.EventHandler(this.sizeBox_SelectedIndexChanged);
            // 
            // heuristic
            // 
            this.heuristic.AutoSize = true;
            this.heuristic.Location = new System.Drawing.Point(473, 99);
            this.heuristic.Name = "heuristic";
            this.heuristic.Size = new System.Drawing.Size(51, 13);
            this.heuristic.TabIndex = 8;
            this.heuristic.Text = "Heuristic:";
            this.heuristic.Click += new System.EventHandler(this.heuristic_Click);
            // 
            // stepCount
            // 
            this.stepCount.AutoSize = true;
            this.stepCount.Location = new System.Drawing.Point(473, 121);
            this.stepCount.Name = "stepCount";
            this.stepCount.Size = new System.Drawing.Size(37, 13);
            this.stepCount.TabIndex = 9;
            this.stepCount.Text = "Steps:";
            // 
            // stateInfo
            // 
            this.stateInfo.AutoSize = true;
            this.stateInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stateInfo.Location = new System.Drawing.Point(468, 12);
            this.stateInfo.Name = "stateInfo";
            this.stateInfo.Size = new System.Drawing.Size(67, 25);
            this.stateInfo.TabIndex = 10;
            this.stateInfo.Text = "State";
            // 
            // algoChoice
            // 
            this.algoChoice.AutoSize = true;
            this.algoChoice.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.algoChoice.Location = new System.Drawing.Point(468, 145);
            this.algoChoice.Name = "algoChoice";
            this.algoChoice.Size = new System.Drawing.Size(111, 25);
            this.algoChoice.TabIndex = 11;
            this.algoChoice.Text = "Algorithm";
            // 
            // parameters
            // 
            this.parameters.AutoSize = true;
            this.parameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.parameters.Location = new System.Drawing.Point(468, 212);
            this.parameters.Name = "parameters";
            this.parameters.Size = new System.Drawing.Size(132, 25);
            this.parameters.TabIndex = 12;
            this.parameters.Text = "Parameters";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(475, 243);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(475, 269);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "label2";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(475, 295);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "label3";
            this.label3.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(475, 321);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "label4";
            this.label4.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(475, 347);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "label5";
            this.label5.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(633, 240);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(48, 20);
            this.textBox1.TabIndex = 18;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(633, 266);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(48, 20);
            this.textBox2.TabIndex = 19;
            this.textBox2.Visible = false;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(633, 292);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(48, 20);
            this.textBox3.TabIndex = 20;
            this.textBox3.Visible = false;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(633, 318);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(48, 20);
            this.textBox4.TabIndex = 21;
            this.textBox4.Visible = false;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(633, 344);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(48, 20);
            this.textBox5.TabIndex = 22;
            this.textBox5.Visible = false;
            // 
            // heuCount
            // 
            this.heuCount.AutoSize = true;
            this.heuCount.Location = new System.Drawing.Point(565, 99);
            this.heuCount.Name = "heuCount";
            this.heuCount.Size = new System.Drawing.Size(13, 13);
            this.heuCount.TabIndex = 23;
            this.heuCount.Text = "0";
            // 
            // stepCounter
            // 
            this.stepCounter.AutoSize = true;
            this.stepCounter.Location = new System.Drawing.Point(565, 121);
            this.stepCounter.Name = "stepCounter";
            this.stepCounter.Size = new System.Drawing.Size(13, 13);
            this.stepCounter.TabIndex = 24;
            this.stepCounter.Text = "0";
            // 
            // info
            // 
            this.info.AutoSize = true;
            this.info.Location = new System.Drawing.Point(12, 465);
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(49, 13);
            this.info.TabIndex = 25;
            this.info.Text = "Bad start";
            this.info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.info.Visible = false;
            this.info.Click += new System.EventHandler(this.Info_Click);
            // 
            // chessCanvas
            // 
            this.chessCanvas.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.chessCanvas.Location = new System.Drawing.Point(12, 12);
            this.chessCanvas.Name = "chessCanvas";
            this.chessCanvas.Size = new System.Drawing.Size(450, 450);
            this.chessCanvas.TabIndex = 26;
            this.chessCanvas.TabStop = false;
            this.chessCanvas.Click += new System.EventHandler(this.chessCanvas_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(920, 482);
            this.Controls.Add(this.chessCanvas);
            this.Controls.Add(this.info);
            this.Controls.Add(this.stepCounter);
            this.Controls.Add(this.heuCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.parameters);
            this.Controls.Add(this.genAlgo);
            this.Controls.Add(this.algoChoice);
            this.Controls.Add(this.beamSearch);
            this.Controls.Add(this.hillClimb);
            this.Controls.Add(this.simAnneal);
            this.Controls.Add(this.stateInfo);
            this.Controls.Add(this.stepCount);
            this.Controls.Add(this.heuristic);
            this.Controls.Add(this.startAlgo);
            this.Controls.Add(this.sizeBox);
            this.Controls.Add(this.startState);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chessCanvas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button startState;
        private System.Windows.Forms.Button startAlgo;
        private System.Windows.Forms.RadioButton hillClimb;
        private System.Windows.Forms.RadioButton simAnneal;
        private System.Windows.Forms.RadioButton beamSearch;
        private System.Windows.Forms.RadioButton genAlgo;
        private System.Windows.Forms.ComboBox sizeBox;
        private System.Windows.Forms.Label heuristic;
        private System.Windows.Forms.Label stepCount;
        private System.Windows.Forms.Label stateInfo;
        private System.Windows.Forms.Label algoChoice;
        private System.Windows.Forms.Label parameters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label heuCount;
        private System.Windows.Forms.Label stepCounter;
        private System.Windows.Forms.Label info;
        private System.Windows.Forms.PictureBox chessCanvas;
    }
}

