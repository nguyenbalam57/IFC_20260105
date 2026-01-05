namespace IFC
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Connection Controls
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.ComboBox cmbPorts;
        private System.Windows.Forms.ComboBox cmbBaudRate;
        private System.Windows.Forms.ComboBox cmbCANBaudRate;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnRefreshPorts;
        private System.Windows.Forms.Button btnSetCANBaud;

        // Transmit Controls
        private System.Windows.Forms.GroupBox grpTransmit;

        // Receive Controls
        private System.Windows.Forms.GroupBox grpReceive;
        private System.Windows.Forms.ListBox lstCANMessages;
        private System.Windows.Forms.Button btnClearReceive;

        // Log Controls
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.Button btnStartLog;
        private System.Windows.Forms.Label lblLogStatus;
        private System.Windows.Forms.TextBox txtSystemLog;
        private System.Windows.Forms.Button btnClearLog;

        // Status Strip
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblRxCount;
        private System.Windows.Forms.ToolStripStatusLabel lblTxCount;

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
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.btnRefreshCANBaud = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSetCANBaud = new System.Windows.Forms.Button();
            this.cmbBaudRate = new System.Windows.Forms.ComboBox();
            this.cmbCANBaudRate = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnRefreshPorts = new System.Windows.Forms.Button();
            this.cmbPorts = new System.Windows.Forms.ComboBox();
            this.grpTransmit = new System.Windows.Forms.GroupBox();
            this.dtgDataBytes = new System.Windows.Forms.DataGridView();
            this.grpReceive = new System.Windows.Forms.GroupBox();
            this.btnClearReceive = new System.Windows.Forms.Button();
            this.lstCANMessages = new System.Windows.Forms.ListBox();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.txtSystemLog = new System.Windows.Forms.TextBox();
            this.lblLogStatus = new System.Windows.Forms.Label();
            this.btnStartLog = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblRxCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTxCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpConnection.SuspendLayout();
            this.grpTransmit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtgDataBytes)).BeginInit();
            this.grpReceive.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConnection.Controls.Add(this.btnRefreshCANBaud);
            this.grpConnection.Controls.Add(this.label3);
            this.grpConnection.Controls.Add(this.label2);
            this.grpConnection.Controls.Add(this.label1);
            this.grpConnection.Controls.Add(this.btnSetCANBaud);
            this.grpConnection.Controls.Add(this.cmbBaudRate);
            this.grpConnection.Controls.Add(this.cmbCANBaudRate);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.btnRefreshPorts);
            this.grpConnection.Controls.Add(this.cmbPorts);
            this.grpConnection.Location = new System.Drawing.Point(12, 12);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(1060, 63);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Kết nối UART";
            // 
            // btnRefreshCANBaud
            // 
            this.btnRefreshCANBaud.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshCANBaud.Location = new System.Drawing.Point(874, 19);
            this.btnRefreshCANBaud.Name = "btnRefreshCANBaud";
            this.btnRefreshCANBaud.Size = new System.Drawing.Size(85, 23);
            this.btnRefreshCANBaud.TabIndex = 12;
            this.btnRefreshCANBaud.Text = "Làm mới CAN";
            this.btnRefreshCANBaud.UseVisualStyleBackColor = true;
            this.btnRefreshCANBaud.Visible = false;
            this.btnRefreshCANBaud.Click += new System.EventHandler(this.btnRefreshCANBaud_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(652, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "CAN Baud:";
            this.label3.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(315, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Baud Rate:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "COM Port:";
            // 
            // btnSetCANBaud
            // 
            this.btnSetCANBaud.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetCANBaud.Location = new System.Drawing.Point(965, 19);
            this.btnSetCANBaud.Name = "btnSetCANBaud";
            this.btnSetCANBaud.Size = new System.Drawing.Size(85, 23);
            this.btnSetCANBaud.TabIndex = 4;
            this.btnSetCANBaud.Text = "Set CAN";
            this.btnSetCANBaud.UseVisualStyleBackColor = true;
            this.btnSetCANBaud.Visible = false;
            this.btnSetCANBaud.Click += new System.EventHandler(this.btnSetCANBaud_Click);
            // 
            // cmbBaudRate
            // 
            this.cmbBaudRate.Location = new System.Drawing.Point(382, 21);
            this.cmbBaudRate.Name = "cmbBaudRate";
            this.cmbBaudRate.Size = new System.Drawing.Size(150, 21);
            this.cmbBaudRate.TabIndex = 8;
            // 
            // cmbCANBaudRate
            // 
            this.cmbCANBaudRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCANBaudRate.FormattingEnabled = true;
            this.cmbCANBaudRate.Location = new System.Drawing.Point(718, 21);
            this.cmbCANBaudRate.Name = "cmbCANBaudRate";
            this.cmbCANBaudRate.Size = new System.Drawing.Size(150, 21);
            this.cmbCANBaudRate.TabIndex = 2;
            this.cmbCANBaudRate.Visible = false;
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.Green;
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(538, 18);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 25);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Kết nối";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnRefreshPorts
            // 
            this.btnRefreshPorts.Location = new System.Drawing.Point(224, 19);
            this.btnRefreshPorts.Name = "btnRefreshPorts";
            this.btnRefreshPorts.Size = new System.Drawing.Size(85, 23);
            this.btnRefreshPorts.TabIndex = 3;
            this.btnRefreshPorts.Text = "Làm mới COM";
            this.btnRefreshPorts.UseVisualStyleBackColor = true;
            this.btnRefreshPorts.Click += new System.EventHandler(this.btnRefreshPorts_Click);
            // 
            // cmbPorts
            // 
            this.cmbPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPorts.FormattingEnabled = true;
            this.cmbPorts.Location = new System.Drawing.Point(68, 21);
            this.cmbPorts.Name = "cmbPorts";
            this.cmbPorts.Size = new System.Drawing.Size(150, 21);
            this.cmbPorts.TabIndex = 0;
            // 
            // grpTransmit
            // 
            this.grpTransmit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTransmit.Controls.Add(this.dtgDataBytes);
            this.grpTransmit.Location = new System.Drawing.Point(12, 403);
            this.grpTransmit.Name = "grpTransmit";
            this.grpTransmit.Size = new System.Drawing.Size(1060, 142);
            this.grpTransmit.TabIndex = 1;
            this.grpTransmit.TabStop = false;
            this.grpTransmit.Text = "Truyền CAN";
            // 
            // dtgDataBytes
            // 
            this.dtgDataBytes.AllowUserToAddRows = false;
            this.dtgDataBytes.AllowUserToDeleteRows = false;
            this.dtgDataBytes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtgDataBytes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtgDataBytes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgDataBytes.EnableHeadersVisualStyles = false;
            this.dtgDataBytes.Location = new System.Drawing.Point(6, 19);
            this.dtgDataBytes.MultiSelect = false;
            this.dtgDataBytes.Name = "dtgDataBytes";
            this.dtgDataBytes.RowHeadersWidth = 20;
            this.dtgDataBytes.Size = new System.Drawing.Size(1048, 117);
            this.dtgDataBytes.TabIndex = 22;
            // 
            // grpReceive
            // 
            this.grpReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpReceive.Controls.Add(this.btnClearReceive);
            this.grpReceive.Controls.Add(this.lstCANMessages);
            this.grpReceive.Location = new System.Drawing.Point(12, 81);
            this.grpReceive.Name = "grpReceive";
            this.grpReceive.Size = new System.Drawing.Size(1060, 316);
            this.grpReceive.TabIndex = 2;
            this.grpReceive.TabStop = false;
            this.grpReceive.Text = "Nhận CAN";
            // 
            // btnClearReceive
            // 
            this.btnClearReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearReceive.Location = new System.Drawing.Point(9, 282);
            this.btnClearReceive.Name = "btnClearReceive";
            this.btnClearReceive.Size = new System.Drawing.Size(100, 25);
            this.btnClearReceive.TabIndex = 1;
            this.btnClearReceive.Text = "Xóa";
            this.btnClearReceive.UseVisualStyleBackColor = true;
            this.btnClearReceive.Click += new System.EventHandler(this.btnClearReceive_Click);
            // 
            // lstCANMessages
            // 
            this.lstCANMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstCANMessages.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstCANMessages.FormattingEnabled = true;
            this.lstCANMessages.ItemHeight = 14;
            this.lstCANMessages.Location = new System.Drawing.Point(10, 22);
            this.lstCANMessages.Name = "lstCANMessages";
            this.lstCANMessages.Size = new System.Drawing.Size(1040, 242);
            this.lstCANMessages.TabIndex = 0;
            // 
            // grpLog
            // 
            this.grpLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLog.Controls.Add(this.btnClearLog);
            this.grpLog.Controls.Add(this.txtSystemLog);
            this.grpLog.Controls.Add(this.lblLogStatus);
            this.grpLog.Controls.Add(this.btnStartLog);
            this.grpLog.Location = new System.Drawing.Point(12, 551);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(1060, 230);
            this.grpLog.TabIndex = 3;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "System Log";
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearLog.Location = new System.Drawing.Point(10, 196);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(100, 25);
            this.btnClearLog.TabIndex = 3;
            this.btnClearLog.Text = "Xóa Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // txtSystemLog
            // 
            this.txtSystemLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSystemLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSystemLog.Location = new System.Drawing.Point(10, 55);
            this.txtSystemLog.Multiline = true;
            this.txtSystemLog.Name = "txtSystemLog";
            this.txtSystemLog.ReadOnly = true;
            this.txtSystemLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSystemLog.Size = new System.Drawing.Size(1040, 135);
            this.txtSystemLog.TabIndex = 2;
            // 
            // lblLogStatus
            // 
            this.lblLogStatus.AutoSize = true;
            this.lblLogStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblLogStatus.Location = new System.Drawing.Point(140, 27);
            this.lblLogStatus.Name = "lblLogStatus";
            this.lblLogStatus.Size = new System.Drawing.Size(72, 13);
            this.lblLogStatus.TabIndex = 1;
            this.lblLogStatus.Text = "Không ghi log";
            // 
            // btnStartLog
            // 
            this.btnStartLog.Location = new System.Drawing.Point(10, 22);
            this.btnStartLog.Name = "btnStartLog";
            this.btnStartLog.Size = new System.Drawing.Size(120, 25);
            this.btnStartLog.TabIndex = 0;
            this.btnStartLog.Text = "Bắt đầu Log";
            this.btnStartLog.UseVisualStyleBackColor = true;
            this.btnStartLog.Click += new System.EventHandler(this.btnStartLog_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblRxCount,
            this.lblTxCount});
            this.statusStrip.Location = new System.Drawing.Point(0, 784);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1084, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblRxCount
            // 
            this.lblRxCount.Name = "lblRxCount";
            this.lblRxCount.Size = new System.Drawing.Size(33, 17);
            this.lblRxCount.Text = "RX: 0";
            // 
            // lblTxCount
            // 
            this.lblTxCount.Name = "lblTxCount";
            this.lblTxCount.Size = new System.Drawing.Size(33, 17);
            this.lblTxCount.Text = "TX: 0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 806);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpReceive);
            this.Controls.Add(this.grpTransmit);
            this.Controls.Add(this.grpConnection);
            this.MinimumSize = new System.Drawing.Size(1100, 800);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "STM32 CAN Monitor - UART Interface";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.grpTransmit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtgDataBytes)).EndInit();
            this.grpReceive.ResumeLayout(false);
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dtgDataBytes;
        private System.Windows.Forms.Button btnRefreshCANBaud;
    }
}

