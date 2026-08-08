using System;
using System.IO;
using System.ServiceProcess;

namespace DemoService
{
    public class Service1 : ServiceBase
    {
        private System.Timers.Timer? _timer;
        private string? _logPath;

        public Service1()
        {
            ServiceName = "DemoAppService";
        }

        protected override void OnStart(string[] args)
        {
            string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "DemoService");
            Directory.CreateDirectory(logDir);
            _logPath = Path.Combine(logDir, "service_activity.log");

            File.AppendAllText(_logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] DemoService started.\n");

            _timer = new System.Timers.Timer(1000); // Every 1 second
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(_logPath))
                {
                    File.AppendAllText(_logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Service heartbeat active.\n");
                }
            }
            catch
            {
            }
        }

        protected override void OnStop()
        {
            _timer?.Stop();
            _timer?.Dispose();
            if (!string.IsNullOrEmpty(_logPath))
            {
                File.AppendAllText(_logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] DemoService stopped.\n");
            }
        }
    }
}
