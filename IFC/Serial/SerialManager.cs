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

        // Code cải tiến sử dụng Task thay vì Thread
        private Task readTask;
        private CancellationTokenSource cancellationTokenSource;

        // Lock cho write operations (thread-safe)
        private readonly SemaphoreSlim writeSemaphore = new SemaphoreSlim(1, 1);

        public event EventHandler<CANMessage> CANMessageReceived;
        public event EventHandler<string> LogMessageReceived;
        public event EventHandler<bool> ConnectionChanged;

        public bool IsConnected => serialPort?.IsOpen ?? false;

        public event EventHandler<CANStatus> CANStatusReceived;

        // Thêm event mới cho AUTOTX Status
        public event EventHandler<AutoTxStatusEventArgs> AutoTxStatusReceived;

        // ⭐ Thêm Dictionary để quản lý Auto TX messages
        private CANTransmitConfig[] autoTxConfigs = new CANTransmitConfig[10]; // Key = Index (0-9)

        public SerialManager()
        {
            serialPort = new SerialPort();

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

        #region Connect / Disconnect

        /// <summary>
        /// Kết nối với UART (COM)
        /// thay đổi thành Async task - Non-blocking
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(string portName, int baudRate)
        {
            try
            {
                if (serialPort.IsOpen)
                    await DisconnectAsync();

                serialPort.PortName = portName;
                serialPort.BaudRate = baudRate;
                serialPort.DataBits = 8;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.Handshake = Handshake.None;

                // Increase buffer sizes
                serialPort.ReadBufferSize = 16384;  // 16 KB
                serialPort.WriteBufferSize = 8192;   // 8 KB

                serialPort.ReadTimeout = 1000;
                serialPort.WriteTimeout = 1000;

                // Open port in background thread to avoid UI blocking
                await Task.Run(() => serialPort.Open());

                // Tạo CancellationTokenSource mới
                cancellationTokenSource = new CancellationTokenSource();

                // Bắt đầu async read task
                readTask = ReadDataAsync(cancellationTokenSource.Token);

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
        /// Backward compatibility - Synchronous wrapper
        /// </summary>
        public bool Connect(string portName, int baudRate)
        {
            return ConnectAsync(portName, baudRate).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Ngắt kết nối với UART (COM)
        /// </summary>
        public async Task DisconnectAsync()
        {
            try
            {
                // Cancel async operations
                cancellationTokenSource?.Cancel();

                // Wait for read task to complete (with timeout)
                if (readTask != null)
                {
                    await Task.WhenAny(readTask, Task.Delay(2000));
                }

                // Close port in background thread
                if (serialPort.IsOpen)
                {
                    await Task.Run(() => serialPort.Close());
                }

                // Cleanup
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;

                ConnectionChanged?.Invoke(this, false);
                LogMessageReceived?.Invoke(this, "Đã ngắt kết nối");
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"Lỗi ngắt kết nối: {ex.Message}");
            }
        }

        /// <summary>
        /// Backward compatibility - Synchronous wrapper
        /// </summary>
        public void Disconnect()
        {
            DisconnectAsync().GetAwaiter().GetResult();
        }

        #endregion

        #region Nhận dữ liệu từ Serial Port

        /// <summary>
        /// Đọc data từ serial port trong một thread riêng
        /// </summary>
        private async Task ReadDataAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1024];
            int bufferIndex = 0;

            try
            {
                while (!cancellationToken.IsCancellationRequested && serialPort.IsOpen)
                {
                    try
                    {
                        if (serialPort.BytesToRead > 0)
                        {
                            // ✅ Async read (non-blocking)
                            int bytesToRead = Math.Min(serialPort.BytesToRead, buffer.Length - bufferIndex);

                            // Wrap synchronous Read in Task. Run for true async
                            int bytesRead = await Task.Run(() =>
                                serialPort.Read(buffer, bufferIndex, bytesToRead),
                                cancellationToken);

                            bufferIndex += bytesRead;

                            // Tìm ký tự kết thúc frame (ví dụ: '\n')
                            int newlineIndex;
                            while ((newlineIndex = Array.IndexOf(buffer, (byte)'\n', 0, bufferIndex)) >= 0)
                            {
                                // Xử lý frame
                                byte[] frame = new byte[newlineIndex];
                                Array.Copy(buffer, frame, newlineIndex);

                                // Process frame asynchronously
                                await ProcessReceivedFrameAsync(frame);

                                // Dịch chuyển buffer
                                int remaining = bufferIndex - newlineIndex - 1;
                                if (remaining > 0)
                                    Array.Copy(buffer, newlineIndex + 1, buffer, 0, remaining);
                                bufferIndex = remaining;
                            }
                        }
                        else
                        {
                            // Async delay instead of Thread.Sleep
                            await Task.Delay(10, cancellationToken);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected when cancelling
                        break;
                    }
                    catch (TimeoutException)
                    {
                        // Normal timeout, continue
                    }
                    catch (Exception ex)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            LogMessageReceived?.Invoke(this, $"Lỗi đọc dữ liệu: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"Read loop error: {ex.Message}");
            }

            
        }

        // Cập nhật hàm ProcessReceivedFrame
        private async Task ProcessReceivedFrameAsync(byte[] frame)
        {
            try
            {
                // ✅ Decode in background thread (CPU-intensive)
                string frameStr = await Task.Run(() =>
                    Encoding.ASCII.GetString(frame).Trim());

                if (frameStr.StartsWith("CAN:"))
                {
                    await ParseCANMessageAsync(frameStr);
                }
                else if (frameStr.StartsWith("LOG:"))
                {
                    LogMessageReceived?.Invoke(this, frameStr.Substring(4));
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
        private async Task ParseCANMessageAsync(string data)
        {
            // Parse in background thread (avoid blocking)
            var msg = await Task.Run(() =>
            {
                // Format: "CAN:ID:DLC:D0:D1:D2:D3:D4:D5:D6:D7"
                string[] parts = data.Split(':');

                if (parts.Length >= 3)
                {
                    CANMessage message = new CANMessage();

                    // Parse CAN ID (hex)
                    message.CANId = Convert.ToUInt32(parts[1], 16);

                    // Parse DLC
                    message.DLC = int.Parse(parts[2]);

                    // Parse Data bytes
                    int dataStartIndex = 3;
                    for (int i = 0; i < message.DLC && (dataStartIndex + i) < parts.Length; i++)
                    {
                        message.Data[i] = Convert.ToByte(parts[dataStartIndex + i], 16);
                    }

                    if (message.CANId > 0x7FF)
                        message.IsExtended = true;

                    message.Timestamp = DateTime.Now;
                    return message;
                }
                return null;
            });

                
            if (msg != null)
            {
                CANMessageReceived?.Invoke(this, msg);
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

        /// <summary>
        /// gửi tin nhắn CAN theo cấu hình
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<bool> SendCANMessageAsync(CANTransmitConfig config)
        {
            // ✅ Acquire semaphore for thread-safe write
            await writeSemaphore.WaitAsync();

            try
            {
                if (!serialPort.IsOpen)
                    return false;


                if(GetAutoTxRowIndexById($"{config.CANId:X}", out int existingIndex, true))
                {
                    autoTxConfigs[existingIndex] = config;
                }
                
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

                // Async write
                byte[] data = Encoding.ASCII.GetBytes(sb.ToString());
                await Task.Run(() => serialPort.Write(data, 0, data.Length));

                LogMessageReceived?.Invoke(this, $"Đã gửi: {sb.ToString().Trim()}");

                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"Lỗi gửi CAN: {ex.Message}");
                return false;
            }
            finally
            {
                // Release semaphore
                writeSemaphore.Release();
            }
        }

        /// <summary>
        /// Backward compatibility
        /// </summary>
        public bool SendCANMessage(CANTransmitConfig config)
        {
            return SendCANMessageAsync(config).GetAwaiter().GetResult();
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
        /// Generic async command sender
        /// </summary>
        private async Task<bool> SendCommandAsync(string command)
        {
            await writeSemaphore.WaitAsync();

            try
            {
                if (!serialPort.IsOpen)
                    return false;

                byte[] data = Encoding.ASCII.GetBytes(command + "\n");
                await Task.Run(() => serialPort.Write(data, 0, data.Length));

                LogMessageReceived?.Invoke(this, $"Sent: {command}");
                return true;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"ERR SendCommand: {ex.Message}");
                return false;
            }
            finally
            {
                writeSemaphore.Release();
            }
        }

        /// <summary>
        /// AUTOTX: ADD - Thêm message vào index chỉ định
        /// Format: AUTOTX:ADD:Index:Extended:ID:Interval:DLC:D0:D1:...
        /// </summary>
        public async Task<bool> AutoTx_AddAsync(int index, CANTransmitConfig config)
        {
            try
            {

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
                    return await AutoTx_EnableAsync(index);
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

                bool result = await SendCommandAsync(sb.ToString());

                // Lưu vào local dictionary
                if (result)
                {
                    autoTxConfigs[index] = config;
                }

                return result;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"ERR AutoTx_Add: {ex.Message}");
                return false;
            }
        }

        public bool AutoTx_Add(int index, CANTransmitConfig config)
        {
            return AutoTx_AddAsync(index, config).GetAwaiter().GetResult();
        }

        /// <summary>
        /// AUTOTX:UPDATE - Cập nhật message tại index
        /// Format: AUTOTX:UPDATE:Index:Extended:ID: Interval:  DLC:D0:D1:...
        /// </summary>
        public async Task<bool> AutoTx_UpdateAsync(int index, CANTransmitConfig config)
        {
            try
            {
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

                bool result = await SendCommandAsync(sb.ToString());

                if (result && autoTxConfigs[index].IsEnabled)
                {
                    autoTxConfigs[index] = config;
                }

                return result;
            }
            catch (Exception ex)
            {
                LogMessageReceived?.Invoke(this, $"ERR AutoTx_Update: {ex.Message}");
                return false;
            }
        }

        public bool AutoTx_Update(int index, CANTransmitConfig config)
        {
            return AutoTx_UpdateAsync(index, config).GetAwaiter().GetResult();
        }

        /// <summary>
        /// AUTOTX:  REMOVE - Xóa theo index
        /// Format: AUTOTX:REMOVE:Index
        /// </summary>
        public async Task<bool> AutoTx_RemoveAsync(int index)
        {


                string command = $"AUTOTX:REMOVE:{index}\n";

                bool result = await SendCommandAsync(command);

                if (result && autoTxConfigs[index].IsEnabled)
                {
                    autoTxConfigs[index] = new CANTransmitConfig();
                }

                return result;

        }

        public bool AutoTx_Remove(int index)
        {
            return AutoTx_RemoveAsync(index).GetAwaiter().GetResult();
        }

        /// <summary>
        /// AUTOTX: ENABLE - Enable auto-transmit
        /// Format: AUTOTX:ENABLE:Index
        /// </summary>
        public async Task<bool> AutoTx_EnableAsync(int index)
        {


                string command = $"AUTOTX:ENABLE:{index}\n";

                bool result = await SendCommandAsync(command);

                if (result && !autoTxConfigs[index].IsEnabled)
                {
                    autoTxConfigs[index].IsEnabled = true;
                }

                return result;

        }

        public bool AutoTx_Enable(int index)
        {
            return AutoTx_EnableAsync(index).GetAwaiter().GetResult();
        }

        /// <summary>
        /// AUTOTX:DISABLE - Disable auto-transmit
        /// Format: AUTOTX:DISABLE:Index
        /// </summary>
        public async Task<bool> AutoTx_DisableAsync(int index)
        {

                string command = $"AUTOTX:DISABLE:{index}\n";

                bool result = await SendCommandAsync(command);

                if (result && autoTxConfigs[index].IsEnabled)
                {
                    autoTxConfigs[index].IsEnabled = false;
                }

                return result;

        }

        public bool AutoTx_Disable(int index)
        {
            return AutoTx_DisableAsync(index).GetAwaiter().GetResult();
        }

        /// <summary>
        /// AUTOTX:CLEAR - Xóa tất cả auto-transmit
        /// Format:  AUTOTX:CLEAR
        /// </summary>
        public async Task<bool> AutoTx_ClearAsync()
        {
            bool result = await SendCommandAsync("AUTOTX:CLEAR");

            if (result)
            {
                ClearCANTransmitConfigs();
            }

            return result;
        }
        public bool AutoTx_Clear()
        {
            return AutoTx_ClearAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// AUTOTX:  STATUS - Yêu cầu status
        /// Format: AUTOTX:STATUS
        /// </summary>
        public async Task<bool> AutoTx_RequestStatusAsync()
        {
            return await SendCommandAsync("AUTOTX:STATUS");
        }

        public bool AutoTx_RequestStatus()
        {
            return AutoTx_RequestStatusAsync().GetAwaiter().GetResult();
        }
        #endregion

        #region Helper Methods
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
            if( canid <= 0)
            {
                return false; // Vượt quá giá trị CAN ID hợp lệ
            }
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

        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }
        #endregion

        #region Dispose

        public void Dispose()
        {
            DisconnectAsync().GetAwaiter().GetResult();
            writeSemaphore?.Dispose();
            serialPort?.Dispose();
        }

        /// <summary>
        /// Async dispose
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
            writeSemaphore?.Dispose();
            serialPort?.Dispose();
        }

        #endregion

    }
}
