using IFC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC.Serial
{
    public class LogManager
    {
        private string logDirectory;
        private string currentLogFile;
        private StreamWriter logWriter;
        private object lockObject = new object();

        public bool IsLogging { get; private set; }

        public LogManager(string baseDirectory = "Logs")
        {
            logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseDirectory);

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);
        }

        public bool StartLogging()
        {
            try
            {
                if (IsLogging)
                    StopLogging();

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                currentLogFile = Path.Combine(logDirectory, $"CAN_Log_{timestamp}.csv");

                logWriter = new StreamWriter(currentLogFile, false, Encoding.UTF8);

                // Ghi header
                logWriter.WriteLine("Timestamp,Direction,CAN ID,DLC,Data");
                logWriter.Flush();

                IsLogging = true;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể bắt đầu logging: {ex.Message}");
            }
        }

        public void StopLogging()
        {
            if (IsLogging)
            {
                lock (lockObject)
                {
                    logWriter?.Flush();
                    logWriter?.Close();
                    logWriter?.Dispose();
                    IsLogging = false;
                }
            }
        }

        public void LogCANMessage(CANMessage message)
        {
            if (!IsLogging)
                return;

            lock (lockObject)
            {
                try
                {
                    logWriter.WriteLine(message.ToLogString());
                    logWriter.Flush();
                }
                catch (Exception ex)
                {
                    // Log error
                    System.Diagnostics.Debug.WriteLine($"Lỗi ghi log: {ex.Message}");
                }
            }
        }

        public string GetCurrentLogFile()
        {
            return currentLogFile;
        }

        public string[] GetLogFiles()
        {
            return Directory.GetFiles(logDirectory, "*.csv");
        }
    }
}
