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
        /// 以 {batch} 条日志为单位进行日志消费
        /// </summary>
        protected int batch;
        /// <summary>
        /// 日志消费线程的并发量
        /// </summary>
        protected int concurrent;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch">以 {batch} 条日志为单位进行日志消费</param>
        /// <param name="concurrent">日志消费线程的并发量</param>
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

                        while(logs.Count < this.batch && this.queue.Count > 0)
                        {
                            if(this.queue.TryDequeue(out ITagger log))
                            {
                                logs.Add(log);
                            }
                        }
                        
                        this.Consume(logs);
                    }
                }, this.tokenSource.Token));
            }
        }

        public void Dispose()
        {
            this.tokenSource.Cancel();
            this.tokenSource.Dispose();
        }

        public bool Finished()
        {
            return this.queue.Count <= 0;
        }

        public void Write(ITagger tagger)
        {
            this.queue.Enqueue(tagger);
        }

        protected abstract void Consume(List<ITagger> taggers);
    }
}