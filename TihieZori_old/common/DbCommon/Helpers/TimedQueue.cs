using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DbCommon.Helpers
{
    public class TimedQueue : IDisposable
    {

        public interface ITimedAction
        {
            DateTime StartUtcTime { get; set; }
            TimeSpan Interval { get; }
            void DoAction();

        }

        /// <summary>
        /// Maximal wait time when joining an interrupted consumer thread
        /// </summary>
        private const int MaxJoinWaitTimeout = 12000;

        private readonly MinPriorityQueue<DateTime, ITimedAction> _minPriorityQueue = new MinPriorityQueue<DateTime, ITimedAction>();


        private readonly object _sync = new object();

        private readonly Thread _consumerThread;

        private bool _isDisposed = false;


        /// <summary>
        /// Creates a new TimedQueue instance with empty queues
        /// </summary>
        /// <param name="consumerAction">A method to be used for user data processing</param>
        public TimedQueue()
        {
            _consumerThread = new Thread(ThreadMain);
            _consumerThread.IsBackground = true;
        }
        public TimedQueue(bool isBackground = true)
        {
            _consumerThread = new Thread(ThreadMain);
            _consumerThread.IsBackground = isBackground;
        }

        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(objectName: null);

            if (_consumerThread.IsAlive)
                throw new InvalidOperationException("Consumer thread has already been started");

            _consumerThread.Start();
        }

        /// <summary>
        /// Interrupts the consumer thread and waits for its exit. Discards all contents
        /// of immediate queue and timed queue
        /// </summary>
        public void Stop()
        {
            lock (_sync)
            {
                _isDisposed = true;
                _minPriorityQueue.Clear();
            }
            if (_consumerThread.IsAlive)
                _consumerThread.Interrupt();
            else
                return;

            if (!_consumerThread.Join(MaxJoinWaitTimeout))
            {
                Log.Trace("Unable to stop the cosnumer thread in allowed timeout during Stop. Timeout = {0} milliseconds", MaxJoinWaitTimeout);
                throw new TimeoutException("Join attempt timed out");
            }

        }

        public bool EnqueueTimed(ITimedAction taskData, TimeSpan delay)
        {
            if (delay < TimeSpan.Zero)
                throw new ArgumentException("Argument cannot be negative", "delay");

            DateTime startTime = DateTime.UtcNow + delay;
            lock (_sync)
            {
                _minPriorityQueue.Enqueue(startTime, taskData);
                Monitor.Pulse(_sync);
            }
            return true;
        }

        public bool EnqueueTimed(ITimedAction taskData, int delayMilliseconds)
        {
            if (delayMilliseconds < 0)
                throw new ArgumentException("Argument cannot be negative", "delayMilliseconds");

            DateTime startTime = DateTime.UtcNow;
            startTime = startTime.AddMilliseconds(delayMilliseconds);
            lock (_sync)
            {
                _minPriorityQueue.Enqueue(startTime, taskData);
                Monitor.Pulse(_sync);
            }
            return true;
        }

        public bool EnqueueUtc(ITimedAction taskData, DateTime utcTime)
        {

            lock (_sync)
            {
                _minPriorityQueue.Enqueue(utcTime, taskData);
                Monitor.Pulse(_sync);
            }
            return true;
        }

        public bool Enqueue(ITimedAction taskData)
        {
            lock (_sync)
            {
                _minPriorityQueue.Enqueue(taskData.StartUtcTime, taskData);
                Monitor.Pulse(_sync);
            }
            return true;
        }


        /// <summary>
        /// Removes the first event in the timed queue whose data matches the passed
        /// predicate
        /// </summary>
        /// <remarks>The immediate queue is unaffected by this method</remarks>
        /// <param name="match"></param>
        /// <returns></returns>
        public bool RemoveFirst(Predicate<ITimedAction> match)
        {
            lock (_sync)
            {
                _minPriorityQueue.RemoveFirst(match);
                Monitor.Pulse(_sync);
            }
            return true;
        }

        public DateTime GetNextTime(Predicate<ITimedAction> match)
        {
            lock (_sync)
            {
                var keyValuePair = _minPriorityQueue.GetFirst(match);
                if (keyValuePair != null) return keyValuePair.Value.Key;
            }
            return DateTime.MinValue; ;
        }


        /// <summary>
        /// Removes all events in the timed queue whose data matches the passed
        /// predicate
        /// </summary>
        /// <remarks>The immediate queue is unaffected by this method</remarks>
        /// <param name="match"></param>
        /// <returns></returns>
        public bool RemoveAll(Predicate<ITimedAction> match)
        {
            lock (_sync)
            {
                _minPriorityQueue.RemoveAll(match);
                Monitor.Pulse(_sync);
            }
            return true;
        }


        public bool RemoveAll()
        {
            lock (_sync)
            {
                _minPriorityQueue.Clear();
                Monitor.Pulse(_sync);
            }
            return true;
        }

        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// Invoked when an unhandled exception is thrown from the consumer action
        /// </summary>
        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;


        private void DoExecute(ITimedAction taskData)
        {
            try
            {
                taskData.DoAction();
            }
            catch (ThreadInterruptedException)
            {
                //Log.Info("Interrupt in DoExecute");
                throw;
            }
            catch (Exception ex)
            {
                if (UnhandledException != null)
                    UnhandledException(this, new UnhandledExceptionEventArgs(ex, false));

                // TODO: consider to use it in else statement
                // else
                Log.Error(ex, "Failed to execute taskData");
            }

        }

        private void ThreadMain()
        {
            try
            {
                while (true)
                {
                    Monitor.Enter(_sync);
                    if (!_minPriorityQueue.IsEmpty)
                    {
                        var delay = _minPriorityQueue.MinKey - DateTime.UtcNow;
                        if (delay <= TimeSpan.Zero)
                        {
                            var kvp = _minPriorityQueue.DequeueMin();
                            Monitor.Exit(_sync);

                            kvp.Value.StartUtcTime = kvp.Key;
                            DoExecute(kvp.Value);

                            var interval = kvp.Value.Interval;
                            if (interval <= TimeSpan.Zero) continue;

                            // не нулевой интервал - ставим в очередь снова

                            DateTime nextTime = kvp.Key + interval;
                            while (nextTime < DateTime.UtcNow)
                            {
                                nextTime += interval;
                                Log.Info("Пропущено циклическое выполнение.");
                            }
                            EnqueueUtc(kvp.Value, nextTime);
                        }
                        else
                            Monitor.Wait(_sync, (int)Math.Min(int.MaxValue, delay.TotalMilliseconds));
                    }
                    else
                        Monitor.Wait(_sync);
                }
            }
            catch (ThreadInterruptedException)
            {
                Log.Trace("ThreadInterruptedException occurred in TimedQueue during ThreadMain method");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred in TimedQueue during ThreadMain method");
            }
        }
    }
}
