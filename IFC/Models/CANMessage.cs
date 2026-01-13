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
    /// STM32 CAN Status
    /// </summary>
    public class CANStatus
    {
        public bool IsInitialized { get; set; }
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
                   $"Errors: RX={RxErrorCount} TX={TxErrorCount}";
        }
    }
}
