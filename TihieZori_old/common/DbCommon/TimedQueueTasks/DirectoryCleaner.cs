using DbCommon.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DbCommon.TimedQueueTasks
{
    public class DirectoryCleaner : TimedQueue.ITimedAction
    {
        public const int MaxFilesCount = 10;
        private DateTime _startUtcTime;
        private readonly TimeSpan _interval;
        private readonly string _folder;
        private readonly string _mask;
        private CancellationToken? _stopToken;

        public DirectoryCleaner(int intervalMin, DateTime startUtcTime, string folder, string mask, CancellationToken? stopCancellationToken = null)
        {
            _startUtcTime = startUtcTime;
            _interval = new TimeSpan(0, intervalMin, 0);
            _folder = folder;
            _mask = mask;
            _stopToken = stopCancellationToken;
        }


        public System.DateTime StartUtcTime
        {
            get { return _startUtcTime; }
            set { _startUtcTime = value; }
        }

        public System.TimeSpan Interval
        {
            get { return _interval; }
        }

        public void DoAction()
        {
            DateTime now = DateTime.UtcNow;
            try
            {
                if (!Directory.Exists(_folder))
                    return;
                var files = new List<FileInfo>();
                foreach (var fileName in Directory.EnumerateFiles(_folder, _mask, SearchOption.TopDirectoryOnly))
                {
                    if (_stopToken.HasValue? _stopToken.Value.IsCancellationRequested : false)
                        return;
                    files.Add(new FileInfo(fileName));
                }

                foreach (var fi in files.OrderByDescending(fi => fi.CreationTimeUtc).Skip(MaxFilesCount))
                {
                    if (_stopToken.HasValue ? _stopToken.Value.IsCancellationRequested : false)
                        return;
                    try
                    {
                        fi.Delete();
                    }
                    catch (ThreadInterruptedException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Ошибка при удалении файла :" + fi.FullName);
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении файлов");
            }
        }

    }
}
