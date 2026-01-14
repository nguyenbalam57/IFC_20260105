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
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnRefreshPorts;

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
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            cmbBaudRate = new System.Windows.Forms.ComboBox();
            btnConnect = new System.Windows.Forms.Button();
            btnRefreshPorts = new System.Windows.Forms.Button();
            cmbPorts = new System.Windows.Forms.ComboBox();
            grpTransmit = new System.Windows.Forms.GroupBox();
            dtgDataBytes = new System.Windows.Forms.DataGridView();
            grpReceive = new System.Windows.Forms.GroupBox();
            btnData = new System.Windows.Forms.Button();
            btnClearReceive = new System.Windows.Forms.Button();
            lblLogStatus = new System.Windows.Forms.Label();
            lstCANMessages = new System.Windows.Forms.ListBox();
            btnStartLog = new System.Windows.Forms.Button();
            grpLog = new System.Windows.Forms.GroupBox();
            btnSendMessageLog = new System.Windows.Forms.Button();
            txtMessageLog = new System.Windows.Forms.TextBox();
            btnClearLog = new System.Windows.Forms.Button();
            txtSystemLog = new System.Windows.Forms.TextBox();
            statusStrip = new System.Windows.Forms.StatusStrip();
            lblRxCount = new System.Windows.Forms.ToolStripStatusLabel();
            lblTxCount = new System.Windows.Forms.ToolStripStatusLabel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            btnHideSystemLog = new System.Windows.Forms.Button();
            grpConnection.SuspendLayout();
            grpTransmit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dtgDataBytes).BeginInit();
            grpReceive.SuspendLayout();
            grpLog.SuspendLayout();
            statusStrip.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // grpConnection
            // 
            grpConnection.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpConnection.Controls.Add(label2);
            grpConnection.Controls.Add(label1);
            grpConnection.Controls.Add(cmbBaudRate);
            grpConnection.Controls.Add(btnConnect);
            grpConnection.Controls.Add(btnRefreshPorts);
            grpConnection.Controls.Add(cmbPorts);
            grpConnection.Location = new System.Drawing.Point(14, 4);
            grpConnection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpConnection.Name = "grpConnection";
            grpConnection.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpConnection.Size = new System.Drawing.Size(1109, 62);
            grpConnection.TabIndex = 0;
            grpConnection.TabStop = false;
            grpConnection.Text = "Kết nối UART";
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
            // cmbBaudRate
            // 
            cmbBaudRate.Location = new System.Drawing.Point(446, 24);
            cmbBaudRate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbBaudRate.Name = "cmbBaudRate";
            cmbBaudRate.Size = new System.Drawing.Size(174, 23);
            cmbBaudRate.TabIndex = 8;
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
            btnRefreshPorts.Text = "Làm mới";
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
            grpTransmit.Location = new System.Drawing.Point(13, 374);
            grpTransmit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpTransmit.Name = "grpTransmit";
            grpTransmit.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpTransmit.Size = new System.Drawing.Size(1109, 241);
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
            dtgDataBytes.Location = new System.Drawing.Point(0, 22);
            dtgDataBytes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dtgDataBytes.MultiSelect = false;
            dtgDataBytes.Name = "dtgDataBytes";
            dtgDataBytes.RowHeadersWidth = 20;
            dtgDataBytes.Size = new System.Drawing.Size(1102, 212);
            dtgDataBytes.TabIndex = 22;
            // 
            // grpReceive
            // 
            grpReceive.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpReceive.Controls.Add(btnData);
            grpReceive.Controls.Add(btnClearReceive);
            grpReceive.Controls.Add(lblLogStatus);
            grpReceive.Controls.Add(lstCANMessages);
            grpReceive.Controls.Add(btnStartLog);
            grpReceive.Location = new System.Drawing.Point(4, 3);
            grpReceive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpReceive.Name = "grpReceive";
            grpReceive.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.SetRowSpan(grpReceive, 2);
            grpReceive.Size = new System.Drawing.Size(725, 290);
            grpReceive.TabIndex = 2;
            grpReceive.TabStop = false;
            grpReceive.Text = "Nhận/Truyền CAN Log";
            // 
            // btnData
            // 
            btnData.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnData.Location = new System.Drawing.Point(135, 250);
            btnData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnData.Name = "btnData";
            btnData.Size = new System.Drawing.Size(117, 29);
            btnData.TabIndex = 2;
            btnData.Text = "Danh sách";
            btnData.UseVisualStyleBackColor = true;
            btnData.Click += btnData_Click;
            // 
            // btnClearReceive
            // 
            btnClearReceive.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnClearReceive.Location = new System.Drawing.Point(10, 250);
            btnClearReceive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnClearReceive.Name = "btnClearReceive";
            btnClearReceive.Size = new System.Drawing.Size(117, 29);
            btnClearReceive.TabIndex = 1;
            btnClearReceive.Text = "Xóa";
            btnClearReceive.UseVisualStyleBackColor = true;
            btnClearReceive.Click += btnClearReceive_Click;
            // 
            // lblLogStatus
            // 
            lblLogStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblLogStatus.AutoSize = true;
            lblLogStatus.ForeColor = System.Drawing.Color.Gray;
            lblLogStatus.Location = new System.Drawing.Point(408, 257);
            lblLogStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblLogStatus.Name = "lblLogStatus";
            lblLogStatus.Size = new System.Drawing.Size(82, 15);
            lblLogStatus.TabIndex = 1;
            lblLogStatus.Text = "Không ghi log";
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
            lstCANMessages.Size = new System.Drawing.Size(701, 214);
            lstCANMessages.TabIndex = 0;
            // 
            // btnStartLog
            // 
            btnStartLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnStartLog.Location = new System.Drawing.Point(260, 250);
            btnStartLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnStartLog.Name = "btnStartLog";
            btnStartLog.Size = new System.Drawing.Size(140, 29);
            btnStartLog.TabIndex = 0;
            btnStartLog.Text = "Bắt đầu Log";
            btnStartLog.UseVisualStyleBackColor = true;
            btnStartLog.Click += btnStartLog_Click;
            // 
            // grpLog
            // 
            grpLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpLog.Controls.Add(btnSendMessageLog);
            grpLog.Controls.Add(txtMessageLog);
            grpLog.Controls.Add(btnClearLog);
            grpLog.Controls.Add(txtSystemLog);
            grpLog.Location = new System.Drawing.Point(737, 3);
            grpLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpLog.Name = "grpLog";
            grpLog.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.SetRowSpan(grpLog, 2);
            grpLog.Size = new System.Drawing.Size(306, 290);
            grpLog.TabIndex = 3;
            grpLog.TabStop = false;
            grpLog.Text = "System Log";
            // 
            // btnSendMessageLog
            // 
            btnSendMessageLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnSendMessageLog.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnSendMessageLog.Location = new System.Drawing.Point(247, 19);
            btnSendMessageLog.Margin = new System.Windows.Forms.Padding(0);
            btnSendMessageLog.Name = "btnSendMessageLog";
            btnSendMessageLog.Size = new System.Drawing.Size(52, 23);
            btnSendMessageLog.TabIndex = 5;
            btnSendMessageLog.Text = "Send";
            btnSendMessageLog.UseVisualStyleBackColor = true;
            btnSendMessageLog.Click += btnSendMessageLog_Click;
            // 
            // txtMessageLog
            // 
            txtMessageLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtMessageLog.Font = new System.Drawing.Font("Segoe UI", 9F);
            txtMessageLog.Location = new System.Drawing.Point(12, 20);
            txtMessageLog.Name = "txtMessageLog";
            txtMessageLog.Size = new System.Drawing.Size(229, 23);
            txtMessageLog.TabIndex = 4;
            // 
            // btnClearLog
            // 
            btnClearLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnClearLog.Location = new System.Drawing.Point(12, 251);
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
            txtSystemLog.Location = new System.Drawing.Point(12, 49);
            txtSystemLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtSystemLog.Multiline = true;
            txtSystemLog.Name = "txtSystemLog";
            txtSystemLog.ReadOnly = true;
            txtSystemLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtSystemLog.Size = new System.Drawing.Size(282, 194);
            txtSystemLog.TabIndex = 2;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { lblRxCount, lblTxCount });
            statusStrip.Location = new System.Drawing.Point(0, 618);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip.Size = new System.Drawing.Size(1137, 22);
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
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            tableLayoutPanel1.Controls.Add(grpLog, 1, 0);
            tableLayoutPanel1.Controls.Add(grpReceive, 0, 0);
            tableLayoutPanel1.Controls.Add(btnHideSystemLog, 2, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(14, 72);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(1108, 296);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // btnHideSystemLog
            // 
            btnHideSystemLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btnHideSystemLog.Location = new System.Drawing.Point(1050, 3);
            btnHideSystemLog.Name = "btnHideSystemLog";
            btnHideSystemLog.Size = new System.Drawing.Size(55, 29);
            btnHideSystemLog.TabIndex = 4;
            btnHideSystemLog.Text = "Hide";
            btnHideSystemLog.UseVisualStyleBackColor = true;
            btnHideSystemLog.Click += btnHideSystemLog_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1137, 640);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(statusStrip);
            Controls.Add(grpTransmit);
            Controls.Add(grpConnection);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(1000, 600);
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "STM32 CAN Monitor - UART Interface";
            FormClosing += MainForm_FormClosing;
            grpConnection.ResumeLayout(false);
            grpConnection.PerformLayout();
            grpTransmit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dtgDataBytes).EndInit();
            grpReceive.ResumeLayout(false);
            grpReceive.PerformLayout();
            grpLog.ResumeLayout(false);
            grpLog.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dtgDataBytes;
        private System.Windows.Forms.Button btnData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnHideSystemLog;
        private System.Windows.Forms.Button btnSendMessageLog;
        private System.Windows.Forms.TextBox txtMessageLog;
    }
}

