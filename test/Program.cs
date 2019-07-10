using System;
using System.Threading.Tasks;
using Niolog;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = NiologManager.CreateLogger(new ConsoleLogWriter());
            Console.WriteLine(logger == NiologManager.Logger);

            Task.Run(() =>
            {
                Console.WriteLine(NiologManager.Logger == null);
                var logger2 = NiologManager.CreateLogger(new ConsoleLogWriter());
                Console.WriteLine(logger == logger2);
            });

            logger.Trace()
                .Message("test")
                .SetTag("key", "value")
                .Write();

            Console.Read();
        }
    }
}
