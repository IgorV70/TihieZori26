using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DbCommon.Helpers
{
    public enum LogLevelEnum
    {
        Debug = 0,
        Trace = 1,
        Info = 2,
        Error = 3,
        Fatal = 4,
    }
    public static class Log
    {
        public static bool ConsoleEcho = false ;
        public delegate void LogEvent(object sender, string textToLog);
        public static event LogEvent ExternavLogWriter;

        public static LogLevelEnum LogLevel = LogLevelEnum.Debug;
        private static readonly object SyncIo = new object();
        private static readonly object SyncDict = new object();
        private static readonly EventWaitHandle WaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private static readonly List<string> LogList = new List<string>();

        static Log()
        {
            Thread logThread = new Thread(LogWorker) { IsBackground = true };
            logThread.Start();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Error(e.ExceptionObject as Exception, "Необработанное исключение");
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            FlushLog();
        }

        //[Conditional("ERROR_ON")]
        public static void Error(Exception ex, string message, params object[] args)
        {
            if (LogLevel > LogLevelEnum.Error) return;
            try
            {

                string tsdt = "";
                string tsname = "";
                if (ex.TargetSite != null)
                {
                    if (ex.TargetSite.DeclaringType != null)
                        tsdt = ex.TargetSite.DeclaringType.ToString();
                    tsname = ex.TargetSite.Name;
                }
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [ERROR] [{1}]", DateTime.Now, ex.GetType().Name)
                    + string.Format(message, args) + "\r\n"
                    + string.Format("[{0}.{1}()] {2}\r\n", tsdt, tsname, ex.Message) + "\r\n"
                    + ex.StackTrace + "\r\n";

                lock (SyncDict)
                {
                    LogList.Add(fullText);
                }
                WaitHandle.Set();
            }
            catch (Exception)
            {
                //empty
            }
        }
        public static void Error(string message, params object[] args)
        {
            if (LogLevel == LogLevelEnum.Fatal) return;
            try
            {
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [ERROR] {1}\r\n",
                        DateTime.Now, string.Format(message, args));

                lock (SyncDict)
                {
                    LogList.Add(fullText);
                }
                WaitHandle.Set();
            }
            catch
            {
                //empty
            }

        }
        public static void Info(string message, params object[] args)
        {
            if (LogLevel > LogLevelEnum.Info) return;
            try
            {
                try
                {
                    message = string.Format(message, args);
                }
                catch (Exception)
                {
                    Log.Error("Log.Info: Ошибка при форматировании сообщения об ошибке:" + message);
                }

                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [INFO] {1}\r\n",
                        DateTime.Now, message);

                lock (SyncDict)
                {
                    LogList.Add(fullText);
                }
                WaitHandle.Set();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Log.Info");
            }
        }
        public static void Trace(string message, params object[] args)
        {
            if (LogLevel > LogLevelEnum.Trace) return;
            try
            {
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [TRACE] {1}\r\n",
                        DateTime.Now, string.Format(message, args));

                lock (SyncDict)
                {
                    LogList.Add(fullText);
                }
                WaitHandle.Set();
            }
            catch
            {
                //empty
            }
        }

        public static void Debug(string message, params object[] args)
        {
            if (LogLevel > LogLevelEnum.Debug) return;
            try
            {
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [DEBUG] {1}\r\n",
                        DateTime.Now, string.Format(message, args));

                lock (SyncDict)
                {
                    LogList.Add(fullText);
                }
                WaitHandle.Set();
            }
            catch (Exception)
            {
                //throw;
            }
        }


        private static string GetLogFileName()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            if (!Directory.Exists(pathToLog))
                Directory.CreateDirectory(pathToLog);
            string friendlyName = AppDomain.CurrentDomain.FriendlyName.Split(new[] { '/', '\\', ':' }).Last();
            return Path.Combine(pathToLog, string.Format("{0}_{1:yyyMMdd}.log", friendlyName, DateTime.Now));
        }

        private static void LogWorker()
        {
            try
            {

                while (true)
                {
                    WaitHandle.Reset();
                    WaitHandle.WaitOne();
                    Thread.Sleep(1000);
                    FlushLog();
                }
            }
            catch (ThreadAbortException)
            {
                Trace("LogWorker ThreadAbortException");
                FlushLog();
            }
            catch (ThreadInterruptedException)
            {
                Trace("LogWorker ThreadInterruptedException");
                FlushLog();
            }
            catch (Exception ex)
            {
                Error(ex, "LogWorker");
                FlushLog();
            }
        }

        private static void FlushLog()
        {
            try
            {
                string allLogText;
                lock (SyncDict)
                {
                    if (LogList.Count == 0) return;
                    allLogText = LogList.Aggregate("", (current, s) => current + s);
                    LogList.Clear();
                }

                string filename = GetLogFileName();

                lock (SyncIo)
                {
                    File.AppendAllText(filename, allLogText, Encoding.GetEncoding("Windows-1251"));
                }
                OnExternavLogWriter(allLogText);
            }
            catch (ThreadInterruptedException)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception)
            {
                // throw;
            }
        }


        public static void SaveBitmap(System.Drawing.Bitmap bitmap, string p)
        {
            string path = Path.GetDirectoryName(GetLogFileName());
            bitmap.Save(Path.Combine(path, p), ImageFormat.Bmp);
        }

        private static void OnExternavLogWriter(string texttolog)
        {
            var handler = ExternavLogWriter;
            if (handler != null)
                handler(null, texttolog);
        }
    }
}