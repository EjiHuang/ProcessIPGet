using System;
using System.Threading;

namespace T_NetworkPerformance
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            NetworkPerformanceReporter reporter = NetworkPerformanceReporter.Create();

            Timer timer = new Timer(_ =>
            {
                var ret = reporter.GetNetworkPerformanceData();
                Console.WriteLine($"Bytes Received:{ret.BytesReceived}");
                Console.WriteLine($"Bytes Sent:{ret.BytesSent}");
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            Console.ReadLine();

            timer.Change(-1, -1); // Stop the timer from running.
        }
    }
}
