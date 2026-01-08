using IFC.Models;
using IFC.Serial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace IFC
{
    public partial class Form1 : Form
    {
        private SerialManager serialManager;
        private LogManager logManager;
        private System.Timers.Timer transmitTimer;
        private CANTransmitConfig txConfig;

        private int receivedCount = 0;
        private int transmittedCount = 0;

        private CANBaudRate calculatedCANBaudRate = null; // Lưu baud rate đã tính

        private CANStatus currentCANStatus;

        // Context Menu Strip
        private ContextMenuStrip contextMenuStrip;

        private CANBaudRateManager configCANBaud;

        private const int MAX_ROWS = 10;

        // Thêm Dictionary để track vị trí của mỗi CAN ID trong ListBox
        private Dictionary<uint, int> canIdListBoxIndexMap = new Dictionary<uint, int>();
        private bool isDisplayData = false;


        public Form1()
        {
            InitializeComponent();

            configCANBaud = new CANBaudRateManager();

            InitializeManagers();
            InitializeUI();

            InitialzeDataGridViewUI(dtgDataBytes);
        }

        private void InitializeManagers()
        {
            // Serial Manager
            serialManager = new SerialManager();
            serialManager.CANMessageReceived += OnCANMessageReceived;
            serialManager.LogMessageReceived += OnLogMessageReceived;
            serialManager.ConnectionChanged += OnConnectionChanged;

            // Log Manager
            logManager = new LogManager();

            // Transmit Timer
            transmitTimer = new System.Timers.Timer();
            //transmitTimer.Elapsed += TransmitTimer_Elapsed;
            transmitTimer.AutoReset = true;

            // TX Config
            txConfig = new CANTransmitConfig();

            serialManager.CANStatusReceived += OnCANStatusReceived;
        }

        private void InitializeUI()
        {
            // Load COM ports
            cmbPorts.Items.Clear();
            cmbPorts.Items.AddRange(SerialManager.GetAvailablePorts());
            if (cmbPorts.Items.Count > 0)
                cmbPorts.SelectedIndex = 0;

            // Baud rates
            cmbBaudRate.Items.AddRange(new object[] { 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 });
            cmbBaudRate.SelectedItem = 115200;

            RefreshCANBaud();

            UpdateConnectionStatus(false);

            // ⭐ Enable custom drawing cho ListBox (optional)
            InitializeListBoxDrawing();
        }

        /// <summary>
        /// Khởi tạo custom drawing cho ListBox để hiển thị màu Rx/Tx
        /// </summary>
        private void InitializeListBoxDrawing()
        {
            lstCANMessages.DrawMode = DrawMode.OwnerDrawFixed;
            lstCANMessages.DrawItem += LstCANMessages_DrawItem;
            updateDisplayButtonData();
        }

        /// <summary>
        /// Custom draw ListBox items với màu sắc cho Rx/Tx
        /// </summary>
        private void LstCANMessages_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            string text = lstCANMessages.Items[e.Index].ToString();
            Color textColor = Color.Black;
            Font textFont = e.Font;

            // Xác định màu dựa trên prefix
            if (text.Contains("[Rx]"))
            {
                textColor = Color.Green; // Rx = Màu xanh
            }
            else if (text.Contains("[Tx]"))
            {
                textColor = Color.Blue; // Tx = Màu xanh dương
            }

            // Vẽ text với màu tương ứng
            using (Brush textBrush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(text, textFont, textBrush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        // ===== Event Handlers =====

        /// <summary>
        /// kết nối với vi điều khiển thông qua baud rate UART
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!serialManager.IsConnected)
            {
                string port = cmbPorts.SelectedItem?.ToString();
                int baudRate = (int)cmbBaudRate.SelectedItem;

                if (string.IsNullOrEmpty(port))
                {
                    MessageBox.Show("Vui lòng chọn COM port!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (serialManager.Connect(port, baudRate))
                {
                    UpdateConnectionStatus(true);

                    // Delay rồi yêu cầu CAN status
                    System.Threading.Thread.Sleep(500);
                    serialManager.RequestCANStatus();
                }
            }
            else
            {
                serialManager.Disconnect();
                transmitTimer.Stop();
                UpdateConnectionStatus(false);
            }
        }
        private void btnRefreshPorts_Click(object sender, EventArgs e)
        {
            cmbPorts.Items.Clear();
            cmbPorts.Items.AddRange(SerialManager.GetAvailablePorts());
            if (cmbPorts.Items.Count > 0)
                cmbPorts.SelectedIndex = 0;
        }

        private void btnRefreshCANBaud_Click(object sender, EventArgs e)
        {
            RefreshCANBaud();
        }

        private void RefreshCANBaud()
        {
            // CAN Baud Rate ComboBox
            //cmbCANBaudRate.Items.Clear();
            cmbCANBaudRate.DataSource = configCANBaud.LoadBaudRates();
            cmbCANBaudRate.DisplayMember = "Name";
            cmbCANBaudRate.SelectedItem = configCANBaud.GetBaudRateByName("500 kbps");

        }

        /// <summary>
        /// Đẩy CAN baud rate xuống vi điều khiển, xét lại baud rate và trả về lại kết quả
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetCANBaud_Click(object sender, EventArgs e)
        {
            if (!serialManager.IsConnected)
            {
                MessageBox.Show("Chưa kết nối!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CANBaudRate selectedBaudRate = cmbCANBaudRate.SelectedItem as CANBaudRate;

            if (selectedBaudRate != null)
            {

                DialogResult result = MessageBox.Show(
                    $"Thay đổi CAN Baud Rate sang {selectedBaudRate.Name}?\n\n" +
                    $"Prescaler: {selectedBaudRate.Prescaler}\n" +
                    $"TimeSeg1: {selectedBaudRate.TimeSeg1}\n" +
                    $"TimeSeg2: {selectedBaudRate.TimeSeg2}\n" +
                    $"SJW: {selectedBaudRate.SyncJumpWidth}\n\n" +
                    $"Vi điều khiển sẽ reset CAN peripheral! ",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    if (serialManager.SetCANBaudRate(selectedBaudRate))
                    {
                        // Đợi STM32 cấu hình xong
                        System.Threading.Thread.Sleep(1000);

                        // Yêu cầu status mới
                        serialManager.RequestCANStatus();
                    }
                }
            }
        }

        private void OnCANStatusReceived(object sender, CANStatus status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ProcessCANStatus(status)));
            }
            else
            {
                ProcessCANStatus(status);
            }
        }

        private void ProcessCANStatus(CANStatus status)
        {
            currentCANStatus = status;

            if (status.IsInitialized)
            {

                // Cập nhật ComboBox
                if (status.CurrentBaudRate != null)
                {
                    // Tìm và chọn baud rate phù hợp
                    for (int i = 0; i < cmbCANBaudRate.Items.Count; i++)
                    {
                        CANBaudRate item = cmbCANBaudRate.Items[i] as CANBaudRate;
                        if (item != null &&
                            item.Prescaler == status.CurrentBaudRate.Prescaler &&
                            item.TimeSeg1 == status.CurrentBaudRate.TimeSeg1)
                        {
                            cmbCANBaudRate.SelectedIndex = i;
                            break;
                        }
                    }
                }

                AppendSystemLog($"CAN Status: {status}");
            }
            else
            {

            }
        }

        private void TransmitTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            serialManager.SendCANMessage(txConfig);
            transmittedCount++;

            if (InvokeRequired)
                Invoke(new Action(UpdateStatistics));
            else
                UpdateStatistics();
        }

        private void btnStartLog_Click(object sender, EventArgs e)
        {
            try
            {
                if (!logManager.IsLogging)
                {
                    logManager.StartLogging();
                    btnStartLog.Text = "Dừng Log";
                    btnStartLog.BackColor = Color.Red;
                    lblLogStatus.Text = $"Đang ghi: {logManager.GetCurrentLogFile()}";
                    lblLogStatus.ForeColor = Color.Green;
                }
                else
                {
                    logManager.StopLogging();
                    btnStartLog.Text = "Bắt đầu Log";
                    btnStartLog.BackColor = SystemColors.Control;
                    lblLogStatus.Text = "Không ghi log";
                    lblLogStatus.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearReceive_Click(object sender, EventArgs e)
        {
            lstCANMessages.Items.Clear();
            canIdListBoxIndexMap.Clear(); // ⭐ Clear map khi clear ListBox
            receivedCount = 0;
            transmittedCount = 0; // ⭐ Reset cả Tx counter
            UpdateStatistics();
        }

        private void btnData_Click(object sender, EventArgs e)
        {
            isDisplayData = !isDisplayData;

            updateDisplayButtonData();

            // Clear ListBox và map khi chuyển mode
            lstCANMessages.Items.Clear();
            canIdListBoxIndexMap.Clear();

            AppendSystemLog($"Chuyển sang chế độ hiển thị: {(isDisplayData ? "Cập nhật theo CAN ID" : "Danh sách tách biệt")}");
        }

        private void updateDisplayButtonData()
        {
            // Cập nhật text button để hiển thị trạng thái
            btnData.Text = isDisplayData ? "Mode: Update" : "Mode: List";
            btnData.BackColor = isDisplayData ? Color.LightBlue : SystemColors.Control;
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtSystemLog.Clear();
        }

        private void OnCANMessageReceived(object sender, CANMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ProcessCANMessage(message)));
            }
            else
            {
                ProcessCANMessage(message);
            }
        }

        private void ProcessCANMessage(CANMessage message)
        {

            // Xử lý thông điệp CAN
            if(serialManager.GetAutoTxRowIndexById($"{message.CANId:X}", out int existingIndex))
            {
                message.IsRxTx = false; // Đánh dấu là Tx
            }
            else
            {
                message.IsRxTx = true; // Đánh dấu là Rx
            }

            // Tạo prefix Rx/Tx với màu sắc
            string rxTxPrefix = message.IsRxTx ? "[Rx]" : "[Tx]";
            string displayText = $"[{message.Timestamp:HH:mm:ss.fff}] {rxTxPrefix} {message}";

            if (isDisplayData)
            {
                // ⭐ Mode: Cập nhật cùng 1 dòng cho mỗi CAN ID
                UpdateOrAddCANMessageInListBox(message, displayText);
            }
            else
            {
                // ⭐ Mode: Hiển thị danh sách tách biệt (như cũ)
                lstCANMessages.Items.Insert(0, displayText);

                // Giới hạn số dòng hiển thị
                if (lstCANMessages.Items.Count > 1000)
                    lstCANMessages.Items.RemoveAt(lstCANMessages.Items.Count - 1);
            }

            // Ghi log
            if (logManager.IsLogging)
                logManager.LogCANMessage(message);

            // Cập nhật counter
            if (message.IsRxTx)
                receivedCount++;
            else
                transmittedCount++;

            UpdateStatistics();
        }

        /// <summary>
        /// Cập nhật hoặc thêm CAN message vào ListBox (1 dòng cho mỗi CAN ID)
        /// </summary>
        private void UpdateOrAddCANMessageInListBox(CANMessage message, string displayText)
        {
            try
            {
                uint canId = message.CANId;

                // Kiểm tra CAN ID đã tồn tại trong map chưa
                if (canIdListBoxIndexMap.ContainsKey(canId))
                {
                    int existingIndex = canIdListBoxIndexMap[canId];

                    // Kiểm tra index còn hợp lệ không
                    if (existingIndex >= 0 && existingIndex < lstCANMessages.Items.Count)
                    {
                        // Cập nhật dòng hiện tại
                        lstCANMessages.Items[existingIndex] = displayText;

                        // Highlight dòng vừa cập nhật (optional)
                        lstCANMessages.SelectedIndex = existingIndex;
                    }
                    else
                    {
                        // Index không hợp lệ - Thêm mới và cập nhật map
                        int newIndex = lstCANMessages.Items.Add(displayText);
                        canIdListBoxIndexMap[canId] = newIndex;
                    }
                }
                else
                {
                    // CAN ID chưa tồn tại - Thêm mới
                    int newIndex = lstCANMessages.Items.Add(displayText);
                    canIdListBoxIndexMap[canId] = newIndex;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating ListBox: {ex.Message}");

                // Fallback: Thêm mới nếu có lỗi
                lstCANMessages.Items.Insert(0, displayText);
            }
        }

        private void OnLogMessageReceived(object sender, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendSystemLog(message)));
            }
            else
            {
                AppendSystemLog(message);
            }
        }

        private void AppendSystemLog(string message)
        {
            txtSystemLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtSystemLog.SelectionStart = txtSystemLog.Text.Length;
            txtSystemLog.ScrollToCaret();
        }

        private void OnConnectionChanged(object sender, bool isConnected)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateConnectionStatus(isConnected)));
            }
            else
            {
                UpdateConnectionStatus(isConnected);
            }
        }

        private void UpdateConnectionStatus(bool isConnected)
        {
            if (isConnected)
            {
                btnConnect.Text = "Ngắt kết nối";
                btnConnect.BackColor = Color.Red;

                cmbPorts.Enabled = false;
                cmbBaudRate.Enabled = false;
                btnRefreshPorts.Enabled = false;
            }
            else
            {
                btnConnect.Text = "Kết nối";
                btnConnect.BackColor = Color.Green;

                cmbPorts.Enabled = true;
                cmbBaudRate.Enabled = true;
                btnRefreshPorts.Enabled = true;
            }
        }

        private void UpdateStatistics()
        {
            lblRxCount.Text = $"RX: {receivedCount}";
            lblTxCount.Text = $"TX: {transmittedCount}";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            transmitTimer?.Stop();
            logManager?.StopLogging();
            serialManager?.Disconnect();

            // ⭐ Gửi lệnh AUTOTX:CLEAR
            serialManager.AutoTx_Clear();
        }


        #region DataGrdiView

        private void InitialzeDataGridViewUI(DataGridView dataGridView)
        {
            dataGridView.Columns.Clear();
            dataGridView.Rows.Clear();

            // Thêm cột Data Frame
            DataGridViewComboBoxColumn cbbDataFrame = new DataGridViewComboBoxColumn
            {
                Name = "Frame",
                HeaderText = "Data Frame",
                Width = 150,
            };
            cbbDataFrame.Items.AddRange(new string[] { "Standard", "Extended" });
            dataGridView.Columns.Add(cbbDataFrame);

            // Thêm cột Send Button
            DataGridViewButtonColumn btnSend = new DataGridViewButtonColumn
            {
                Name = "SendButton",
                HeaderText = "Action",
                Text = "▶",
                UseColumnTextForButtonValue = false,
                Width = 100,
            };
            dataGridView.Columns.Add(btnSend);

            // Thêm cột Mode
            DataGridViewComboBoxColumn cbbMode = new DataGridViewComboBoxColumn
            {
                Name = "Mode",
                HeaderText = "Mode",
                Width = 150,
            };
            cbbMode.Items.AddRange(new string[] { "Single", "Multi" });
            dataGridView.Columns.Add(cbbMode);

            // Thêm cột Interval
            DataGridViewTextBoxColumn colInterval = new DataGridViewTextBoxColumn
            {
                Name = "Interval",
                HeaderText = "Interval (ms)",
                Width = 100,
            };
            dataGridView.Columns.Add(colInterval);

            // Thêm cột CAN ID
            DataGridViewTextBoxColumn colCANID = new DataGridViewTextBoxColumn
            {
                Name = "CANID",
                HeaderText = "CAN ID",
                Width = 100,
            };
            dataGridView.Columns.Add(colCANID);

            // Thêm cột DLC
            DataGridViewTextBoxColumn colDLC = new DataGridViewTextBoxColumn
            {
                Name = "DLC",
                HeaderText = "DLC",
                Width = 60,
            };
            dataGridView.Columns.Add(colDLC);

            // Đăng ký events
            RegisterDataGridViewEvents(dataGridView);

            // Thêm 1 row để nhập dữ liệu
            dtgDataUISendData(dataGridView);

        }

        /// <summary>
        /// Đăng ký events cho DataGridView
        /// </summary>
        private void RegisterDataGridViewEvents(DataGridView dataGridView)
        {
            dataGridView.CellValidating += DtgData_CellValidating;
            dataGridView.CellEndEdit += DtgData_CellEndEdit;
            dataGridView.CellValueChanged += DtgData_CellValueChanged;
            dataGridView.CurrentCellDirtyStateChanged += DtgData_CurrentCellDirtyStateChanged;
            dataGridView.EditingControlShowing += DtgData_EditingControlShowing;
            dataGridView.CellContentClick += DtgData_CellContentClick;

            dataGridView.MouseClick += DtgData_MouseClick;

            // Tạo Context Menu
            CreateContextMenu(dataGridView);
        }


        /// <summary>
        /// Xử lý khi giá trị cell thay đổi (cho ComboBox)
        /// </summary>
        private void DtgData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            // Commit ngay khi thay đổi ComboBox
            if (dgv.CurrentCell is DataGridViewComboBoxCell)
            {
                dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Xử lý sau khi giá trị cell thay đổi
        /// </summary>
        private void DtgData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridView dgv = sender as DataGridView;
            string columnName = dgv.Columns[e.ColumnIndex].Name;

            if (columnName == "Frame")
            {
                // Xử lý thay đổi Data Frame
                HandleFrameChanged(dgv, e.RowIndex);
            }
            else if (columnName == "Mode")
            {
                // Xử lý thay đổi Mode
                HandleModeChanged(dgv, e.RowIndex);
            }
            else if (columnName == "CANID")
            {
                // Check CAN có duplicate không
                CheckDuplicateCANID(dgv, e.RowIndex);
            }
            else if (columnName == "DLC")
            {
                // Cập nhật Data columns
                UpdateDataColumns(dgv);
            }
            else if (columnName == "Interval")
            {
                // Xử lý thay đổi data
                HandleDataChanged(dgv, e.RowIndex);

            }
            else if (columnName.StartsWith("Data"))
            {
                // Xử lý thay đổi data0
                HandleDataChanged(dgv, e.RowIndex);
            }
        }

        /// <summary>
        /// Xử lý khi Data Frame thay đổi
        /// </summary>
        private void HandleFrameChanged(DataGridView dgv, int rowIndex)
        {
            if (dgv.Rows[rowIndex].Cells["Frame"].Value == null) return;

            string frameType = dgv.Rows[rowIndex].Cells["Frame"].Value.ToString();
            DataGridViewCell canIdCell = dgv.Rows[rowIndex].Cells["CANID"];
            DataGridViewCell dlcCell = dgv.Rows[rowIndex].Cells["DLC"];

            if (frameType == "Standard")
            {
                // Standard: CAN ID max 3 ký tự (0x7FF), DLC max 8
                canIdCell.Value = "000";

                // Nếu DLC hiện tại > 8, reset về 8
                if (dlcCell.Value != null && int.TryParse(dlcCell.Value.ToString(), out int currentDLC))
                {
                    if (currentDLC > 8)
                    {
                        dlcCell.Value = 8;
                    }
                }
                else
                {
                    dlcCell.Value = 8;
                }
            }
            else // Extended
            {
                // Extended: CAN ID max 8 ký tự (0x1FFFFFFF), DLC max 64
                canIdCell.Value = "00000000";

                // Giữ nguyên DLC hoặc set mặc định
                if (dlcCell.Value == null)
                {
                    dlcCell.Value = 8;
                }
            }

            // Cập nhật Data columns
            UpdateDataColumns(dgv);
        }

        /// <summary>
        /// Xử lý khi Mode thay đổi
        /// </summary>
        private void HandleModeChanged(DataGridView dgv, int rowIndex)
        {
            if (dgv.Rows[rowIndex].Cells["Mode"].Value == null) return;

            string mode = dgv.Rows[rowIndex].Cells["Mode"].Value.ToString();
            DataGridViewCell intervalCell = dgv.Rows[rowIndex].Cells["Interval"];

            if (mode == "Single")
            {
                // Single mode - Disable Interval
                intervalCell.ReadOnly = true;
                intervalCell.Style.BackColor = Color.LightGray;
                intervalCell.Style.ForeColor = Color.DarkGray;
                intervalCell.Value = "0";
            }
            else // Multi
            {
                // Multi mode - Enable Interval
                intervalCell.ReadOnly = false;
                intervalCell.Style.BackColor = Color.White;
                intervalCell.Style.ForeColor = Color.Black;

                // Set giá trị mặc định nếu chưa có
                if (string.IsNullOrEmpty(intervalCell.Value?.ToString()))
                {
                    intervalCell.Value = "10";
                }
            }
        }

        /// <summary>
        /// Xử lý click vào button cell
        /// </summary>
        private void DtgData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridView dgv = sender as DataGridView;

            if (dgv.Columns[e.ColumnIndex].Name == "SendButton")
            {
                // Xử lý click Send button
                HandleSendButtonClick(dgv, e.RowIndex);
            }
        }

        /// <summary>
        /// Xử lý khi click chuột vào DataGridView
        /// </summary>
        private void DtgData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView dgv = sender as DataGridView;

                // Lấy vị trí click
                DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);

                if (hit.RowIndex >= 0)
                {
                    // Click vào row, chọn row đó
                    dgv.ClearSelection();
                    dgv.Rows[hit.RowIndex].Selected = true;
                    dgv.CurrentCell = dgv.Rows[hit.RowIndex].Cells[0];
                }

                // Hiển thị context menu
                contextMenuStrip.Show(dgv, e.Location);
            }
        }


        /// <summary>
        /// Xử lý khi click Send button
        /// </summary>
        private void HandleSendButtonClick(
            DataGridView dgv,
            int rowIndex,
            bool isUpdate = false,
            bool isStop = false)
        {
            DataGridViewRow row = dgv.Rows[rowIndex];

            // Lấy thông tin từ row
            string frame = row.Cells["Frame"].Value?.ToString() ?? "Standard";
            string mode = row.Cells["Mode"].Value?.ToString() ?? "Single";
            string canId = row.Cells["CANID"].Value?.ToString() ?? "";
            string dlcStr = row.Cells["DLC"].Value?.ToString() ?? "0";
            string interval = row.Cells["Interval"].Value?.ToString() ?? "10";

            DataGridViewButtonCell buttonCell = row.Cells["SendButton"] as DataGridViewButtonCell;
            string currentButtonText = buttonCell.Value?.ToString() ?? "▶";

            // Validate
            if (string.IsNullOrEmpty(canId))
            {
                MessageBox.Show("Vui lòng nhập CAN ID!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!serialManager.GetAutoTxRowIndexById(canId, out int rowIndexOriginal, true))
            {
                MessageBox.Show("CAN ID không hợp lệ!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(dlcStr, out int dlc) || dlc <= 0 || dlc > 8)
            {
                MessageBox.Show("DLC không hợp lệ (1-8)!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy data bytes
            byte[] dataBytes = new byte[dlc];
            for (int i = 0; i < dlc; i++)
            {
                string colName = $"Data{i}";
                if (dgv.Columns.Contains(colName))
                {
                    string hexValue = row.Cells[colName].Value?.ToString() ?? "00";
                    if (byte.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out byte byteValue))
                    {
                        dataBytes[i] = byteValue;
                    }
                }
            }

            // Hiển thị thông tin (hoặc gửi CAN data thực tế)
            string dataStr = BitConverter.ToString(dataBytes).Replace("-", " ");
            string message = $"Frame: {frame}\n" +
                             $"Mode: {mode}\n" +
                             $"Intervel {interval}" +
                             $"CAN ID:  0x{canId}\n" +
                             $"DLC: {dlc}\n" +
                             $"Data: {dataStr}";

            uint canIdInt = Convert.ToUInt32(canId, 16);
            bool isExtended = frame == "Extended";
            int intervalInt = string.IsNullOrWhiteSpace(interval) ? 0 : int.Parse(interval);

            CANTransmitConfig config = new CANTransmitConfig
            {
                CANId = canIdInt,
                Data = dataBytes,
                DLC = dlc,
                IntervalMs = intervalInt,
                IsExtended = isExtended,

            };

            // ⭐ Sử dụng AUTOTX commands
            if ((mode == "Multi" && config.IntervalMs > 0))
            {
                if (currentButtonText == "▶" || isUpdate)
                {
                    // ⭐ ADD hoặc UPDATE
                    // Xét gửi liên tục
                    config.IsEnabled = true;

                    bool success = false;

                    if (isUpdate)
                    {
                        // Update existing
                        success = serialManager.AutoTx_Update(rowIndexOriginal, config);
                    }
                    else
                    {
                        // Add new
                        success = serialManager.AutoTx_Add(rowIndexOriginal, config);
                    }

                    if (success)
                    {
                        // Bắt đầu gửi liên tục
                        buttonCell.Value = "⏹";
                        row.Cells["Frame"].Style.BackColor = Color.LightGray;
                        row.Cells["Frame"].ReadOnly = true;
                        row.Cells["CANID"].Style.BackColor = Color.LightGray;
                        row.Cells["CANID"].ReadOnly = true;
                        row.Cells["Mode"].Style.BackColor = Color.LightGray;
                        row.Cells["Mode"].ReadOnly = true;
                        row.Cells["DLC"].Style.BackColor = Color.LightGray;
                        row.Cells["DLC"].ReadOnly = true;
                        dgv.InvalidateCell(buttonCell);

                        string msg = isUpdate ? "✅ Cập nhật gửi liên tục..." : "✅ Bắt đầu gửi liên tục...  ";
                        AppendSystemLog($"[Row {rowIndexOriginal}] {msg}");

                    }
                }
                else if (currentButtonText == "⏹" || isStop) // currentButtonText == "⏹"
                {
                    // Dừng gửi
                    if (serialManager.AutoTx_Disable(rowIndexOriginal))
                    {
                        buttonCell.Value = "▶";
                        row.Cells["Frame"].Style.BackColor = Color.White;
                        row.Cells["Frame"].ReadOnly = false;
                        row.Cells["CANID"].Style.BackColor = Color.White;
                        row.Cells["CANID"].ReadOnly = false;
                        row.Cells["Mode"].Style.BackColor = Color.White;
                        row.Cells["Mode"].ReadOnly = false;
                        row.Cells["DLC"].Style.BackColor = Color.White;
                        row.Cells["DLC"].ReadOnly = false;
                        dgv.InvalidateCell(buttonCell);
                        AppendSystemLog($"[Row {rowIndex}] ⏹ Đã dừng gửi");
                    }
                }
            }
            else // Single
            {
                // ⭐ Single - Gửi 1 lần (dùng TX command cũ)
                config.IsEnabled = false;
                if (serialManager.SendCANMessage(config))
                {
                    AppendSystemLog($"[Row {rowIndex}] ✅ Gửi 1 lần");
                }
            }

#if DEBUG
            //MessageBox.Show(message, "CAN Send", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
        }

        // Kiểm tra CANID khi data thay đổi, có bị trùng không, nếu có thì bắt buộc nhập lại CANID
        private void CheckDuplicateCANID(DataGridView dgv, int rowIndex)
        {
            try
            {
                if (rowIndex < 0 || rowIndex >= dgv.Rows.Count)
                    return;

                DataGridViewRow row = dgv.Rows[rowIndex];
                string canId = row.Cells["CANID"].Value?.ToString() ?? "";

                if (string.IsNullOrWhiteSpace(canId))
                    return;

                // Kiểm tra trùng lặp
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    if (i != rowIndex && dgv.Rows[i].Cells["CANID"].Value?.ToString() == canId)
                    {
                        MessageBox.Show($"CAN ID {canId} đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        row.Cells["CANID"].Value = ""; // Yêu cầu nhập lại
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Row {rowIndex}] Error in CheckDuplicateCANID: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật khi data thay đổi
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="rowIndex"></param>
        private void HandleDataChanged(DataGridView dgv, int rowIndex)
        {
            try
            {
                if (rowIndex < 0 || rowIndex >= dgv.Rows.Count)
                    return;
                int rowIndexOriginal = rowIndex;
                DataGridViewRow row = dgv.Rows[rowIndex];

                // Lấy CAN ID
                string canId = row.Cells["CANID"].Value?.ToString() ?? "";

                if (!uint.TryParse(canId, out uint uintCanId))
                {
                    Debug.WriteLine($"[Row {rowIndex}] Invalid or empty CAN ID");
                    return;
                }

                // Kiểm tra CAN ID hợp lệ
                if (string.IsNullOrWhiteSpace(canId))
                {
                    Debug.WriteLine($"[Row {rowIndex}] Invalid or empty CAN ID");
                    return;
                }

                // Parse CAN ID
                if (!uint.TryParse(canId, System.Globalization.NumberStyles.HexNumber, null, out uint canIdUint))
                {
                    Debug.WriteLine($"[Row {rowIndex}] Cannot parse CAN ID: {canId}");
                    return;
                }

                // Tìm config
                if (!serialManager.GetAutoTxRowIndexById(canId, out rowIndexOriginal))
                {
                    Debug.WriteLine($"[Row {rowIndex}] No Auto TX config found for index {canId}");
                    return;
                }

                var selectConfig = serialManager.GetCANTransmitAutoConfig(rowIndexOriginal);

                if (selectConfig != null)
                {
                    Debug.WriteLine($"[Row {rowIndex}] Found config for CAN ID 0x{canIdUint: X}");

                    if (selectConfig.IsEnabled)
                    {
                        Debug.WriteLine($"[Row {rowIndex}] Config is enabled, triggering send");
                        HandleSendButtonClick(dgv, rowIndex, isUpdate: true);
                    }
                    else
                    {
                        Debug.WriteLine($"[Row {rowIndex}] Config is disabled");
                    }
                }
                else
                {
                    Debug.WriteLine($"[Row {rowIndex}] No config found for CAN ID 0x{canIdUint:X}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Row {rowIndex}] Error in HandleDataChanged: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }

        }

        /// <summary>
        /// Event khi bắt đầu edit cell - Xử lý KeyPress
        /// </summary>
        private void DtgData_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Xóa event cũ để tránh đăng ký nhiều lần
            e.Control.KeyPress -= new KeyPressEventHandler(DtgData_KeyPress);

            DataGridView dgv = sender as DataGridView;

            if (dgv.CurrentCell.OwningColumn.Name == "DLC" ||
                dgv.CurrentCell.OwningColumn.Name == "Interval")
            {
                // DLC và Interval cell - chỉ cho phép nhập số
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += DtgData_KeyPress;
                }
            }
            else if (dgv.CurrentCell.OwningColumn.Name == "CANID")
            {
                // CANID cell - chỉ cho phép nhập Hex (0-9, A-F)
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += DtgData_KeyPress;
                }
            }
            else if (dgv.CurrentCell.OwningColumn.Name.StartsWith("Data"))
            {
                // Data cells - chỉ cho phép nhập Hex (0-9, A-F)
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += DtgData_KeyPress;
                }
            }
        }

        /// <summary>
        /// Xử lý KeyPress cho các cell
        /// </summary>
        private void DtgData_KeyPress(object sender, KeyPressEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv == null || dgv.CurrentCell == null) return;

            string columnName = dgv.CurrentCell.OwningColumn.Name;

            if (columnName == "DLC" || columnName == "Interval")
            {
                // DLC và Interval:  Chỉ cho phép nhập số 0-9 và Backspace
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            else if (columnName == "CANID" || columnName.StartsWith("Data"))
            {
                // CANID và Data: Chỉ cho phép Hex (0-9, A-F, a-f) và Backspace
                if (!char.IsControl(e.KeyChar) && !Uri.IsHexDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
                else if (Uri.IsHexDigit(e.KeyChar))
                {
                    // Tự động chuyển sang chữ hoa
                    e.KeyChar = char.ToUpper(e.KeyChar);
                }
            }
        }

        /// <summary>
        /// Validate cell trước khi kết thúc edit
        /// </summary>
        private void DtgData_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            string columnName = dgv.Columns[e.ColumnIndex].Name;
            string value = e.FormattedValue?.ToString() ?? "";

            if (columnName == "DLC")
            {
                // Validate DLC
                if (!string.IsNullOrEmpty(value))
                {
                    if (int.TryParse(value, out int dlcValue))
                    {
                        // Lấy Frame type từ row hiện tại
                        string frameType = dgv.Rows[e.RowIndex].Cells["Frame"].Value?.ToString() ?? "Standard";
                        int minDLC = 0;
                        int maxDLC = frameType == "Standard" ? 8 : 8;

                        if (dlcValue < minDLC || dlcValue > maxDLC)
                        {
                            e.Cancel = true;
                            MessageBox.Show($"DLC ({frameType}) phải trong khoảng {minDLC} - {maxDLC}",
                                            "Invalid DLC",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                        MessageBox.Show("DLC phải là số! ",
                                        "Invalid DLC",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                    }
                }
            }
            else if (columnName == "Interval")
            {
                // Validate Interval
                if (!string.IsNullOrEmpty(value))
                {
                    if (int.TryParse(value, out int intervalValue))
                    {
                        if (intervalValue < 1 || intervalValue > 10000)
                        {
                            e.Cancel = true;
                            MessageBox.Show("Interval phải trong khoảng 1 - 10000 ms",
                                            "Invalid Interval",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                        MessageBox.Show("Interval phải là số! ",
                                        "Invalid Interval",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                    }
                }
            }
            else if (columnName == "CANID")
            {
                // Validate CANID
                if (!string.IsNullOrEmpty(value))
                {
                    // Kiểm tra có phải Hex không
                    if (!IsHexString(value))
                    {
                        e.Cancel = true;
                        MessageBox.Show("CAN ID chỉ được nhập Hex (0-9, A-F)",
                                        "Invalid CAN ID",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                    }
                    else
                    {
                        // Lấy Frame type từ row hiện tại
                        string frameType = dgv.Rows[e.RowIndex].Cells["Frame"].Value?.ToString() ?? "Standard";
                        int maxLength = frameType == "Standard" ? 3 : 8;

                        if (value.Length > maxLength)
                        {
                            e.Cancel = true;
                            MessageBox.Show($"CAN ID ({frameType}) tối đa {maxLength} ký tự",
                                            "Invalid CAN ID",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        }

                        // Kiểm tra giá trị max
                        if (frameType == "Standard")
                        {
                            // Standard: max 0x7FF
                            if (int.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out int canId))
                            {
                                if (canId > 0x7FF)
                                {
                                    e.Cancel = true;
                                    MessageBox.Show("CAN ID Standard tối đa là 0x7FF (2047)",
                                                    "Invalid CAN ID",
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Warning);
                                }
                            }
                        }
                        else
                        {
                            // Extended:  max 0x1FFFFFFF
                            if (long.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out long canId))
                            {
                                if (canId > 0x1FFFFFFF)
                                {
                                    e.Cancel = true;
                                    MessageBox.Show("CAN ID Extended tối đa là 0x1FFFFFFF",
                                                    "Invalid CAN ID",
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Warning);
                                }
                            }
                        }
                    }
                }
            }
            else if (columnName.StartsWith("Data"))
            {
                // Validate Data cells
                if (!string.IsNullOrEmpty(value))
                {
                    if (!IsHexString(value))
                    {
                        e.Cancel = true;
                        MessageBox.Show("Data chỉ được nhập Hex (0-9, A-F)",
                                        "Invalid Data",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                    }
                    else if (value.Length > 2)
                    {
                        e.Cancel = true;
                        MessageBox.Show("Data tối đa 2 ký tự (00-FF)",
                                        "Invalid Data",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                    }
                    else
                    {
                        // Kiểm tra giá trị 0-255 (0x00-0xFF)
                        if (int.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out int dataValue))
                        {
                            if (dataValue > 0xFF)
                            {
                                e.Cancel = true;
                                MessageBox.Show("Data tối đa là 0xFF (255)",
                                                "Invalid Data",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Xử lý sau khi kết thúc edit cell
        /// </summary>
        private void DtgData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            string columnName = dgv.Columns[e.ColumnIndex].Name;
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string value = cell.Value?.ToString() ?? "";

            if (columnName == "DLC")
            {
                // Format DLC
                if (string.IsNullOrEmpty(value))
                {
                    cell.Value = "0"; // Set về 0 nếu trống
                }
                else
                {
                    if (int.TryParse(value, out int dlcValue))
                    {
                        cell.Value = dlcValue.ToString();

                        // Cập nhật hiển thị Data columns
                        UpdateDataColumns(dgv);
                    }
                }
            }
            else if (columnName == "Interval")
            {
                // Format Interval
                if (string.IsNullOrEmpty(value))
                {
                    cell.Value = "10"; // Set mặc định 100ms
                }
            }
            else if (columnName == "CANID")
            {
                // Format CANID - Uppercase và pad left
                if (!string.IsNullOrEmpty(value))
                {
                    string frameType = dgv.Rows[e.RowIndex].Cells["Frame"].Value?.ToString() ?? "Standard";
                    int padLength = frameType == "Standard" ? 3 : 8;
                    cell.Value = value.ToUpper().PadLeft(padLength, '0');
                }
                else
                {
                    // Set giá trị mặc định
                    string frameType = dgv.Rows[e.RowIndex].Cells["Frame"].Value?.ToString() ?? "Standard";
                    cell.Value = frameType == "Standard" ? "000" : "00000000";
                }
            }
            else if (columnName.StartsWith("Data"))
            {
                // Format Data - Uppercase và pad left với 2 ký tự
                if (!string.IsNullOrEmpty(value))
                {
                    cell.Value = value.ToUpper().PadLeft(2, '0');
                }
                else
                {
                    cell.Value = "00";
                }
            }
        }

        /// <summary>
        /// Cập nhật dữ liệu CAN ID và DLC cho row
        /// </summary>
        private void UpdateRowData(DataGridView dataGridView, int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < dataGridView.Rows.Count)
            {
                dataGridView.Rows[rowIndex].Cells["Frame"].Value = "Standard";

                dataGridView.Rows[rowIndex].Cells["SendButton"].Value = "▶";

                dataGridView.Rows[rowIndex].Cells["Mode"].Value = "Single";

                // Interval ban đầu là readonly vì Mode = Single
                dataGridView.Rows[rowIndex].Cells["Interval"].Value = "";
                dataGridView.Rows[rowIndex].Cells["Interval"].ReadOnly = true;
                dataGridView.Rows[rowIndex].Cells["Interval"].Style.BackColor = Color.LightGray;
                dataGridView.Rows[rowIndex].Cells["Interval"].Style.ForeColor = Color.DarkGray;

                // Set CAN ID
                dataGridView.Rows[rowIndex].Cells["CANID"].Value = "000";

                // Set DLC
                dataGridView.Rows[rowIndex].Cells["DLC"].Value = 8;
            }
        }

        /// <summary>
        /// Cập nhật các cột Data theo DLC
        /// </summary>
        private void UpdateDataColumns(DataGridView dataGridView)
        {

            if (dataGridView == null || dataGridView.Rows.Count == 0)
                return;

            try
            {

                // Kết thúc edit mode
                if (dataGridView.IsCurrentCellInEditMode)
                {
                    dataGridView.EndEdit();
                }

                // Tạm dừng layout để tránh flicker
                dataGridView.SuspendLayout();

                int dlcMax = 0;
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells["DLC"].Value != null && int.TryParse(row.Cells["DLC"].Value.ToString(), out int dlcValue))
                    {
                        dlcMax = Math.Max(dlcMax, dlcValue);
                    }
                }

                if (dlcMax > 0)
                {
                    int countDataCols = dataGridView.Columns.Cast<DataGridViewColumn>()
                                                            .Count(c => c.Name.StartsWith("Data"));

                    if (countDataCols < dlcMax)
                    {
                        // Thêm cột Data
                        for (int i = countDataCols; i < dlcMax; i++)
                        {
                            DataGridViewTextBoxColumn colData = new DataGridViewTextBoxColumn
                            {
                                Name = $"Data{i}",
                                HeaderText = $"Data {i}",
                                Width = 70,
                                MaxInputLength = 2
                            };
                            dataGridView.Columns.Add(colData);
                        }
                    }
                    else if (countDataCols > dlcMax)
                    {
                        // Xóa bớt cột Data
                        // Tạo danh sách tên cột cần xóa
                        List<string> columnsToRemove = new List<string>();

                        foreach (DataGridViewColumn col in dataGridView.Columns)
                        {
                            if (col.Name.StartsWith("Data"))
                            {
                                string indexStr = col.Name.Substring(4);
                                if (int.TryParse(indexStr, out int dataIndex))
                                {
                                    if (dataIndex >= dlcMax)
                                    {
                                        columnsToRemove.Add(col.Name);
                                    }
                                }
                            }
                        }

                        // Xóa các cột theo tên
                        foreach (string colName in columnsToRemove)
                        {
                            if (dataGridView.Columns.Contains(colName))
                            {
                                dataGridView.Columns.Remove(colName);
                            }
                        }
                    }

                    dtgDataGridViewDLCUpdateData(dataGridView);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating columns: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Resume layout
                dataGridView.ResumeLayout();
            }
        }

        /// <summary>
        /// Update nội dung DLC trong datagridview để visible đúng số cột Data
        /// </summary>
        private void dtgDataGridViewDLCUpdateData(DataGridView dataGridView)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["DLC"].Value != null && int.TryParse(row.Cells["DLC"].Value.ToString(), out int dlcValue))
                {
                    // Lấy tổng số cột Data
                    int totalDataCols = dataGridView.Columns.Cast<DataGridViewColumn>()
                        .Count(col => col.Name.StartsWith("Data"));

                    // Cập nhật từng cột Data
                    for (int i = 0; i < totalDataCols; i++)
                    {
                        string colName = $"Data{i}";
                        if (dataGridView.Columns.Contains(colName))
                        {
                            DataGridViewCell cell = row.Cells[colName];

                            if (i < dlcValue)
                            {
                                // Enable cell
                                cell.ReadOnly = false;
                                cell.Style.BackColor = Color.White;
                                cell.Style.ForeColor = Color.Black;

                                // Set default value
                                if (cell.Value == null || string.IsNullOrEmpty(cell.Value.ToString()))
                                {
                                    cell.Value = "00";
                                }
                            }
                            else
                            {
                                // Disable cell
                                cell.ReadOnly = true;
                                cell.Style.BackColor = Color.LightGray;
                                cell.Style.ForeColor = Color.DarkGray;
                                cell.Value = "";
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Thêm rows dữ liệu vào datagridview
        /// </summary>
        private void dtgDataUISendData(DataGridView dataGridView)
        {
            if (dataGridView != null)
            {
                // Nếu chưa có row nào, thêm 1 row
                if (dataGridView.Rows.Count == 0)
                {
                    int rowIndex = dataGridView.Rows.Add();
                    UpdateRowData(dataGridView, rowIndex);
                }
                else
                {
                    // Cập nhật dữ liệu cho row hiện tại
                    UpdateRowData(dataGridView, 0);
                }

                UpdateDataColumns(dataGridView);
            }
        }

        /// <summary>
        /// Kiểm tra chuỗi có phải Hex không
        /// </summary>
        private bool IsHexString(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;

            foreach (char c in str)
            {
                if (!Uri.IsHexDigit(c))
                    return false;
            }
            return true;
        }

        #region ContextMenuStripDatagridView

        /// <summary>
        /// Tạo Context Menu cho DataGridView
        /// </summary>
        private void CreateContextMenu(DataGridView dataGridView)
        {
            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Font = new Font("Segoe UI", 10F);

            // Menu Item:  Add Row
            ToolStripMenuItem addRowMenuItem = new ToolStripMenuItem
            {
                Text = "Add Row",
                ShortcutKeys = Keys.Control | Keys.N,
                Image = SystemIcons.Application.ToBitmap() // Hoặc dùng icon resources
            };
            addRowMenuItem.Click += (sender, e) => AddNewRow(dataGridView);
            contextMenuStrip.Items.Add(addRowMenuItem);

            // Menu Item:  Duplicate Row
            ToolStripMenuItem duplicateRowMenuItem = new ToolStripMenuItem
            {
                Text = "Duplicate Row",
                ShortcutKeys = Keys.Control | Keys.D
            };
            duplicateRowMenuItem.Click += (sender, e) => DuplicateSelectedRow(dataGridView);
            contextMenuStrip.Items.Add(duplicateRowMenuItem);

            // Separator
            contextMenuStrip.Items.Add(new ToolStripSeparator());

            // Menu Item: Remove Row
            ToolStripMenuItem removeRowMenuItem = new ToolStripMenuItem
            {
                Text = "Remove Row",
                ShortcutKeys = Keys.Delete,
                ForeColor = Color.DarkRed
            };
            removeRowMenuItem.Click += (sender, e) => RemoveSelectedRow(dataGridView);
            contextMenuStrip.Items.Add(removeRowMenuItem);

            // Menu Item: Remove All Rows
            ToolStripMenuItem removeAllMenuItem = new ToolStripMenuItem
            {
                Text = "Remove All Rows",
                ForeColor = Color.Red
            };
            removeAllMenuItem.Click += (sender, e) => RemoveAllRows(dataGridView);
            contextMenuStrip.Items.Add(removeAllMenuItem);

            // Gán Context Menu cho DataGridView
            dataGridView.ContextMenuStrip = contextMenuStrip;
        }

        /// <summary>
        /// Thêm row mới
        /// </summary>
        private void AddNewRow(DataGridView dataGridView)
        {
            try
            {
                // Kết thúc edit mode nếu đang edit
                if (dataGridView.IsCurrentCellInEditMode)
                {
                    dataGridView.EndEdit();
                }

                if (dataGridView.Rows.Count >= MAX_ROWS)
                {
                    MessageBox.Show($"Không thể thêm row mới.  Đã đạt giới hạn tối đa {MAX_ROWS} rows.",
                                    "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Thêm row mới
                int newRowIndex = dataGridView.Rows.Add();

                // Cập nhật dữ liệu mặc định cho row mới
                UpdateRowData(dataGridView, newRowIndex);

                // Focus vào row mới
                dataGridView.CurrentCell = dataGridView.Rows[newRowIndex].Cells["CANID"];

                // Cập nhật Data columns nếu cần
                UpdateDataColumns(dataGridView);

                Debug.WriteLine($"Added new row at index {newRowIndex}.  Total rows: {dataGridView.Rows.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm row: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Duplicate row được chọn
        /// </summary>
        private void DuplicateSelectedRow(DataGridView dataGridView)
        {
            try
            {
                if (dataGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn row cần duplicate!", "Warning",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dataGridView.Rows.Count >= MAX_ROWS)
                {
                    MessageBox.Show($"Không thể thêm row mới.  Đã đạt giới hạn tối đa {MAX_ROWS} rows.",
                                    "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int sourceRowIndex = dataGridView.SelectedRows[0].Index;
                DataGridViewRow sourceRow = dataGridView.Rows[sourceRowIndex];

                // Thêm row mới
                int newRowIndex = dataGridView.Rows.Add();
                DataGridViewRow newRow = dataGridView.Rows[newRowIndex];

                // Copy dữ liệu từ source row
                foreach (DataGridViewCell cell in sourceRow.Cells)
                {
                    if (cell.OwningColumn.Name != "SendButton") // Không copy button
                    {
                        newRow.Cells[cell.ColumnIndex].Value = cell.Value;
                    }
                    else
                    {
                        newRow.Cells[cell.ColumnIndex].Value = "▶"; // Reset button
                    }
                }

                // Focus vào row mới
                dataGridView.CurrentCell = newRow.Cells["CANID"];

                Debug.WriteLine($"Duplicated row {sourceRowIndex} to {newRowIndex}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi duplicate row: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xóa row được chọn
        /// </summary>
        private void RemoveSelectedRow(DataGridView dataGridView)
        {
            try
            {
                int rowCounts = dataGridView.SelectedRows.Count;
                if (rowCounts == 0)
                {
                    MessageBox.Show("Vui lòng chọn row cần xóa!", "Warning",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy row index cần xóa
                int rowIndex = dataGridView.SelectedRows[0].Index;

                uint canId = Convert.ToUInt32(dataGridView.Rows[rowIndex].Cells["CANID"].Value?.ToString(), 16);

                // Confirm trước khi xóa
                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa row #{rowIndex + 1}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    /// ⭐ Gửi lệnh AUTOTX:REMOVE
                    serialManager.AutoTx_Remove(rowIndex);

                    // Xóa row
                    dataGridView.Rows.RemoveAt(rowIndex);

                    Debug.WriteLine($"Removed row at index {rowIndex}. Remaining rows: {dataGridView.Rows.Count}");

                    // Nếu không còn row nào, khởi tạo lại
                    if (dataGridView.Rows.Count == 0)
                    {
                        MessageBox.Show("Không còn row nào.  Đang khởi tạo lại DataGridView.. .",
                                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        InitialzeDataGridViewUI(dataGridView);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa row: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xóa tất cả rows
        /// </summary>
        private void RemoveAllRows(DataGridView dataGridView)
        {
            try
            {
                if (dataGridView.Rows.Count == 0)
                {
                    MessageBox.Show("Không có row nào để xóa!", "Warning",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirm trước khi xóa tất cả
                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa TẤT CẢ {dataGridView.Rows.Count} rows?",
                    "Confirm Delete All",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // ⭐ Gửi lệnh AUTOTX:CLEAR
                    serialManager.AutoTx_Clear();

                    // Xóa tất cả rows
                    dataGridView.Rows.Clear();

                    Debug.WriteLine("Removed all rows. Reinitializing DataGridView...");

                    // Khởi tạo lại DataGridView
                    MessageBox.Show("Đã xóa tất cả rows.  Đang khởi tạo lại.. .",
                                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    InitialzeDataGridViewUI(dataGridView);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa tất cả rows: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion



        #endregion

        
    }
}
