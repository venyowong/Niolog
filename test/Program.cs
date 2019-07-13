using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Niolog;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // var logger = NiologManager.CreateLogger(new ConsoleLogWriter());
            // Console.WriteLine(logger == NiologManager.Logger);

            // Task.Run(() =>
            // {
            //     Console.WriteLine(NiologManager.Logger == null);
            //     var logger2 = NiologManager.CreateLogger(new ConsoleLogWriter());
            //     Console.WriteLine(logger == logger2);
            // });

            using(var logWriter = new HttpLogWriter("http://localhost:9615/store", 10, 2))
            {
                var logger = NiologManager.CreateLogger(logWriter);
                for(var i = 0; i < 20; i++)
                {
                    logger.Trace()
                        .Message($"test{i}")
                        .SetTag("Index", i.ToString())
                        .Write();
                }

                SpinWait.SpinUntil(logWriter.Finished);
                Console.WriteLine("Finished");
            }
        }
    }
}
