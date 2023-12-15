using System;
using System.IO;
using System.Reflection;

namespace BrutalCompanyPlus.BCP
{
    public class BcpLogger
    {
        private static string logFilePath;
        private const long MaxFileSize = 500 * 1024 * 1024; // 500 MB in bytes

        static BcpLogger()
        {
            try
            {
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                string directory = Path.GetDirectoryName(assemblyLocation);
                logFilePath = Path.Combine(directory, "bcp_logDUMP.txt");

                // Create the log file if it doesn't exist
                if (!File.Exists(logFilePath))
                {
                    File.Create(logFilePath).Dispose();
                }
            }
            catch (Exception ex)
            {
                Variables.mls.LogError("Error in BcpLogger static constructor: " + ex.Message);
            }
        }

        public static void Log(string message)
        {
            try
            {
                // Check if the file size exceeds the threshold
                if (new FileInfo(logFilePath).Length > MaxFileSize)
                {
                    File.WriteAllText(logFilePath, ""); // Clear the file
                }

                // Write the log message
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}\n");
                }
            }
            catch (Exception ex)
            {
                // Handle any logging errors (e.g., file access issues)
                // This should ideally not throw any exceptions to avoid recursive logging errors
                Variables.mls.LogError("--------- Error in logging: " + ex.Message + " ---------");
            }
        }

        // Optional: Call this method when the application exits
        public static void Close()
        {
            // Perform any cleanup or final logging here
            Log("---End of Session---");
        }

        public static void Start()
        {
            Log("---Start of Session---");
        }
    }
}
