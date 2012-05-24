namespace DOLConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FullNameTextBox = new System.Windows.Forms.TextBox();
            this.shortNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.serverTypeComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mysqlPasswordTextBox = new System.Windows.Forms.TextBox();
            this.mysqlUsernameTextBox = new System.Windows.Forms.TextBox();
            this.mysqlDatabaseTextBox = new System.Windows.Forms.TextBox();
            this.mysqlHostTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbxPort1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbxPort2 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbxPort3 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FullNameTextBox);
            this.groupBox1.Controls.Add(this.shortNameTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.serverTypeComboBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(197, 131);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server Settings";
            // 
            // FullNameTextBox
            // 
            this.FullNameTextBox.Location = new System.Drawing.Point(80, 68);
            this.FullNameTextBox.Name = "FullNameTextBox";
            this.FullNameTextBox.Size = new System.Drawing.Size(100, 21);
            this.FullNameTextBox.TabIndex = 5;
            this.FullNameTextBox.Text = "7Road Server";
            this.FullNameTextBox.MouseEnter += new System.EventHandler(this.FullNameTextBox_MouseEnter);
            // 
            // shortNameTextBox
            // 
            this.shortNameTextBox.Location = new System.Drawing.Point(80, 44);
            this.shortNameTextBox.Name = "shortNameTextBox";
            this.shortNameTextBox.Size = new System.Drawing.Size(100, 21);
            this.shortNameTextBox.TabIndex = 4;
            this.shortNameTextBox.Text = "RoadServer";
            this.shortNameTextBox.MouseEnter += new System.EventHandler(this.shortNameTextBox_MouseEnter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "Full Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Short Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server Type:";
            // 
            // serverTypeComboBox
            // 
            this.serverTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverTypeComboBox.FormattingEnabled = true;
            this.serverTypeComboBox.Items.AddRange(new object[] {
            "Normal"});
            this.serverTypeComboBox.Location = new System.Drawing.Point(81, 23);
            this.serverTypeComboBox.Name = "serverTypeComboBox";
            this.serverTypeComboBox.Size = new System.Drawing.Size(99, 20);
            this.serverTypeComboBox.TabIndex = 0;
            this.serverTypeComboBox.MouseEnter += new System.EventHandler(this.serverTypeComboBox_MouseEnter);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mysqlPasswordTextBox);
            this.groupBox2.Controls.Add(this.mysqlUsernameTextBox);
            this.groupBox2.Controls.Add(this.mysqlDatabaseTextBox);
            this.groupBox2.Controls.Add(this.mysqlHostTextBox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(215, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(217, 131);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database Settings";
            // 
            // mysqlPasswordTextBox
            // 
            this.mysqlPasswordTextBox.Location = new System.Drawing.Point(106, 90);
            this.mysqlPasswordTextBox.Name = "mysqlPasswordTextBox";
            this.mysqlPasswordTextBox.PasswordChar = '*';
            this.mysqlPasswordTextBox.Size = new System.Drawing.Size(100, 21);
            this.mysqlPasswordTextBox.TabIndex = 9;
            this.mysqlPasswordTextBox.MouseEnter += new System.EventHandler(this.mysqlPasswordTextBox_MouseEnter);
            // 
            // mysqlUsernameTextBox
            // 
            this.mysqlUsernameTextBox.Location = new System.Drawing.Point(106, 66);
            this.mysqlUsernameTextBox.Name = "mysqlUsernameTextBox";
            this.mysqlUsernameTextBox.Size = new System.Drawing.Size(100, 21);
            this.mysqlUsernameTextBox.TabIndex = 8;
            this.mysqlUsernameTextBox.MouseEnter += new System.EventHandler(this.mysqlUsernameTextBox_MouseEnter);
            // 
            // mysqlDatabaseTextBox
            // 
            this.mysqlDatabaseTextBox.Location = new System.Drawing.Point(106, 42);
            this.mysqlDatabaseTextBox.Name = "mysqlDatabaseTextBox";
            this.mysqlDatabaseTextBox.Size = new System.Drawing.Size(100, 21);
            this.mysqlDatabaseTextBox.TabIndex = 7;
            this.mysqlDatabaseTextBox.MouseEnter += new System.EventHandler(this.mysqlDatabaseTextBox_MouseEnter);
            // 
            // mysqlHostTextBox
            // 
            this.mysqlHostTextBox.Location = new System.Drawing.Point(106, 18);
            this.mysqlHostTextBox.Name = "mysqlHostTextBox";
            this.mysqlHostTextBox.Size = new System.Drawing.Size(100, 21);
            this.mysqlHostTextBox.TabIndex = 6;
            this.mysqlHostTextBox.MouseEnter += new System.EventHandler(this.mysqlHostTextBox_MouseEnter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 93);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "DB Password:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "DB Username:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 3;
            this.label6.Text = "DB Database:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "DB Host:";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(276, 196);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 21);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            this.saveButton.MouseEnter += new System.EventHandler(this.saveButton_MouseEnter);
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(357, 196);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 21);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.button2_Click);
            this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 220);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(447, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(173, 17);
            this.toolStripStatusLabel1.Text = "Please configure your server";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.tbxPort3);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.tbxPort2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.tbxPort1);
            this.groupBox3.Location = new System.Drawing.Point(13, 142);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(413, 48);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Socket Setting";
            // 
            // tbxPort1
            // 
            this.tbxPort1.Location = new System.Drawing.Point(54, 20);
            this.tbxPort1.Name = "tbxPort1";
            this.tbxPort1.Size = new System.Drawing.Size(73, 21);
            this.tbxPort1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Port1:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(144, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "Port2:";
            // 
            // tbxPort2
            // 
            this.tbxPort2.Location = new System.Drawing.Point(188, 20);
            this.tbxPort2.Name = "tbxPort2";
            this.tbxPort2.Size = new System.Drawing.Size(73, 21);
            this.tbxPort2.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(278, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "Port3:";
            // 
            // tbxPort3
            // 
            this.tbxPort3.Location = new System.Drawing.Point(325, 20);
            this.tbxPort3.Name = "tbxPort3";
            this.tbxPort3.Size = new System.Drawing.Size(73, 21);
            this.tbxPort3.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 242);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "7Road Server Configuration";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.Button closeButton;
        public System.Windows.Forms.ComboBox serverTypeComboBox;
		public System.Windows.Forms.TextBox FullNameTextBox;
		public System.Windows.Forms.TextBox shortNameTextBox;
		public System.Windows.Forms.TextBox mysqlPasswordTextBox;
		public System.Windows.Forms.TextBox mysqlUsernameTextBox;
		public System.Windows.Forms.TextBox mysqlDatabaseTextBox;
        public System.Windows.Forms.TextBox mysqlHostTextBox;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox tbxPort1;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox tbxPort3;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox tbxPort2;
	}
}

