using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public static class LogUtil
{
    private static readonly Lazy<object> logLock = new Lazy<object>(() => new object());
    private static string logFilePath;

    static LogUtil()
    {
        // Initialize the log file path
        string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        logFilePath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd_HH-mm}.log");
    }

    public static void log(string message)
    {
        lock (logLock.Value)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ");
            string logMessage = $"{timestamp}[{message}]{Environment.NewLine}";

            File.AppendAllText(logFilePath, logMessage);
        }
    }

    public static void OpenLog()
    {
        if (File.Exists(logFilePath))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = logFilePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open log file: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Log file does not exist.");
        }
    }
}
