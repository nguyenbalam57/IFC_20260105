using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFC.Models
{
    public class CANMessage
    {
        public DateTime Timestamp { get; set; }
        public uint CANId { get; set; }
        public byte[] Data { get; set; }
        public int DLC { get; set; } // Data Length Code
        public bool IsExtended { get; set; }
        public bool IsRemote { get; set; }

        public bool IsRxTx { get; set; } // True = Rx, False = Tx

        public CANMessage()
        {
            Timestamp = DateTime.Now;
            Data = new byte[8];
        }

        public string ToLogString()
        {
            string dataHex = BitConverter.ToString(Data, 0, DLC).Replace("-", " ");
            string canIdString = IsExtended ? $"0x{CANId:X8}" : $"0x{CANId:X3}";
            string rxTxString = IsRxTx ? "RX" : "TX";
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff},{rxTxString},{canIdString},{DLC},{dataHex}";
        }

        public override string ToString()
        {
            string dataHex = BitConverter.ToString(Data, 0, DLC).Replace("-", " ");
            string canIdString = IsExtended ? $"0x{CANId:X8}" : $"0x{CANId:X3}";
            return $"ID: {canIdString} DLC: {DLC} Data: [{dataHex}]";
        }
    }

    public class CANTransmitConfig
    {

        public uint CANId { get; set; }
        public byte[] Data { get; set; }
        public int DLC { get; set; }
        public int IntervalMs { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsExtended { get; set; }

        public CANTransmitConfig()
        {
            CANId = 0x000;
            Data = new byte[8];
            DLC = 8;
            IntervalMs = 100;
            IsEnabled = false;
        }

        // Override Equals
        // So sánh các thuộc tính để xác định hai cấu hình có giống nhau không
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            CANTransmitConfig other = (CANTransmitConfig)obj;

            return CANId == other.CANId &&
                   DLC == other.DLC &&
                   IntervalMs == other.IntervalMs &&
                   IsEnabled == other.IsEnabled &&
                   IsExtended == other.IsExtended &&
                   DataEquals(Data, other.Data);
        }

        // So sánh byte array
        private bool DataEquals(byte[] data1, byte[] data2)
        {
            if (data1 == null && data2 == null)
                return true;

            if (data1 == null || data2 == null)
                return false;

            if (data1.Length != data2.Length)
                return false;

            for (int i = 0; i < data1.Length; i++)
            {
                if (data1[i] != data2[i])
                    return false;
            }

            return true;
        }

        // Override GetHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + CANId.GetHashCode();
                hash = hash * 23 + DLC.GetHashCode();
                hash = hash * 23 + IntervalMs.GetHashCode();
                hash = hash * 23 + IsEnabled.GetHashCode();
                hash = hash * 23 + IsExtended.GetHashCode();

                if (Data != null)
                {
                    foreach (byte b in Data)
                    {
                        hash = hash * 23 + b.GetHashCode();
                    }
                }

                return hash;
            }
        }

    }


    /// <summary>
    /// CAN Baud Rate Configuration
    /// </summary>
    public class CANBaudRate
    {
        public string Name { get; set; }
        public int BaudRate { get; set; }
        public int Prescaler { get; set; }
        public int TimeSeg1 { get; set; }
        public int TimeSeg2 { get; set; }
        public int SyncJumpWidth { get; set; }

        public CANBaudRate(string name, int baudRate, int prescaler, int timeSeg1, int timeSeg2, int sjw = 1)
        {
            Name = name;
            BaudRate = baudRate;
            Prescaler = prescaler;
            TimeSeg1 = timeSeg1;
            TimeSeg2 = timeSeg2;
            SyncJumpWidth = sjw;
        }

        // Override ToString để hiển thị trong ComboBox
        public override string ToString()
        {
            return $"{Name} ({BaudRate} bps)";
        }

        /// <summary>
        /// Command string để gửi xuống STM32
        /// Format: "CFG:CANBAUD:prescaler:timeseg1:timeseg2:sjw"
        /// </summary>
        public string ToConfigCommand()
        {
            return $"CFG:CANBAUD:{Prescaler}:{TimeSeg1}:{TimeSeg2}:{SyncJumpWidth}";
        }

        /// <summary>
        /// Hiển thị thông tin chi tiết
        /// </summary>
        public string GetDetailedInfo()
        {
            int totalTQ = 1 + TimeSeg1 + TimeSeg2;
            double samplePoint = ((1.0 + TimeSeg1) / totalTQ) * 100;

            return $"{Name}\n" +
                   $"Baud Rate: {BaudRate / 1000} kbps\n" +
                   $"Prescaler: {Prescaler}\n" +
                   $"TimeSeg1: {TimeSeg1} TQ\n" +
                   $"TimeSeg2: {TimeSeg2} TQ\n" +
                   $"SJW: {SyncJumpWidth} TQ\n" +
                   $"Total TQ: {totalTQ}\n" +
                   $"Sample Point: {samplePoint:F1}%";
        }
    }

    public class CANBaudRateConfig
    {
        public List<CANBaudRate> BaudRates { get; set; }

        public CANBaudRateConfig()
        {
            BaudRates = new List<CANBaudRate>();
        }
    }


    public class CANBaudRateManager
    {
        private const string CONFIG_FILE = "CANBaudRates.json";
        private string configFilePath;

        public CANBaudRateManager()
        {
            // Lưu file trong thư mục ứng dụng
            configFilePath = Path.Combine(Application.StartupPath, CONFIG_FILE);
        }

        /// <summary>
        /// Load baud rates từ file JSON
        /// </summary>
        public List<CANBaudRate> LoadBaudRates()
        {
            try
            {
                if (!File.Exists(configFilePath))
                {
                    // Nếu file không tồn tại, tạo file mặc định
                    CreateDefaultConfigFile();
                }

                string json = File.ReadAllText(configFilePath);
                CANBaudRateConfig config = JsonConvert.DeserializeObject<CANBaudRateConfig>(json);

                if (config?.BaudRates != null && config.BaudRates.Count > 0)
                {
                    Debug.WriteLine($"Loaded {config.BaudRates.Count} baud rates from {configFilePath}");
                    return config.BaudRates;
                }
                else
                {
                    Debug.WriteLine("No baud rates found in config file.  Creating default.. .");
                    return CreateDefaultBaudRates();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading baud rates: {ex.Message}");
                MessageBox.Show($"Không thể load file cấu hình: {ex.Message}\nSử dụng cấu hình mặc định.",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return CreateDefaultBaudRates();
            }
        }

        /// <summary>
        /// Lưu baud rates vào file JSON
        /// </summary>
        public bool SaveBaudRates(List<CANBaudRate> baudRates)
        {
            try
            {
                CANBaudRateConfig config = new CANBaudRateConfig
                {
                    BaudRates = baudRates
                };

                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configFilePath, json);

                Debug.WriteLine($"Saved {baudRates.Count} baud rates to {configFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving baud rates:  {ex.Message}");
                MessageBox.Show($"Không thể lưu file cấu hình: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Tạo file cấu hình mặc định
        /// </summary>
        private void CreateDefaultConfigFile()
        {
            List<CANBaudRate> defaultRates = CreateDefaultBaudRates();
            SaveBaudRates(defaultRates);
            Debug.WriteLine($"Created default config file: {configFilePath}");
        }

        /// <summary>
        /// Tạo danh sách baud rates mặc định
        /// </summary>
        private List<CANBaudRate> CreateDefaultBaudRates()
        {
            return new List<CANBaudRate>
            {
                // Name,BaudRate,Prescaler,TimeSeg1,TimeSeg2,SyncJumpWidth
                new CANBaudRate ( "10 kbps",  10000,   200, 13, 2, 1 ),
                new CANBaudRate ( "20 kbps",  20000,   100, 13, 2, 1 ),
                new CANBaudRate ( "50 kbps",  50000,   40,  13, 2, 1 ),
                new CANBaudRate ( "100 kbps", 100000,  20,  13, 2, 1 ),
                new CANBaudRate ( "125 kbps", 125000,  16,  13, 2, 1 ),
                new CANBaudRate ( "250 kbps", 250000,  8,   13, 2, 1 ),
                new CANBaudRate ( "500 kbps", 500000,  4,   13, 2, 1 ),
                new CANBaudRate ( "800 kbps", 800000,  2,   14, 3, 1 ),
                new CANBaudRate ( "1 Mbps",   1000000, 2,   13, 2, 1 )
            };
        }

        public CANBaudRate GetBaudRateByName(string name)
        {
            var baudRates = LoadBaudRates();
            return baudRates.FirstOrDefault(br => br.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Lấy đường dẫn file config
        /// </summary>
        public string GetConfigFilePath()
        {
            return configFilePath;
        }
    }



    /// <summary>
    /// Danh sách CAN Baud Rate chuẩn
    /// Tính toán dựa trên APB1 Clock = 45MHz (STM32F4)
    /// </summary>
    public class CANBaudRateList
    {
        // APB1 Clock frequency (MHz) - Thay đổi theo MCU của bạn
        public const int APB1_CLOCK_MHZ = 45;

        /// <summary>
        /// Tính toán CAN Baud Rate tùy chỉnh
        /// </summary>
        public static CANBaudRate CalculateCustomBaudRate(int targetBaudRate, int apb1ClockMHz = APB1_CLOCK_MHZ)
        {
            // Tìm prescaler và TQ phù hợp
            int apb1ClockHz = apb1ClockMHz * 1000000;

            // Thử các giá trị TQ phổ biến
            int[] tqOptions = { 16, 15, 20, 25, 10, 8 };

            foreach (int tq in tqOptions)
            {
                int prescaler = apb1ClockHz / (targetBaudRate * tq);

                if (prescaler >= 1 && prescaler <= 1024)
                {
                    // Tính actual baud rate
                    int actualBaudRate = apb1ClockHz / (prescaler * tq);

                    // Kiểm tra sai số
                    double error = Math.Abs((double)(actualBaudRate - targetBaudRate) / targetBaudRate) * 100;

                    if (error < 5.0) // Sai số < 5%
                    {
                        // Phân bổ TQ cho TimeSeg1 và TimeSeg2
                        // Sample point ở ~87.5%: TimeSeg1 = 13, TimeSeg2 = 2 (cho 16 TQ)
                        int timeSeg2 = Math.Max(2, tq / 8);
                        int timeSeg1 = tq - 1 - timeSeg2;

                        return new CANBaudRate(
                            $"{actualBaudRate / 1000} kbps (Custom)",
                            actualBaudRate,
                            prescaler,
                            timeSeg1,
                            timeSeg2,
                            1
                        );
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Validate CAN Baud Rate configuration
        /// </summary>
        public static bool ValidateConfiguration(CANBaudRate config, int apb1ClockMHz = APB1_CLOCK_MHZ)
        {
            // Kiểm tra giới hạn
            if (config.Prescaler < 1 || config.Prescaler > 1024) return false;
            if (config.TimeSeg1 < 1 || config.TimeSeg1 > 16) return false;
            if (config.TimeSeg2 < 1 || config.TimeSeg2 > 8) return false;
            if (config.SyncJumpWidth < 1 || config.SyncJumpWidth > 4) return false;

            // Tính toán baud rate thực tế
            int apb1ClockHz = apb1ClockMHz * 1000000;
            int tq = 1 + config.TimeSeg1 + config.TimeSeg2;
            int actualBaudRate = apb1ClockHz / (config.Prescaler * tq);

            // Kiểm tra sai số
            double error = Math.Abs((double)(actualBaudRate - config.BaudRate) / config.BaudRate) * 100;

            return error < 5.0; // Sai số phải < 5%
        }
    }

    /// <summary>
    /// STM32 CAN Status
    /// </summary>
    public class CANStatus
    {
        public bool IsInitialized { get; set; }
        public CANBaudRate CurrentBaudRate { get; set; }
        public int RxErrorCount { get; set; }
        public int TxErrorCount { get; set; }
        public string LastError { get; set; }

        public CANStatus()
        {
            IsInitialized = false;
            RxErrorCount = 0;
            TxErrorCount = 0;
            LastError = string.Empty;
        }

        public override string ToString()
        {
            return $"CAN: {(IsInitialized ? "OK" : "Not Init")} | " +
                   $"Baud: {CurrentBaudRate?.Name ?? "N/A"} | " +
                   $"Errors: RX={RxErrorCount} TX={TxErrorCount}";
        }
    }
}
