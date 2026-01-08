using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFC.Models
{
    // ⭐ Event Args mới
    public class AutoTxStatusEventArgs : EventArgs
    {
        public string RawData { get; set; }
    }

    public class SerialManager : IDisposable
    {
        private SerialPort serialPort;
        private Thread readThread;
        private bool isRunning;

        public event EventHandler<CANMessage> CANMessageReceived;
        public event EventHandler<string> LogMessageReceived;
        public event EventHandler<bool> ConnectionChanged;

        public bool IsConnected => serialPort?.IsOpen ?? false;

        public event EventHandler<CANStatus> CANStatusReceived;

        private CANBaudRateManager configCANBaud;

        public List<CANTransmitConfig> cANTransmits;

        // ⭐ Thêm event mới cho AUTOTX Status
        public event EventHandler<AutoTxStatusEventArgs> AutoTxStatusReceived;

        // ⭐ Thêm Dictionary để quản lý Auto TX messages
        private CANTransmitConfig[] autoTxConfigs = new CANTransmitConfig[10]; // Key = Index (0-9)

        public SerialManager()
        {
            serialPort = new SerialPort();
            configCANBaud = new CANBaudRateManager();
            cANTransmits = new List<CANTransmitConfig>();

            ClearCANTransmitConfigs();
        }

        public void ClearCANTransmitConfigs()
        {
            // Khởi tạo autoTxConfigs
            for (int i = 0; i < 10; i++)
            {
                autoTxConfigs[i] = new CANTransmitConfig();
            }
        }

        /// <summary>
        /// Kết nối với UART (COM)
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public bool Connect(string portName, int baudRate)
        {
            try
            {
                if (serialPort.IsOpen)
                    Disconnect();

                serialPort.PortName = portName;
                serialPort.BaudRate = baudRate;
                serialPort.DataBits = 8;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.Handshake = Handshake.None;
                serialPort.ReadTimeout = 500;
                serialPort.WriteTimeout = 500;

                serialPort.Open();

                // Bắt đầu thread đọc dữ liệu
                isRunning = true;
                readThread = new Thread(ReadDataThread);
                readThread.IsBackground = true;
                readThread.Start();

                ConnectionChanged?.Invoke(this, true);
                LogMessageReceived?.Invoke(this, $"Đã kết nối {portName} @ {baudRate} baud");

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"Lỗi kết nối: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ngắt kết nối với UART (COM)
        /// </summary>
        public void Disconnect()
        {
            try
            {
                isRunning = false;

                if (readThread != null && readThread.IsAlive)
                    readThread.Join(1000);

                if (serialPort.IsOpen)
                    serialPort.Close();

                ConnectionChanged?.Invoke(this, false);
                LogMessageReceived?.Invoke(this, "Đã ngắt kết nối");
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"Lỗi ngắt kết nối: {ex.Message}");
            }
        }

        #region Nhận dữ liệu từ Serial Port

        /// <summary>
        /// Đọc data từ serial port trong một thread riêng
        /// </summary>
        private void ReadDataThread()
        {
            byte[] buffer = new byte[1024];
            int bufferIndex = 0;

            while (isRunning && serialPort.IsOpen)
            {
                try
                {
                    if (serialPort.BytesToRead > 0)
                    {
                        int bytesRead = serialPort.Read(buffer, bufferIndex, buffer.Length - bufferIndex);
                        bufferIndex += bytesRead;

                        // Tìm ký tự kết thúc frame (ví dụ: '\n')
                        int newlineIndex;
                        while ((newlineIndex = Array.IndexOf(buffer, (byte)'\n', 0, bufferIndex)) >= 0)
                        {
                            // Xử lý frame
                            byte[] frame = new byte[newlineIndex];
                            Array.Copy(buffer, frame, newlineIndex);
                            ProcessReceivedFrame(frame);

                            // Dịch chuyển buffer
                            int remaining = bufferIndex - newlineIndex - 1;
                            if (remaining > 0)
                                Array.Copy(buffer, newlineIndex + 1, buffer, 0, remaining);
                            bufferIndex = remaining;
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (TimeoutException)
                {
                    // Timeout bình thường, tiếp tục
                }
                catch (Exception ex)
                {
                    if (isRunning)
                        LogMessageReceived?.Invoke(this, $"Lỗi đọc dữ liệu: {ex.Message}");
                }
            }
        }

        // Cập nhật hàm ProcessReceivedFrame
        private void ProcessReceivedFrame(byte[] frame)
        {
            try
            {
                string frameStr = Encoding.ASCII.GetString(frame).Trim();

                if (frameStr.StartsWith("CAN:"))
                {
                    ParseCANMessage(frameStr);
                }
                else if (frameStr.StartsWith("LOG:"))
                {
                    LogMessageReceived?.Invoke(this, frameStr.Substring(4));
                }
                else if (frameStr.StartsWith("STATUS:"))
                {
                    ParseCANStatus(frameStr);
                }
                else if (frameStr.StartsWith("AUTOTX:") || frameStr.StartsWith("========"))
                {
                    // ⭐ Xử lý AUTOTX status response
                    ParseAutoTxStatus(frameStr);
                }
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"Lỗi parse frame: {ex.Message}");
            }
        }

        /// <summary>
        /// bắm tin nhắn CAN từ dữ liệu nhận được
        /// </summary>
        /// <param name="data"></param>
        private void ParseCANMessage(string data)
        {
            // Format: "CAN:ID:DLC:D0:D1:D2:D3:D4:D5:D6:D7"
            string[] parts = data.Split(':');

            if (parts.Length >= 3)
            {
                CANMessage msg = new CANMessage();

                // Parse CAN ID (hex)
                msg.CANId = Convert.ToUInt32(parts[1], 16);

                // Parse DLC
                msg.DLC = int.Parse(parts[2]);

                // Parse Data bytes
                int dataStartIndex = 3;
                for (int i = 0; i < msg.DLC && (dataStartIndex + i) < parts.Length; i++)
                {
                    msg.Data[i] = Convert.ToByte(parts[dataStartIndex + i], 16);
                }

                if (msg.CANId > 0x7FF)
                    msg.IsExtended = true;

                msg.Timestamp = DateTime.Now;
                CANMessageReceived?.Invoke(this, msg);
            }
        }

        /// <summary>
        /// Parse CAN Status từ STM32
        /// Format: "STATUS:CAN:INIT:prescaler:timeseg1:timeseg2:sjw:rxerr:txerr"
        /// </summary>
        private void ParseCANStatus(string data)
        {
            // Example: "STATUS:CAN:INIT:6:12:2:1:0:0"
            string[] parts = data.Split(':');

            if (parts[1] == "CAN" &&
                parts.Length >= 9)
            {
                CANStatus status = new CANStatus();
                status.IsInitialized = (parts[2] == "INIT");

                if (status.IsInitialized)
                {
                    int prescaler = int.Parse(parts[3]);
                    int timeSeg1 = int.Parse(parts[4]);
                    int timeSeg2 = int.Parse(parts[5]);
                    int sjw = int.Parse(parts[6]);
                    status.RxErrorCount = int.Parse(parts[7]);
                    status.TxErrorCount = int.Parse(parts[8]);

                    // Tìm baudrate phù hợp
                    status.CurrentBaudRate = configCANBaud.LoadBaudRates()
                        .Find(b => b.Prescaler == prescaler &&
                                  b.TimeSeg1 == timeSeg1 &&
                                  b.TimeSeg2 == timeSeg2);

                    if (status.CurrentBaudRate == null)
                    {
                        // Tạo custom baud rate
                        int apb1Clock = 45; // MHz
                        int tq = 1 + timeSeg1 + timeSeg2;
                        int baudRate = (apb1Clock * 1000000) / (prescaler * tq);

                        status.CurrentBaudRate = new CANBaudRate(
                            $"{baudRate / 1000} kbps",
                            baudRate,
                            prescaler,
                            timeSeg1,
                            timeSeg2,
                            sjw
                        );
                    }
                }

                CANStatusReceived?.Invoke(this, status);
            }
        }

        /// <summary>
        /// Parse AUTOTX Status response từ STM32
        /// Format: 
        /// ========== AUTO TX STATUS ==========
        /// Active Entries:   3 / 10
        /// ------------------------------------
        /// [0] ID:0x123 Int:100ms [ON]
        ///     DLC:8 Data:01 02 03 04 05 06 07 08
        /// [1] ID:0x456 Int:500ms [OFF]
        ///     DLC: 4 Data:AA BB CC DD
        /// ====================================
        /// </summary>
        private void ParseAutoTxStatus(string data)
        {
            // Trigger event để UI xử lý
            AutoTxStatusReceived?.Invoke(this, new AutoTxStatusEventArgs { RawData = data });
        }

        #endregion

        #region Gửi dữ liệu qua Serial Port

        public List<CANTransmitConfig> GetCANTransmitConfigs()
        {
            return cANTransmits.Count > 0 ? cANTransmits.ToList() : new List<CANTransmitConfig>();
        }

        /// <summary>
        /// gửi tin nhắn CAN theo cấu hình
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool SendCANMessage(CANTransmitConfig config)
        {
            try
            {
                if (!serialPort.IsOpen)
                    return false;


                UpdateCANTransmitConfigs(config);

                // Protocol: "TX:Enable:Interver:Extended:ID:DLC:D0:D1:D2:D3:D4:D5:D6:D7\n"
                // Enable: 1 or 0. 1 là bật gửi định kỳ, 0 là tắt
                // Interver: khoảng thời gian gửi (ms)
                // Example: "TX:1:100:1AB:8:11:22:33:44:55:66:77:88\n" 
                StringBuilder sb = new StringBuilder();
                string canIdString = config.IsExtended ? $"{config.CANId:X8}" : $"{config.CANId:X3}";
                string isEnabled = config.IsEnabled ? "1" : "0";
                string isExtended = config.IsExtended ? "1" : "0";
                sb.Append($"TX:{isEnabled}:{config.IntervalMs.ToString()}:{isExtended}:{canIdString}:{config.DLC}");

                for (int i = 0; i < config.DLC; i++)
                {
                    sb.Append($":{config.Data[i]:X2}");
                }
                sb.Append("\n");

                serialPort.Write(sb.ToString());
                LogMessageReceived?.Invoke(this, $"Đã gửi: {sb.ToString().Trim()}");

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"Lỗi gửi CAN: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// cập nhật dữ liệu ghi vào cấu hình gửi CAN
        /// </summary>
        /// <param name="configs"></param>
        /// <param name="isDelete"></param>
        public void UpdateCANTransmitConfigs(CANTransmitConfig configs, bool isDelete = false)
        {
            if (configs == null)
                return;

            // check xem tồn tại chưa, nếu chưa thì ghi vào
            if (!cANTransmits.Any(c => c.CANId == configs.CANId))
            {
                cANTransmits.Add(configs);
            }
            else
            {

                if (isDelete)
                {
                    // Xóa cấu hình
                    var existingConfigDelete = cANTransmits.First(c => c.CANId == configs.CANId);
                    cANTransmits.Remove(existingConfigDelete);
                    return;
                }
                // Cập nhật cấu hình
                var existingConfig = cANTransmits.First(c => c.CANId == configs.CANId);
                existingConfig.Data = configs.Data;
                existingConfig.DLC = configs.DLC;
                existingConfig.IntervalMs = configs.IntervalMs;
                existingConfig.IsEnabled = configs.IsEnabled;
                existingConfig.IsExtended = configs.IsExtended;
            }
        }

        /// <summary>
        /// Cấu hình CAN Baud Rate
        /// </summary>
        public bool SetCANBaudRate(CANBaudRate baudRate)
        {
            try
            {
                if (!serialPort.IsOpen)
                    return false;

                // Command format: "CFG:CANBAUD:prescaler:timeseg1:timeseg2:sjw\n"
                string command = $"{baudRate.ToConfigCommand()}\n";
                serialPort.Write(command);

                LogMessageReceived?.Invoke(this, $"Đã gửi cấu hình CAN: {baudRate.Name}");
                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"Lỗi cấu hình CAN baud rate: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Yêu cầu STM32 gửi thông tin CAN hiện tại
        /// </summary>
        public bool RequestCANStatus()
        {
            try
            {
                if (!serialPort.IsOpen)
                    return false;

                string command = "GET:CANSTATUS\n";
                serialPort.Write(command);

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"Lỗi yêu cầu CAN status: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region AUTOTX Commands

        public CANTransmitConfig GetCANTransmitAutoConfig(int index)
        {
            if (index < 0 || index > 9)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 9.");

            return autoTxConfigs[index];
        }

        /// <summary>
        /// AUTOTX: ADD - Thêm message vào index chỉ định
        /// Format: AUTOTX:ADD:Index:Extended:ID:Interval:DLC:D0:D1:...
        /// </summary>
        public bool AutoTx_Add(int index, CANTransmitConfig config)
        {
            try
            {
                if (!serialPort.IsOpen) return false;

                if (index < 0 || index > 9)
                {
                    LogMessageReceived?.Invoke(this, $"ERR: Invalid index {index} (must be 0-9)");
                    return false;
                }

                // Xét giá trị để kiểm tra xem có bị ghi đè không
                // Nếu có thì kích hoạt lại
                var tempCan = autoTxConfigs[index];
                tempCan.IsEnabled = true;
                if (tempCan.Equals(config))
                {
                    return AutoTx_Enable(index);
                }

                // Build command
                StringBuilder sb = new StringBuilder();
                sb.Append($"AUTOTX:ADD:{index}");
                sb.Append($":{(config.IsExtended ? "1" : "0")}");
                sb.Append($":{config.CANId:X}");
                sb.Append($":{config.IntervalMs}");
                sb.Append($":{config.DLC}");

                for (int i = 0; i < config.DLC; i++)
                {
                    sb.Append($":{config.Data[i]:X2}");
                }
                sb.Append("\n");

                serialPort.Write(sb.ToString());
                LogMessageReceived?.Invoke(this, $"Sent: {sb.ToString().Trim()}");

                // Lưu vào local dictionary
                autoTxConfigs[index] = config;

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"ERR AutoTx_Add: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// AUTOTX:UPDATE - Cập nhật message tại index
        /// Format: AUTOTX:UPDATE:Index:Extended:ID: Interval:  DLC:D0:D1:...
        /// </summary>
        public bool AutoTx_Update(int index, CANTransmitConfig config)
        {
            try
            {
                if (!serialPort.IsOpen) return false;

                if (index < 0 || index > 9)
                {
                    LogMessageReceived?.Invoke(this, $"ERR: Invalid index {index}");
                    return false;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append($"AUTOTX:UPDATE:{index}");
                sb.Append($":{(config.IsExtended ? "1" : "0")}");
                sb.Append($":{config.CANId:X}");
                sb.Append($":{config.IntervalMs}");
                sb.Append($":{config.DLC}");

                for (int i = 0; i < config.DLC; i++)
                {
                    sb.Append($":{config.Data[i]:X2}");
                }
                sb.Append("\n");

                serialPort.Write(sb.ToString());
                LogMessageReceived?.Invoke(this, $"Sent: {sb.ToString().Trim()}");

                // Cập nhật local dictionary
                if (autoTxConfigs[index].IsEnabled)
                {
                    autoTxConfigs[index] = config;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"ERR AutoTx_Update: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// AUTOTX:  REMOVE - Xóa theo index
        /// Format: AUTOTX:REMOVE:Index
        /// </summary>
        public bool AutoTx_Remove(int index)
        {
            try
            {
                if (!serialPort.IsOpen) return false;

                string command = $"AUTOTX:REMOVE:{index}\n";
                serialPort.Write(command);
                LogMessageReceived?.Invoke(this, $"Sent: {command.Trim()}");

                // Xóa khỏi local dictionary
                if (autoTxConfigs[index].IsEnabled)
                {
                    autoTxConfigs[index] = new CANTransmitConfig();
                }

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"ERR AutoTx_Remove: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// AUTOTX: ENABLE - Enable auto-transmit
        /// Format: AUTOTX:ENABLE:Index
        /// </summary>
        public bool AutoTx_Enable(int index)
        {
            try
            {
                if (!serialPort.IsOpen) return false;

                string command = $"AUTOTX:ENABLE:{index}\n";
                serialPort.Write(command);
                LogMessageReceived?.Invoke(this, $"Sent: {command.Trim()}");

                // Cập nhật local config
                if (autoTxConfigs[index].IsEnabled == false)
                {
                    autoTxConfigs[index].IsEnabled = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"ERR AutoTx_Enable: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// AUTOTX:DISABLE - Disable auto-transmit
        /// Format: AUTOTX:DISABLE:Index
        /// </summary>
        public bool AutoTx_Disable(int index)
        {
            try
            {
                if (!serialPort.IsOpen) return false;

                string command = $"AUTOTX:DISABLE:{index}\n";
                serialPort.Write(command);
                LogMessageReceived?.Invoke(this, $"Sent: {command.Trim()}");

                // Cập nhật local config
                if (autoTxConfigs[index].IsEnabled == true)
                {
                    autoTxConfigs[index].IsEnabled = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"ERR AutoTx_Disable: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// AUTOTX:CLEAR - Xóa tất cả auto-transmit
        /// Format:  AUTOTX:CLEAR
        /// </summary>
        public bool AutoTx_Clear()
        {
            try
            {
                if (!serialPort.IsOpen) return false;

                string command = "AUTOTX: CLEAR\n";
                serialPort.Write(command);
                LogMessageReceived?.Invoke(this, $"Sent: {command.Trim()}");

                // Xóa tất cả local configs
                ClearCANTransmitConfigs();

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"ERR AutoTx_Clear: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// AUTOTX:  STATUS - Yêu cầu status
        /// Format: AUTOTX:STATUS
        /// </summary>
        public bool AutoTx_RequestStatus()
        {
            try
            {
                if (!serialPort.IsOpen) return false;

                string command = "AUTOTX:STATUS\n";
                serialPort.Write(command);
                LogMessageReceived?.Invoke(this, $"Sent: {command.Trim()}");

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"ERR AutoTx_RequestStatus: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách Auto TX configs hiện tại
        /// </summary>
        public CANTransmitConfig[] GetAutoTxConfigs()
        {
            return autoTxConfigs.ToArray();
        }

        /// <summary>
        /// Lấy rowindex của Auto TX config theo Id
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool GetAutoTxRowIndexById(string canId, out int rowIndex, bool searchZeroId = false)
        {
            rowIndex = -1;

            if(!uint.TryParse(canId, System.Globalization.NumberStyles.HexNumber, null, out uint canIdUInt))
            {
                return false; // Không hợp lệ
            }
            uint canid = canIdUInt;
            for (int i = 0; i < autoTxConfigs.Length; i++)
            {
                if (autoTxConfigs[i] != null && autoTxConfigs[i].CANId == canid)
                {
                    rowIndex = i;
                    return true;
                }
            }
            // Nếu không có dữ liệu thì trả về index mà CANID = 0 gần nhất
            if (searchZeroId)
            {
                for (int i = 0; i < autoTxConfigs.Length; i++)
                {
                    if (autoTxConfigs[i] != null && autoTxConfigs[i].CANId == 0)
                    {
                        rowIndex = i;
                        return true;
                    }
                }
            }
            return false; // Không tìm thấy
        }

        #endregion

        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Dispose()
        {
            Disconnect();
            serialPort?.Dispose();
        }
    }
}
