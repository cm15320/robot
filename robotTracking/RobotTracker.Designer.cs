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
            this.getCalibrationButton = new System.Windows.Forms.Button();
            this.testRegression = new System.Windows.Forms.Button();
            this.pauseCalibrationButton = new System.Windows.Forms.Button();
            this.logZeroButton = new System.Windows.Forms.Button();
            this.runTestPointsButton = new System.Windows.Forms.Button();
            this.getAllDataButton = new System.Windows.Forms.Button();
            this.valTextBox1 = new System.Windows.Forms.TextBox();
            this.valTextBox2 = new System.Windows.Forms.TextBox();
            this.valTextBox3 = new System.Windows.Forms.TextBox();
            this.valTextBox4 = new System.Windows.Forms.TextBox();
            this.getMotorsRadioButton = new System.Windows.Forms.RadioButton();
            this.getPositionRadioButton = new System.Windows.Forms.RadioButton();
            this.bandwidthTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.followLivePointButton = new System.Windows.Forms.Button();
            this.relativeTargetPointX = new System.Windows.Forms.TextBox();
            this.relativeTargetPointY = new System.Windows.Forms.TextBox();
            this.relativeTargetPointZ = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.moveToRelPointButton = new System.Windows.Forms.Button();
            this.moveToBodyButton = new System.Windows.Forms.Button();
            this.followBodyButton = new System.Windows.Forms.Button();
            this.userStudyButton = new System.Windows.Forms.Button();
            this.colourArrangeRobot = new System.Windows.Forms.RadioButton();
            this.gesturingPerformance = new System.Windows.Forms.RadioButton();
            this.colourArrangeUser = new System.Windows.Forms.RadioButton();
            this.bodePlotButton = new System.Windows.Forms.Button();
            this.generateBode = new System.Windows.Forms.Button();
            this.trackedStatus = new System.Windows.Forms.Label();
            this.testReadTargetPositionsButton = new System.Windows.Forms.Button();
            this.testTriggerButton = new System.Windows.Forms.Button();
            this.undoTargetButton = new System.Windows.Forms.Button();
            this.moveToCurrentPositionButton = new System.Windows.Forms.Button();
            this.generatePositionsButton = new System.Windows.Forms.Button();
            this.m3Offset = new System.Windows.Forms.TextBox();
            this.m1Offset = new System.Windows.Forms.TextBox();
            this.writeOffsetButton = new System.Windows.Forms.Button();
            this.storeOffsetButton = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.readOffsetsButton = new System.Windows.Forms.Button();
            this.angleAveragingButton = new System.Windows.Forms.Button();
            this.randomiseCheckBox = new System.Windows.Forms.CheckBox();
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
            this.listView1.Location = new System.Drawing.Point(220, 267);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(342, 180);
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
            this.runCalibrationButton.Location = new System.Drawing.Point(590, 141);
            this.runCalibrationButton.Name = "runCalibrationButton";
            this.runCalibrationButton.Size = new System.Drawing.Size(90, 21);
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
            this.buttonTestStorage.Location = new System.Drawing.Point(12, 486);
            this.buttonTestStorage.Name = "buttonTestStorage";
            this.buttonTestStorage.Size = new System.Drawing.Size(75, 23);
            this.buttonTestStorage.TabIndex = 24;
            this.buttonTestStorage.Text = "test storage";
            this.buttonTestStorage.UseVisualStyleBackColor = true;
            this.buttonTestStorage.Click += new System.EventHandler(this.buttonTestStorage_Click);
            // 
            // buttonTestRetrieval
            // 
            this.buttonTestRetrieval.Location = new System.Drawing.Point(12, 527);
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
            this.label7.Location = new System.Drawing.Point(593, 213);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Tip to Base (test)";
            // 
            // tipToBaseTestLabel
            // 
            this.tipToBaseTestLabel.AutoSize = true;
            this.tipToBaseTestLabel.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.tipToBaseTestLabel.Location = new System.Drawing.Point(690, 213);
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
            // getCalibrationButton
            // 
            this.getCalibrationButton.Location = new System.Drawing.Point(689, 94);
            this.getCalibrationButton.Name = "getCalibrationButton";
            this.getCalibrationButton.Size = new System.Drawing.Size(93, 39);
            this.getCalibrationButton.TabIndex = 30;
            this.getCalibrationButton.Text = "Get base calibration data";
            this.getCalibrationButton.UseVisualStyleBackColor = true;
            this.getCalibrationButton.Click += new System.EventHandler(this.getCalibrationButton_Click);
            // 
            // testRegression
            // 
            this.testRegression.Enabled = false;
            this.testRegression.Location = new System.Drawing.Point(594, 332);
            this.testRegression.Name = "testRegression";
            this.testRegression.Size = new System.Drawing.Size(188, 23);
            this.testRegression.TabIndex = 31;
            this.testRegression.Text = "test regression function";
            this.testRegression.UseVisualStyleBackColor = true;
            this.testRegression.Click += new System.EventHandler(this.testRegression_Click);
            // 
            // pauseCalibrationButton
            // 
            this.pauseCalibrationButton.Enabled = false;
            this.pauseCalibrationButton.Location = new System.Drawing.Point(587, 168);
            this.pauseCalibrationButton.Name = "pauseCalibrationButton";
            this.pauseCalibrationButton.Size = new System.Drawing.Size(93, 42);
            this.pauseCalibrationButton.TabIndex = 32;
            this.pauseCalibrationButton.Text = "Pause calibration";
            this.pauseCalibrationButton.UseVisualStyleBackColor = true;
            this.pauseCalibrationButton.Click += new System.EventHandler(this.pauseCalibrationButton_Click);
            // 
            // logZeroButton
            // 
            this.logZeroButton.Enabled = false;
            this.logZeroButton.Location = new System.Drawing.Point(12, 447);
            this.logZeroButton.Name = "logZeroButton";
            this.logZeroButton.Size = new System.Drawing.Size(75, 23);
            this.logZeroButton.TabIndex = 34;
            this.logZeroButton.Text = "log zero pos";
            this.logZeroButton.UseVisualStyleBackColor = true;
            // 
            // runTestPointsButton
            // 
            this.runTestPointsButton.Location = new System.Drawing.Point(689, 141);
            this.runTestPointsButton.Name = "runTestPointsButton";
            this.runTestPointsButton.Size = new System.Drawing.Size(93, 22);
            this.runTestPointsButton.TabIndex = 35;
            this.runTestPointsButton.Text = "Run test points";
            this.runTestPointsButton.UseVisualStyleBackColor = true;
            this.runTestPointsButton.Click += new System.EventHandler(this.runTestPointsButton_Click);
            // 
            // getAllDataButton
            // 
            this.getAllDataButton.Location = new System.Drawing.Point(689, 168);
            this.getAllDataButton.Name = "getAllDataButton";
            this.getAllDataButton.Size = new System.Drawing.Size(93, 42);
            this.getAllDataButton.TabIndex = 36;
            this.getAllDataButton.Text = "Get all data and test bandwidth";
            this.getAllDataButton.UseVisualStyleBackColor = true;
            this.getAllDataButton.Click += new System.EventHandler(this.getAllDataButton_Click);
            // 
            // valTextBox1
            // 
            this.valTextBox1.Location = new System.Drawing.Point(596, 360);
            this.valTextBox1.Name = "valTextBox1";
            this.valTextBox1.Size = new System.Drawing.Size(42, 20);
            this.valTextBox1.TabIndex = 37;
            this.valTextBox1.Text = "0.238252938";
            // 
            // valTextBox2
            // 
            this.valTextBox2.Location = new System.Drawing.Point(644, 360);
            this.valTextBox2.Name = "valTextBox2";
            this.valTextBox2.Size = new System.Drawing.Size(42, 20);
            this.valTextBox2.TabIndex = 37;
            this.valTextBox2.Text = "0.00461266";
            // 
            // valTextBox3
            // 
            this.valTextBox3.Location = new System.Drawing.Point(692, 360);
            this.valTextBox3.Name = "valTextBox3";
            this.valTextBox3.Size = new System.Drawing.Size(42, 20);
            this.valTextBox3.TabIndex = 37;
            this.valTextBox3.Text = "-0.179042518";
            // 
            // valTextBox4
            // 
            this.valTextBox4.Location = new System.Drawing.Point(740, 360);
            this.valTextBox4.Name = "valTextBox4";
            this.valTextBox4.Size = new System.Drawing.Size(42, 20);
            this.valTextBox4.TabIndex = 37;
            this.valTextBox4.Text = "0";
            // 
            // getMotorsRadioButton
            // 
            this.getMotorsRadioButton.AutoSize = true;
            this.getMotorsRadioButton.Location = new System.Drawing.Point(596, 285);
            this.getMotorsRadioButton.Name = "getMotorsRadioButton";
            this.getMotorsRadioButton.Size = new System.Drawing.Size(74, 17);
            this.getMotorsRadioButton.TabIndex = 38;
            this.getMotorsRadioButton.Text = "get motors";
            this.getMotorsRadioButton.UseVisualStyleBackColor = true;
            // 
            // getPositionRadioButton
            // 
            this.getPositionRadioButton.AutoSize = true;
            this.getPositionRadioButton.Location = new System.Drawing.Point(596, 308);
            this.getPositionRadioButton.Name = "getPositionRadioButton";
            this.getPositionRadioButton.Size = new System.Drawing.Size(79, 17);
            this.getPositionRadioButton.TabIndex = 38;
            this.getPositionRadioButton.Text = "get position";
            this.getPositionRadioButton.UseVisualStyleBackColor = true;
            // 
            // bandwidthTextBox
            // 
            this.bandwidthTextBox.Location = new System.Drawing.Point(655, 384);
            this.bandwidthTextBox.Name = "bandwidthTextBox";
            this.bandwidthTextBox.Size = new System.Drawing.Size(36, 20);
            this.bandwidthTextBox.TabIndex = 39;
            this.bandwidthTextBox.Text = "0.0008";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(593, 387);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 40;
            this.label8.Text = "bandwidth:";
            // 
            // followLivePointButton
            // 
            this.followLivePointButton.Enabled = false;
            this.followLivePointButton.Location = new System.Drawing.Point(241, 456);
            this.followLivePointButton.Name = "followLivePointButton";
            this.followLivePointButton.Size = new System.Drawing.Size(231, 23);
            this.followLivePointButton.TabIndex = 41;
            this.followLivePointButton.Text = "Follow relative point live";
            this.followLivePointButton.UseCompatibleTextRendering = true;
            this.followLivePointButton.UseVisualStyleBackColor = true;
            this.followLivePointButton.Click += new System.EventHandler(this.followLivePointButton_Click);
            // 
            // relativeTargetPointX
            // 
            this.relativeTargetPointX.Location = new System.Drawing.Point(241, 489);
            this.relativeTargetPointX.Name = "relativeTargetPointX";
            this.relativeTargetPointX.Size = new System.Drawing.Size(60, 20);
            this.relativeTargetPointX.TabIndex = 42;
            this.relativeTargetPointX.Text = "0.237250075";
            // 
            // relativeTargetPointY
            // 
            this.relativeTargetPointY.Location = new System.Drawing.Point(327, 489);
            this.relativeTargetPointY.Name = "relativeTargetPointY";
            this.relativeTargetPointY.Size = new System.Drawing.Size(60, 20);
            this.relativeTargetPointY.TabIndex = 42;
            this.relativeTargetPointY.Text = "-0.00166291371";
            // 
            // relativeTargetPointZ
            // 
            this.relativeTargetPointZ.Location = new System.Drawing.Point(412, 489);
            this.relativeTargetPointZ.Name = "relativeTargetPointZ";
            this.relativeTargetPointZ.Size = new System.Drawing.Size(60, 20);
            this.relativeTargetPointZ.TabIndex = 42;
            this.relativeTargetPointZ.Text = "-0.182035267";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(263, 512);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 13);
            this.label9.TabIndex = 43;
            this.label9.Text = "X";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(351, 512);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 13);
            this.label10.TabIndex = 43;
            this.label10.Text = "Y";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(433, 512);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 13);
            this.label11.TabIndex = 43;
            this.label11.Text = "Z";
            // 
            // moveToRelPointButton
            // 
            this.moveToRelPointButton.Enabled = false;
            this.moveToRelPointButton.Location = new System.Drawing.Point(241, 528);
            this.moveToRelPointButton.Name = "moveToRelPointButton";
            this.moveToRelPointButton.Size = new System.Drawing.Size(231, 23);
            this.moveToRelPointButton.TabIndex = 44;
            this.moveToRelPointButton.Text = "Move to relative point";
            this.moveToRelPointButton.UseVisualStyleBackColor = true;
            this.moveToRelPointButton.Click += new System.EventHandler(this.moveToRelPointButton_Click);
            // 
            // moveToBodyButton
            // 
            this.moveToBodyButton.Enabled = false;
            this.moveToBodyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.moveToBodyButton.Location = new System.Drawing.Point(103, 527);
            this.moveToBodyButton.Name = "moveToBodyButton";
            this.moveToBodyButton.Size = new System.Drawing.Size(110, 23);
            this.moveToBodyButton.TabIndex = 45;
            this.moveToBodyButton.Text = "Move to body";
            this.moveToBodyButton.UseVisualStyleBackColor = true;
            this.moveToBodyButton.Click += new System.EventHandler(this.moveToBodyButton_Click);
            // 
            // followBodyButton
            // 
            this.followBodyButton.Enabled = false;
            this.followBodyButton.Location = new System.Drawing.Point(103, 488);
            this.followBodyButton.Name = "followBodyButton";
            this.followBodyButton.Size = new System.Drawing.Size(110, 23);
            this.followBodyButton.TabIndex = 46;
            this.followBodyButton.Text = "Follow body live";
            this.followBodyButton.UseVisualStyleBackColor = true;
            this.followBodyButton.Click += new System.EventHandler(this.followBodyButton_Click);
            // 
            // userStudyButton
            // 
            this.userStudyButton.Enabled = false;
            this.userStudyButton.Location = new System.Drawing.Point(596, 424);
            this.userStudyButton.Name = "userStudyButton";
            this.userStudyButton.Size = new System.Drawing.Size(120, 23);
            this.userStudyButton.TabIndex = 47;
            this.userStudyButton.Text = "Start User Study";
            this.userStudyButton.UseVisualStyleBackColor = true;
            this.userStudyButton.Click += new System.EventHandler(this.userStudyButton_Click);
            // 
            // colourArrangeRobot
            // 
            this.colourArrangeRobot.AutoSize = true;
            this.colourArrangeRobot.Enabled = false;
            this.colourArrangeRobot.Location = new System.Drawing.Point(596, 453);
            this.colourArrangeRobot.Name = "colourArrangeRobot";
            this.colourArrangeRobot.Size = new System.Drawing.Size(128, 17);
            this.colourArrangeRobot.TabIndex = 48;
            this.colourArrangeRobot.Text = "arrange blocks (robot)";
            this.colourArrangeRobot.UseVisualStyleBackColor = true;
            // 
            // gesturingPerformance
            // 
            this.gesturingPerformance.AutoSize = true;
            this.gesturingPerformance.Checked = true;
            this.gesturingPerformance.Enabled = false;
            this.gesturingPerformance.Location = new System.Drawing.Point(596, 499);
            this.gesturingPerformance.Name = "gesturingPerformance";
            this.gesturingPerformance.Size = new System.Drawing.Size(130, 17);
            this.gesturingPerformance.TabIndex = 48;
            this.gesturingPerformance.TabStop = true;
            this.gesturingPerformance.Text = "gesturing performance";
            this.gesturingPerformance.UseVisualStyleBackColor = true;
            // 
            // colourArrangeUser
            // 
            this.colourArrangeUser.AutoSize = true;
            this.colourArrangeUser.Enabled = false;
            this.colourArrangeUser.Location = new System.Drawing.Point(596, 476);
            this.colourArrangeUser.Name = "colourArrangeUser";
            this.colourArrangeUser.Size = new System.Drawing.Size(124, 17);
            this.colourArrangeUser.TabIndex = 48;
            this.colourArrangeUser.Text = "arrange blocks (user)";
            this.colourArrangeUser.UseVisualStyleBackColor = true;
            // 
            // bodePlotButton
            // 
            this.bodePlotButton.Enabled = false;
            this.bodePlotButton.Location = new System.Drawing.Point(554, 527);
            this.bodePlotButton.Name = "bodePlotButton";
            this.bodePlotButton.Size = new System.Drawing.Size(95, 23);
            this.bodePlotButton.TabIndex = 47;
            this.bodePlotButton.Text = "Start Bode Plot";
            this.bodePlotButton.UseVisualStyleBackColor = true;
            this.bodePlotButton.Click += new System.EventHandler(this.bodePlotButton_Click);
            // 
            // generateBode
            // 
            this.generateBode.Location = new System.Drawing.Point(661, 527);
            this.generateBode.Name = "generateBode";
            this.generateBode.Size = new System.Drawing.Size(121, 23);
            this.generateBode.TabIndex = 49;
            this.generateBode.Text = "Generate Bode Plot";
            this.generateBode.UseVisualStyleBackColor = true;
            this.generateBode.Click += new System.EventHandler(this.generateBode_Click);
            // 
            // trackedStatus
            // 
            this.trackedStatus.AutoSize = true;
            this.trackedStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackedStatus.Location = new System.Drawing.Point(309, 236);
            this.trackedStatus.Name = "trackedStatus";
            this.trackedStatus.Size = new System.Drawing.Size(117, 24);
            this.trackedStatus.TabIndex = 50;
            this.trackedStatus.Text = "Not Tracking";
            // 
            // testReadTargetPositionsButton
            // 
            this.testReadTargetPositionsButton.Location = new System.Drawing.Point(12, 408);
            this.testReadTargetPositionsButton.Name = "testReadTargetPositionsButton";
            this.testReadTargetPositionsButton.Size = new System.Drawing.Size(160, 23);
            this.testReadTargetPositionsButton.TabIndex = 51;
            this.testReadTargetPositionsButton.Text = "Test Read Target Positions";
            this.testReadTargetPositionsButton.UseVisualStyleBackColor = true;
            this.testReadTargetPositionsButton.Click += new System.EventHandler(this.testReadTargetPositionsButton_Click);
            // 
            // testTriggerButton
            // 
            this.testTriggerButton.Enabled = false;
            this.testTriggerButton.Location = new System.Drawing.Point(103, 447);
            this.testTriggerButton.Name = "testTriggerButton";
            this.testTriggerButton.Size = new System.Drawing.Size(110, 23);
            this.testTriggerButton.TabIndex = 52;
            this.testTriggerButton.Text = "Test trigger";
            this.testTriggerButton.UseVisualStyleBackColor = true;
            this.testTriggerButton.Click += new System.EventHandler(this.testTriggerButton_Click);
            // 
            // undoTargetButton
            // 
            this.undoTargetButton.Location = new System.Drawing.Point(733, 424);
            this.undoTargetButton.Name = "undoTargetButton";
            this.undoTargetButton.Size = new System.Drawing.Size(75, 23);
            this.undoTargetButton.TabIndex = 53;
            this.undoTargetButton.Text = "Undo target";
            this.undoTargetButton.UseVisualStyleBackColor = true;
            this.undoTargetButton.Click += new System.EventHandler(this.undoTargetButton_Click);
            // 
            // moveToCurrentPositionButton
            // 
            this.moveToCurrentPositionButton.Enabled = false;
            this.moveToCurrentPositionButton.Location = new System.Drawing.Point(487, 456);
            this.moveToCurrentPositionButton.Name = "moveToCurrentPositionButton";
            this.moveToCurrentPositionButton.Size = new System.Drawing.Size(75, 55);
            this.moveToCurrentPositionButton.TabIndex = 54;
            this.moveToCurrentPositionButton.Text = "Move to current position";
            this.moveToCurrentPositionButton.UseVisualStyleBackColor = true;
            this.moveToCurrentPositionButton.Click += new System.EventHandler(this.moveToCurrentPositionButton_Click);
            // 
            // generatePositionsButton
            // 
            this.generatePositionsButton.Enabled = false;
            this.generatePositionsButton.Location = new System.Drawing.Point(820, 456);
            this.generatePositionsButton.Name = "generatePositionsButton";
            this.generatePositionsButton.Size = new System.Drawing.Size(87, 53);
            this.generatePositionsButton.TabIndex = 55;
            this.generatePositionsButton.Text = "Generate positions by trigger";
            this.generatePositionsButton.UseVisualStyleBackColor = true;
            this.generatePositionsButton.Click += new System.EventHandler(this.generatePositionsButton_Click);
            // 
            // m3Offset
            // 
            this.m3Offset.Location = new System.Drawing.Point(802, 282);
            this.m3Offset.Name = "m3Offset";
            this.m3Offset.Size = new System.Drawing.Size(56, 20);
            this.m3Offset.TabIndex = 56;
            this.m3Offset.Text = "90";
            // 
            // m1Offset
            // 
            this.m1Offset.Location = new System.Drawing.Point(740, 282);
            this.m1Offset.Name = "m1Offset";
            this.m1Offset.Size = new System.Drawing.Size(56, 20);
            this.m1Offset.TabIndex = 56;
            this.m1Offset.Text = "90";
            // 
            // writeOffsetButton
            // 
            this.writeOffsetButton.Enabled = false;
            this.writeOffsetButton.Location = new System.Drawing.Point(740, 308);
            this.writeOffsetButton.Name = "writeOffsetButton";
            this.writeOffsetButton.Size = new System.Drawing.Size(60, 23);
            this.writeOffsetButton.TabIndex = 57;
            this.writeOffsetButton.Text = "Write";
            this.writeOffsetButton.UseVisualStyleBackColor = true;
            this.writeOffsetButton.Click += new System.EventHandler(this.writeOffsetButton_Click);
            // 
            // storeOffsetButton
            // 
            this.storeOffsetButton.Enabled = false;
            this.storeOffsetButton.Location = new System.Drawing.Point(806, 308);
            this.storeOffsetButton.Name = "storeOffsetButton";
            this.storeOffsetButton.Size = new System.Drawing.Size(88, 23);
            this.storeOffsetButton.TabIndex = 58;
            this.storeOffsetButton.Text = "Store offset";
            this.storeOffsetButton.UseVisualStyleBackColor = true;
            this.storeOffsetButton.Click += new System.EventHandler(this.storeOffsetButton_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(772, 236);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 24);
            this.label12.TabIndex = 59;
            this.label12.Text = "Offsets";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(761, 266);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(21, 13);
            this.label13.TabIndex = 60;
            this.label13.Text = "m1";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(817, 266);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(21, 13);
            this.label14.TabIndex = 60;
            this.label14.Text = "m3";
            // 
            // readOffsetsButton
            // 
            this.readOffsetsButton.Location = new System.Drawing.Point(776, 52);
            this.readOffsetsButton.Name = "readOffsetsButton";
            this.readOffsetsButton.Size = new System.Drawing.Size(82, 23);
            this.readOffsetsButton.TabIndex = 61;
            this.readOffsetsButton.Text = "Read Offsets";
            this.readOffsetsButton.UseVisualStyleBackColor = true;
            this.readOffsetsButton.Click += new System.EventHandler(this.readOffsetsButton_Click);
            // 
            // angleAveragingButton
            // 
            this.angleAveragingButton.Enabled = false;
            this.angleAveragingButton.Location = new System.Drawing.Point(789, 94);
            this.angleAveragingButton.Name = "angleAveragingButton";
            this.angleAveragingButton.Size = new System.Drawing.Size(69, 39);
            this.angleAveragingButton.TabIndex = 62;
            this.angleAveragingButton.Text = "Use angle averaging";
            this.angleAveragingButton.UseVisualStyleBackColor = true;
            this.angleAveragingButton.Click += new System.EventHandler(this.angleAveragingButton_Click);
            // 
            // randomiseCheckBox
            // 
            this.randomiseCheckBox.AutoSize = true;
            this.randomiseCheckBox.Enabled = false;
            this.randomiseCheckBox.Location = new System.Drawing.Point(728, 499);
            this.randomiseCheckBox.Name = "randomiseCheckBox";
            this.randomiseCheckBox.Size = new System.Drawing.Size(79, 17);
            this.randomiseCheckBox.TabIndex = 63;
            this.randomiseCheckBox.Text = "Randomise";
            this.randomiseCheckBox.UseVisualStyleBackColor = true;
            this.randomiseCheckBox.CheckedChanged += new System.EventHandler(this.randomiseCheckBox_CheckedChanged);
            // 
            // RobotTracker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 562);
            this.Controls.Add(this.randomiseCheckBox);
            this.Controls.Add(this.angleAveragingButton);
            this.Controls.Add(this.readOffsetsButton);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.storeOffsetButton);
            this.Controls.Add(this.writeOffsetButton);
            this.Controls.Add(this.m1Offset);
            this.Controls.Add(this.m3Offset);
            this.Controls.Add(this.generatePositionsButton);
            this.Controls.Add(this.moveToCurrentPositionButton);
            this.Controls.Add(this.undoTargetButton);
            this.Controls.Add(this.testTriggerButton);
            this.Controls.Add(this.testReadTargetPositionsButton);
            this.Controls.Add(this.trackedStatus);
            this.Controls.Add(this.generateBode);
            this.Controls.Add(this.colourArrangeUser);
            this.Controls.Add(this.gesturingPerformance);
            this.Controls.Add(this.colourArrangeRobot);
            this.Controls.Add(this.bodePlotButton);
            this.Controls.Add(this.userStudyButton);
            this.Controls.Add(this.followBodyButton);
            this.Controls.Add(this.moveToBodyButton);
            this.Controls.Add(this.moveToRelPointButton);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.relativeTargetPointZ);
            this.Controls.Add(this.relativeTargetPointY);
            this.Controls.Add(this.relativeTargetPointX);
            this.Controls.Add(this.followLivePointButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.bandwidthTextBox);
            this.Controls.Add(this.getPositionRadioButton);
            this.Controls.Add(this.getMotorsRadioButton);
            this.Controls.Add(this.valTextBox4);
            this.Controls.Add(this.valTextBox3);
            this.Controls.Add(this.valTextBox2);
            this.Controls.Add(this.valTextBox1);
            this.Controls.Add(this.getAllDataButton);
            this.Controls.Add(this.runTestPointsButton);
            this.Controls.Add(this.logZeroButton);
            this.Controls.Add(this.pauseCalibrationButton);
            this.Controls.Add(this.testRegression);
            this.Controls.Add(this.getCalibrationButton);
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
        private System.Windows.Forms.Button getCalibrationButton;
        private System.Windows.Forms.Button testRegression;
        private System.Windows.Forms.Button pauseCalibrationButton;
        private System.Windows.Forms.Button logZeroButton;
        private System.Windows.Forms.Button runTestPointsButton;
        private System.Windows.Forms.Button getAllDataButton;
        private System.Windows.Forms.TextBox valTextBox1;
        private System.Windows.Forms.TextBox valTextBox2;
        private System.Windows.Forms.TextBox valTextBox3;
        private System.Windows.Forms.TextBox valTextBox4;
        private System.Windows.Forms.RadioButton getMotorsRadioButton;
        private System.Windows.Forms.RadioButton getPositionRadioButton;
        private System.Windows.Forms.TextBox bandwidthTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button followLivePointButton;
        private System.Windows.Forms.TextBox relativeTargetPointX;
        private System.Windows.Forms.TextBox relativeTargetPointY;
        private System.Windows.Forms.TextBox relativeTargetPointZ;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button moveToRelPointButton;
        private System.Windows.Forms.Button moveToBodyButton;
        private System.Windows.Forms.Button followBodyButton;
        private System.Windows.Forms.Button userStudyButton;
        private System.Windows.Forms.RadioButton colourArrangeRobot;
        private System.Windows.Forms.RadioButton gesturingPerformance;
        private System.Windows.Forms.RadioButton colourArrangeUser;
        private System.Windows.Forms.Button bodePlotButton;
        private System.Windows.Forms.Button generateBode;
        private System.Windows.Forms.Label trackedStatus;
        private System.Windows.Forms.Button testReadTargetPositionsButton;
        private System.Windows.Forms.Button testTriggerButton;
        private System.Windows.Forms.Button undoTargetButton;
        private System.Windows.Forms.Button moveToCurrentPositionButton;
        private System.Windows.Forms.Button generatePositionsButton;
        private System.Windows.Forms.TextBox m3Offset;
        private System.Windows.Forms.TextBox m1Offset;
        private System.Windows.Forms.Button writeOffsetButton;
        private System.Windows.Forms.Button storeOffsetButton;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button readOffsetsButton;
        private System.Windows.Forms.Button angleAveragingButton;
        private System.Windows.Forms.CheckBox randomiseCheckBox;
    }
}

