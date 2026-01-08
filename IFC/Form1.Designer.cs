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
            grpConnection = new System.Windows.Forms.GroupBox();
            btnRefreshCANBaud = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            btnSetCANBaud = new System.Windows.Forms.Button();
            cmbBaudRate = new System.Windows.Forms.ComboBox();
            cmbCANBaudRate = new System.Windows.Forms.ComboBox();
            btnConnect = new System.Windows.Forms.Button();
            btnRefreshPorts = new System.Windows.Forms.Button();
            cmbPorts = new System.Windows.Forms.ComboBox();
            grpTransmit = new System.Windows.Forms.GroupBox();
            dtgDataBytes = new System.Windows.Forms.DataGridView();
            grpReceive = new System.Windows.Forms.GroupBox();
            btnData = new System.Windows.Forms.Button();
            btnClearReceive = new System.Windows.Forms.Button();
            lstCANMessages = new System.Windows.Forms.ListBox();
            grpLog = new System.Windows.Forms.GroupBox();
            btnClearLog = new System.Windows.Forms.Button();
            txtSystemLog = new System.Windows.Forms.TextBox();
            lblLogStatus = new System.Windows.Forms.Label();
            btnStartLog = new System.Windows.Forms.Button();
            statusStrip = new System.Windows.Forms.StatusStrip();
            lblRxCount = new System.Windows.Forms.ToolStripStatusLabel();
            lblTxCount = new System.Windows.Forms.ToolStripStatusLabel();
            grpConnection.SuspendLayout();
            grpTransmit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dtgDataBytes).BeginInit();
            grpReceive.SuspendLayout();
            grpLog.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // grpConnection
            // 
            grpConnection.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpConnection.Controls.Add(btnRefreshCANBaud);
            grpConnection.Controls.Add(label3);
            grpConnection.Controls.Add(label2);
            grpConnection.Controls.Add(label1);
            grpConnection.Controls.Add(btnSetCANBaud);
            grpConnection.Controls.Add(cmbBaudRate);
            grpConnection.Controls.Add(cmbCANBaudRate);
            grpConnection.Controls.Add(btnConnect);
            grpConnection.Controls.Add(btnRefreshPorts);
            grpConnection.Controls.Add(cmbPorts);
            grpConnection.Location = new System.Drawing.Point(14, 14);
            grpConnection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpConnection.Name = "grpConnection";
            grpConnection.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpConnection.Size = new System.Drawing.Size(1236, 73);
            grpConnection.TabIndex = 0;
            grpConnection.TabStop = false;
            grpConnection.Text = "Kết nối UART";
            // 
            // btnRefreshCANBaud
            // 
            btnRefreshCANBaud.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRefreshCANBaud.Location = new System.Drawing.Point(1019, 22);
            btnRefreshCANBaud.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRefreshCANBaud.Name = "btnRefreshCANBaud";
            btnRefreshCANBaud.Size = new System.Drawing.Size(99, 27);
            btnRefreshCANBaud.TabIndex = 12;
            btnRefreshCANBaud.Text = "Làm mới CAN";
            btnRefreshCANBaud.UseVisualStyleBackColor = true;
            btnRefreshCANBaud.Visible = false;
            btnRefreshCANBaud.Click += btnRefreshCANBaud_Click;
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(760, 28);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(65, 15);
            label3.TabIndex = 11;
            label3.Text = "CAN Baud:";
            label3.Visible = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(368, 28);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(63, 15);
            label2.TabIndex = 10;
            label2.Text = "Baud Rate:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 28);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(63, 15);
            label1.TabIndex = 9;
            label1.Text = "COM Port:";
            // 
            // btnSetCANBaud
            // 
            btnSetCANBaud.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnSetCANBaud.Location = new System.Drawing.Point(1125, 22);
            btnSetCANBaud.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSetCANBaud.Name = "btnSetCANBaud";
            btnSetCANBaud.Size = new System.Drawing.Size(99, 27);
            btnSetCANBaud.TabIndex = 4;
            btnSetCANBaud.Text = "Set CAN";
            btnSetCANBaud.UseVisualStyleBackColor = true;
            btnSetCANBaud.Visible = false;
            btnSetCANBaud.Click += btnSetCANBaud_Click;
            // 
            // cmbBaudRate
            // 
            cmbBaudRate.Location = new System.Drawing.Point(446, 24);
            cmbBaudRate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbBaudRate.Name = "cmbBaudRate";
            cmbBaudRate.Size = new System.Drawing.Size(174, 23);
            cmbBaudRate.TabIndex = 8;
            // 
            // cmbCANBaudRate
            // 
            cmbCANBaudRate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cmbCANBaudRate.FormattingEnabled = true;
            cmbCANBaudRate.Location = new System.Drawing.Point(837, 24);
            cmbCANBaudRate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbCANBaudRate.Name = "cmbCANBaudRate";
            cmbCANBaudRate.Size = new System.Drawing.Size(174, 23);
            cmbCANBaudRate.TabIndex = 2;
            cmbCANBaudRate.Visible = false;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = System.Drawing.Color.Green;
            btnConnect.ForeColor = System.Drawing.Color.White;
            btnConnect.Location = new System.Drawing.Point(628, 21);
            btnConnect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new System.Drawing.Size(117, 29);
            btnConnect.TabIndex = 5;
            btnConnect.Text = "Kết nối";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnRefreshPorts
            // 
            btnRefreshPorts.Location = new System.Drawing.Point(261, 22);
            btnRefreshPorts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRefreshPorts.Name = "btnRefreshPorts";
            btnRefreshPorts.Size = new System.Drawing.Size(99, 27);
            btnRefreshPorts.TabIndex = 3;
            btnRefreshPorts.Text = "Làm mới COM";
            btnRefreshPorts.UseVisualStyleBackColor = true;
            btnRefreshPorts.Click += btnRefreshPorts_Click;
            // 
            // cmbPorts
            // 
            cmbPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbPorts.FormattingEnabled = true;
            cmbPorts.Location = new System.Drawing.Point(79, 24);
            cmbPorts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbPorts.Name = "cmbPorts";
            cmbPorts.Size = new System.Drawing.Size(174, 23);
            cmbPorts.TabIndex = 0;
            // 
            // grpTransmit
            // 
            grpTransmit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpTransmit.Controls.Add(dtgDataBytes);
            grpTransmit.Location = new System.Drawing.Point(14, 296);
            grpTransmit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpTransmit.Name = "grpTransmit";
            grpTransmit.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpTransmit.Size = new System.Drawing.Size(1236, 164);
            grpTransmit.TabIndex = 1;
            grpTransmit.TabStop = false;
            grpTransmit.Text = "Truyền CAN";
            // 
            // dtgDataBytes
            // 
            dtgDataBytes.AllowUserToAddRows = false;
            dtgDataBytes.AllowUserToDeleteRows = false;
            dtgDataBytes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dtgDataBytes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dtgDataBytes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dtgDataBytes.EnableHeadersVisualStyles = false;
            dtgDataBytes.Location = new System.Drawing.Point(7, 22);
            dtgDataBytes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dtgDataBytes.MultiSelect = false;
            dtgDataBytes.Name = "dtgDataBytes";
            dtgDataBytes.RowHeadersWidth = 20;
            dtgDataBytes.Size = new System.Drawing.Size(1222, 135);
            dtgDataBytes.TabIndex = 22;
            // 
            // grpReceive
            // 
            grpReceive.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpReceive.Controls.Add(btnData);
            grpReceive.Controls.Add(btnClearReceive);
            grpReceive.Controls.Add(lstCANMessages);
            grpReceive.Location = new System.Drawing.Point(14, 93);
            grpReceive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpReceive.Name = "grpReceive";
            grpReceive.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpReceive.Size = new System.Drawing.Size(1236, 196);
            grpReceive.TabIndex = 2;
            grpReceive.TabStop = false;
            grpReceive.Text = "Nhận CAN";
            // 
            // btnData
            // 
            btnData.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnData.Location = new System.Drawing.Point(135, 156);
            btnData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnData.Name = "btnData";
            btnData.Size = new System.Drawing.Size(117, 29);
            btnData.TabIndex = 2;
            btnData.Text = "Dữ liệu";
            btnData.UseVisualStyleBackColor = true;
            btnData.Click += btnData_Click;
            // 
            // btnClearReceive
            // 
            btnClearReceive.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnClearReceive.Location = new System.Drawing.Point(10, 156);
            btnClearReceive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnClearReceive.Name = "btnClearReceive";
            btnClearReceive.Size = new System.Drawing.Size(117, 29);
            btnClearReceive.TabIndex = 1;
            btnClearReceive.Text = "Xóa";
            btnClearReceive.UseVisualStyleBackColor = true;
            btnClearReceive.Click += btnClearReceive_Click;
            // 
            // lstCANMessages
            // 
            lstCANMessages.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lstCANMessages.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lstCANMessages.FormattingEnabled = true;
            lstCANMessages.ItemHeight = 14;
            lstCANMessages.Location = new System.Drawing.Point(12, 25);
            lstCANMessages.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lstCANMessages.Name = "lstCANMessages";
            lstCANMessages.Size = new System.Drawing.Size(1212, 130);
            lstCANMessages.TabIndex = 0;
            // 
            // grpLog
            // 
            grpLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpLog.Controls.Add(btnClearLog);
            grpLog.Controls.Add(txtSystemLog);
            grpLog.Controls.Add(lblLogStatus);
            grpLog.Controls.Add(btnStartLog);
            grpLog.Location = new System.Drawing.Point(14, 467);
            grpLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpLog.Name = "grpLog";
            grpLog.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpLog.Size = new System.Drawing.Size(1236, 265);
            grpLog.TabIndex = 3;
            grpLog.TabStop = false;
            grpLog.Text = "System Log";
            // 
            // btnClearLog
            // 
            btnClearLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnClearLog.Location = new System.Drawing.Point(12, 226);
            btnClearLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new System.Drawing.Size(117, 29);
            btnClearLog.TabIndex = 3;
            btnClearLog.Text = "Xóa Log";
            btnClearLog.UseVisualStyleBackColor = true;
            btnClearLog.Click += btnClearLog_Click;
            // 
            // txtSystemLog
            // 
            txtSystemLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtSystemLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtSystemLog.Location = new System.Drawing.Point(12, 63);
            txtSystemLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtSystemLog.Multiline = true;
            txtSystemLog.Name = "txtSystemLog";
            txtSystemLog.ReadOnly = true;
            txtSystemLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtSystemLog.Size = new System.Drawing.Size(1212, 155);
            txtSystemLog.TabIndex = 2;
            // 
            // lblLogStatus
            // 
            lblLogStatus.AutoSize = true;
            lblLogStatus.ForeColor = System.Drawing.Color.Gray;
            lblLogStatus.Location = new System.Drawing.Point(163, 31);
            lblLogStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblLogStatus.Name = "lblLogStatus";
            lblLogStatus.Size = new System.Drawing.Size(82, 15);
            lblLogStatus.TabIndex = 1;
            lblLogStatus.Text = "Không ghi log";
            // 
            // btnStartLog
            // 
            btnStartLog.Location = new System.Drawing.Point(12, 25);
            btnStartLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnStartLog.Name = "btnStartLog";
            btnStartLog.Size = new System.Drawing.Size(140, 29);
            btnStartLog.TabIndex = 0;
            btnStartLog.Text = "Bắt đầu Log";
            btnStartLog.UseVisualStyleBackColor = true;
            btnStartLog.Click += btnStartLog_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { lblRxCount, lblTxCount });
            statusStrip.Location = new System.Drawing.Point(0, 739);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip.Size = new System.Drawing.Size(1264, 22);
            statusStrip.TabIndex = 4;
            statusStrip.Text = "statusStrip1";
            // 
            // lblRxCount
            // 
            lblRxCount.Name = "lblRxCount";
            lblRxCount.Size = new System.Drawing.Size(33, 17);
            lblRxCount.Text = "RX: 0";
            // 
            // lblTxCount
            // 
            lblTxCount.Name = "lblTxCount";
            lblTxCount.Size = new System.Drawing.Size(33, 17);
            lblTxCount.Text = "TX: 0";
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1264, 761);
            Controls.Add(statusStrip);
            Controls.Add(grpLog);
            Controls.Add(grpReceive);
            Controls.Add(grpTransmit);
            Controls.Add(grpConnection);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(1280, 800);
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "STM32 CAN Monitor - UART Interface";
            FormClosing += MainForm_FormClosing;
            grpConnection.ResumeLayout(false);
            grpConnection.PerformLayout();
            grpTransmit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dtgDataBytes).EndInit();
            grpReceive.ResumeLayout(false);
            grpLog.ResumeLayout(false);
            grpLog.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dtgDataBytes;
        private System.Windows.Forms.Button btnRefreshCANBaud;
        private System.Windows.Forms.Button btnData;
    }
}

