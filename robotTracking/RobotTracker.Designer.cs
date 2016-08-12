namespace robotTracking
{
    partial class RobotTracker
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
            this.connectButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Object = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Z = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pitch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Yaw = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Roll = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.activeBlockLabel = new System.Windows.Forms.Label();
            this.distanceFromTipLabel = new System.Windows.Forms.Label();
            this.rollLabel = new System.Windows.Forms.Label();
            this.pitchLabel = new System.Windows.Forms.Label();
            this.yawLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pollDataCheckbox = new System.Windows.Forms.CheckBox();
            this.connectRobotButton = new System.Windows.Forms.Button();
            this.robotConnectLabel = new System.Windows.Forms.Label();
            this.testMovementButton = new System.Windows.Forms.Button();
            this.runCalibrationButton = new System.Windows.Forms.Button();
            this.experimentButton = new System.Windows.Forms.Button();
            this.buttonTestStorage = new System.Windows.Forms.Button();
            this.buttonTestRetrieval = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tipToBaseTestLabel = new System.Windows.Forms.Label();
            this.zeroMotorsButton = new System.Windows.Forms.Button();
            this.stopCalibrationButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(12, 12);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Object,
            this.X,
            this.Y,
            this.Z,
            this.Pitch,
            this.Yaw,
            this.Roll});
            this.dataGridView1.Location = new System.Drawing.Point(12, 41);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(571, 185);
            this.dataGridView1.TabIndex = 1;
            // 
            // Object
            // 
            this.Object.HeaderText = "Object";
            this.Object.Name = "Object";
            // 
            // X
            // 
            this.X.HeaderText = "X";
            this.X.Name = "X";
            // 
            // Y
            // 
            this.Y.HeaderText = "Y";
            this.Y.Name = "Y";
            // 
            // Z
            // 
            this.Z.HeaderText = "Z";
            this.Z.Name = "Z";
            // 
            // Pitch
            // 
            this.Pitch.HeaderText = "Pitch (X)";
            this.Pitch.Name = "Pitch";
            // 
            // Yaw
            // 
            this.Yaw.HeaderText = "Yaw (Y)";
            this.Yaw.Name = "Yaw";
            // 
            // Roll
            // 
            this.Roll.HeaderText = "Roll (Z)";
            this.Roll.Name = "Roll";
            // 
            // activeBlockLabel
            // 
            this.activeBlockLabel.AutoSize = true;
            this.activeBlockLabel.BackColor = System.Drawing.SystemColors.Highlight;
            this.activeBlockLabel.Location = new System.Drawing.Point(129, 244);
            this.activeBlockLabel.Name = "activeBlockLabel";
            this.activeBlockLabel.Size = new System.Drawing.Size(43, 13);
            this.activeBlockLabel.TabIndex = 4;
            this.activeBlockLabel.Text = "<none>";
            // 
            // distanceFromTipLabel
            // 
            this.distanceFromTipLabel.AutoSize = true;
            this.distanceFromTipLabel.BackColor = System.Drawing.SystemColors.Highlight;
            this.distanceFromTipLabel.Location = new System.Drawing.Point(129, 271);
            this.distanceFromTipLabel.Name = "distanceFromTipLabel";
            this.distanceFromTipLabel.Size = new System.Drawing.Size(43, 13);
            this.distanceFromTipLabel.TabIndex = 5;
            this.distanceFromTipLabel.Text = "<none>";
            // 
            // rollLabel
            // 
            this.rollLabel.AutoSize = true;
            this.rollLabel.BackColor = System.Drawing.SystemColors.Highlight;
            this.rollLabel.Location = new System.Drawing.Point(91, 326);
            this.rollLabel.Name = "rollLabel";
            this.rollLabel.Size = new System.Drawing.Size(43, 13);
            this.rollLabel.TabIndex = 7;
            this.rollLabel.Text = "<none>";
            // 
            // pitchLabel
            // 
            this.pitchLabel.AutoSize = true;
            this.pitchLabel.BackColor = System.Drawing.SystemColors.Highlight;
            this.pitchLabel.Location = new System.Drawing.Point(91, 352);
            this.pitchLabel.Name = "pitchLabel";
            this.pitchLabel.Size = new System.Drawing.Size(43, 13);
            this.pitchLabel.TabIndex = 8;
            this.pitchLabel.Text = "<none>";
            // 
            // yawLabel
            // 
            this.yawLabel.AutoSize = true;
            this.yawLabel.BackColor = System.Drawing.SystemColors.Highlight;
            this.yawLabel.Location = new System.Drawing.Point(91, 376);
            this.yawLabel.Name = "yawLabel";
            this.yawLabel.Size = new System.Drawing.Size(43, 13);
            this.yawLabel.TabIndex = 9;
            this.yawLabel.Text = "<none>";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 244);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Active Block";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 271);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Distance from tip (mm)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 299);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Orientation Difference:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 326);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Roll";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 352);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Pitch";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 376);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Yaw";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.Location = new System.Drawing.Point(220, 244);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(363, 180);
            this.listView1.TabIndex = 17;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Time";
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Message";
            this.columnHeader3.Width = 400;
            // 
            // pollDataCheckbox
            // 
            this.pollDataCheckbox.AutoSize = true;
            this.pollDataCheckbox.Location = new System.Drawing.Point(103, 16);
            this.pollDataCheckbox.Name = "pollDataCheckbox";
            this.pollDataCheckbox.Size = new System.Drawing.Size(69, 17);
            this.pollDataCheckbox.TabIndex = 18;
            this.pollDataCheckbox.Text = "Poll Data";
            this.pollDataCheckbox.UseVisualStyleBackColor = true;
            this.pollDataCheckbox.CheckedChanged += new System.EventHandler(this.pollDataCheckbox_CheckedChanged);
            // 
            // connectRobotButton
            // 
            this.connectRobotButton.Location = new System.Drawing.Point(590, 13);
            this.connectRobotButton.Name = "connectRobotButton";
            this.connectRobotButton.Size = new System.Drawing.Size(90, 21);
            this.connectRobotButton.TabIndex = 19;
            this.connectRobotButton.Text = "Connect robot";
            this.connectRobotButton.UseVisualStyleBackColor = true;
            this.connectRobotButton.Click += new System.EventHandler(this.connectRobotButton_Click);
            // 
            // robotConnectLabel
            // 
            this.robotConnectLabel.AutoSize = true;
            this.robotConnectLabel.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.robotConnectLabel.Location = new System.Drawing.Point(686, 16);
            this.robotConnectLabel.Name = "robotConnectLabel";
            this.robotConnectLabel.Size = new System.Drawing.Size(78, 13);
            this.robotConnectLabel.TabIndex = 20;
            this.robotConnectLabel.Text = "Not connected";
            // 
            // testMovementButton
            // 
            this.testMovementButton.AllowDrop = true;
            this.testMovementButton.Enabled = false;
            this.testMovementButton.Location = new System.Drawing.Point(590, 52);
            this.testMovementButton.Name = "testMovementButton";
            this.testMovementButton.Size = new System.Drawing.Size(90, 23);
            this.testMovementButton.TabIndex = 21;
            this.testMovementButton.Text = "Test Movement";
            this.testMovementButton.UseVisualStyleBackColor = true;
            this.testMovementButton.Click += new System.EventHandler(this.testMovementButton_Click);
            // 
            // runCalibrationButton
            // 
            this.runCalibrationButton.Location = new System.Drawing.Point(590, 123);
            this.runCalibrationButton.Name = "runCalibrationButton";
            this.runCalibrationButton.Size = new System.Drawing.Size(93, 21);
            this.runCalibrationButton.TabIndex = 22;
            this.runCalibrationButton.Text = "Run calibration";
            this.runCalibrationButton.UseVisualStyleBackColor = true;
            this.runCalibrationButton.Click += new System.EventHandler(this.runCalibrationButton_Click);
            // 
            // experimentButton
            // 
            this.experimentButton.Location = new System.Drawing.Point(590, 94);
            this.experimentButton.Name = "experimentButton";
            this.experimentButton.Size = new System.Drawing.Size(93, 23);
            this.experimentButton.TabIndex = 23;
            this.experimentButton.Text = "Start experiment";
            this.experimentButton.UseVisualStyleBackColor = true;
            this.experimentButton.Click += new System.EventHandler(this.experimentButton_Click);
            // 
            // buttonTestStorage
            // 
            this.buttonTestStorage.Location = new System.Drawing.Point(647, 203);
            this.buttonTestStorage.Name = "buttonTestStorage";
            this.buttonTestStorage.Size = new System.Drawing.Size(75, 23);
            this.buttonTestStorage.TabIndex = 24;
            this.buttonTestStorage.Text = "test storage";
            this.buttonTestStorage.UseVisualStyleBackColor = true;
            this.buttonTestStorage.Click += new System.EventHandler(this.buttonTestStorage_Click);
            // 
            // buttonTestRetrieval
            // 
            this.buttonTestRetrieval.Location = new System.Drawing.Point(647, 244);
            this.buttonTestRetrieval.Name = "buttonTestRetrieval";
            this.buttonTestRetrieval.Size = new System.Drawing.Size(75, 23);
            this.buttonTestRetrieval.TabIndex = 25;
            this.buttonTestRetrieval.Text = "test retrieval";
            this.buttonTestRetrieval.UseVisualStyleBackColor = true;
            this.buttonTestRetrieval.Click += new System.EventHandler(this.buttonTestRetrieval_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(589, 175);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Tip to Base (test)";
            // 
            // tipToBaseTestLabel
            // 
            this.tipToBaseTestLabel.AutoSize = true;
            this.tipToBaseTestLabel.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.tipToBaseTestLabel.Location = new System.Drawing.Point(686, 175);
            this.tipToBaseTestLabel.Name = "tipToBaseTestLabel";
            this.tipToBaseTestLabel.Size = new System.Drawing.Size(43, 13);
            this.tipToBaseTestLabel.TabIndex = 27;
            this.tipToBaseTestLabel.Text = "<none>";
            // 
            // zeroMotorsButton
            // 
            this.zeroMotorsButton.Enabled = false;
            this.zeroMotorsButton.Location = new System.Drawing.Point(689, 52);
            this.zeroMotorsButton.Name = "zeroMotorsButton";
            this.zeroMotorsButton.Size = new System.Drawing.Size(75, 23);
            this.zeroMotorsButton.TabIndex = 28;
            this.zeroMotorsButton.Text = "Zero motors";
            this.zeroMotorsButton.UseVisualStyleBackColor = true;
            this.zeroMotorsButton.Click += new System.EventHandler(this.zeroMotorsButton_Click);
            // 
            // stopCalibrationButton
            // 
            this.stopCalibrationButton.Location = new System.Drawing.Point(689, 121);
            this.stopCalibrationButton.Name = "stopCalibrationButton";
            this.stopCalibrationButton.Size = new System.Drawing.Size(93, 23);
            this.stopCalibrationButton.TabIndex = 29;
            this.stopCalibrationButton.Text = "Stop calibration";
            this.stopCalibrationButton.UseVisualStyleBackColor = true;
            this.stopCalibrationButton.Click += new System.EventHandler(this.stopCalibrationButton_Click);
            // 
            // RobotTracker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.stopCalibrationButton);
            this.Controls.Add(this.zeroMotorsButton);
            this.Controls.Add(this.tipToBaseTestLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.buttonTestRetrieval);
            this.Controls.Add(this.buttonTestStorage);
            this.Controls.Add(this.experimentButton);
            this.Controls.Add(this.runCalibrationButton);
            this.Controls.Add(this.testMovementButton);
            this.Controls.Add(this.robotConnectLabel);
            this.Controls.Add(this.connectRobotButton);
            this.Controls.Add(this.pollDataCheckbox);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.yawLabel);
            this.Controls.Add(this.pitchLabel);
            this.Controls.Add(this.rollLabel);
            this.Controls.Add(this.distanceFromTipLabel);
            this.Controls.Add(this.activeBlockLabel);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.connectButton);
            this.Name = "RobotTracker";
            this.Text = "Robot Tracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RobotTracker_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Object;
        private System.Windows.Forms.DataGridViewTextBoxColumn X;
        private System.Windows.Forms.DataGridViewTextBoxColumn Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn Z;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pitch;
        private System.Windows.Forms.DataGridViewTextBoxColumn Yaw;
        private System.Windows.Forms.DataGridViewTextBoxColumn Roll;
        private System.Windows.Forms.Label activeBlockLabel;
        private System.Windows.Forms.Label distanceFromTipLabel;
        private System.Windows.Forms.Label rollLabel;
        private System.Windows.Forms.Label pitchLabel;
        private System.Windows.Forms.Label yawLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox pollDataCheckbox;
        private System.Windows.Forms.Button connectRobotButton;
        private System.Windows.Forms.Label robotConnectLabel;
        private System.Windows.Forms.Button testMovementButton;
        private System.Windows.Forms.Button runCalibrationButton;
        private System.Windows.Forms.Button experimentButton;
        private System.Windows.Forms.Button buttonTestStorage;
        private System.Windows.Forms.Button buttonTestRetrieval;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label tipToBaseTestLabel;
        private System.Windows.Forms.Button zeroMotorsButton;
        private System.Windows.Forms.Button stopCalibrationButton;
    }
}

