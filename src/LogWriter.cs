using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Niolog.Interfaces;

namespace Niolog
{
    public abstract class LogWriter : ILogWriter, IDisposable
    {
        protected ConcurrentQueue<ITagger> queue = new ConcurrentQueue<ITagger>();
        protected List<Task> consumerTasks = new List<Task>();
        /// <summary>
        /// Log consumption with {batch} logs
        /// </summary>
        protected int batch;
        /// <summary>
        /// Number of log consuming threads
        /// </summary>
        protected int concurrent;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private int activeTask;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch">Log consumption with {batch} logs</param>
        /// <param name="concurrent">Number of log consuming threads</param>
        public LogWriter(int batch, int concurrent)
        {
            this.batch = batch;
            this.concurrent = concurrent;

            for(var i = 0; i < this.concurrent; i++)
            {
                this.consumerTasks.Add(Task.Run(() =>
                {
                    List<ITagger> logs = new List<ITagger>();
                    while(true)
                    {
                        logs.Clear();
                        SpinWait.SpinUntil(() => this.queue.Count > 0);
                        Interlocked.Increment(ref this.activeTask);

                        while(logs.Count < this.batch && this.queue.Count > 0)
                        {
                            if(this.queue.TryDequeue(out ITagger log))
                            {
                                logs.Add(log);
                            }
                        }
                        
                        this.Consume(logs);
                        Interlocked.Decrement(ref this.activeTask);
                    }
                }, this.tokenSource.Token));
            }
        }

        public virtual void Dispose()
        {
            this.tokenSource.Cancel();
            this.tokenSource.Dispose();
        }

        public bool Finished()
        {
            return this.queue.Count <= 0 && this.activeTask <= 0;
        }

        public void Write(ITagger tagger)
        {
            this.queue.Enqueue(tagger);
        }

        protected abstract void Consume(List<ITagger> taggers);
    }
}