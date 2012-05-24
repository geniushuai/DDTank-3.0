namespace Tools.SendUdp
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
            this.tbxServer = new System.Windows.Forms.TextBox();
            this.tbxPort = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxContent = new System.Windows.Forms.TextBox();
            this.txtRec = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbxServer
            // 
            this.tbxServer.Location = new System.Drawing.Point(60, 12);
            this.tbxServer.Name = "tbxServer";
            this.tbxServer.Size = new System.Drawing.Size(92, 21);
            this.tbxServer.TabIndex = 0;
            this.tbxServer.Text = "192.168.0.20";
            // 
            // tbxPort
            // 
            this.tbxPort.Location = new System.Drawing.Point(281, 10);
            this.tbxPort.Name = "tbxPort";
            this.tbxPort.Size = new System.Drawing.Size(113, 21);
            this.tbxPort.TabIndex = 1;
            this.tbxPort.Text = "9000";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(400, 10);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(83, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port";
            // 
            // tbxContent
            // 
            this.tbxContent.Location = new System.Drawing.Point(14, 41);
            this.tbxContent.Name = "tbxContent";
            this.tbxContent.Size = new System.Drawing.Size(469, 21);
            this.tbxContent.TabIndex = 5;
            // 
            // txtRec
            // 
            this.txtRec.Location = new System.Drawing.Point(13, 103);
            this.txtRec.Name = "txtRec";
            this.txtRec.Size = new System.Drawing.Size(469, 21);
            this.txtRec.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 227);
            this.Controls.Add(this.txtRec);
            this.Controls.Add(this.tbxContent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.tbxPort);
            this.Controls.Add(this.tbxServer);
            this.Name = "Form1";
            this.Text = "Send Udp";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxServer;
        private System.Windows.Forms.TextBox tbxPort;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxContent;
        private System.Windows.Forms.TextBox txtRec;
    }
}

