using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Threading.Tasks;

namespace T_NetworkPerformance
{
    public sealed class NetworkPerformanceReporter : IDisposable
    {
        private DateTime _EtwStartTime;

        private TraceEventSession _EtwSession;

        private readonly Counters _Counters = new Counters();

        private class Counters
        {
            public long Received;
            public long Sent;
        }

        private NetworkPerformanceReporter() { }

        public static NetworkPerformanceReporter Create()
        {
            var npr = new NetworkPerformanceReporter();
            npr.Initialise();
            return npr;
        }

        private void Initialise()
        {
            // 请注意，ETW类会阻止处理消息，因此如果您希望应用程序保持响应，则应在不同的线程上运行。
            Task.Run(() => StartEtwSession());
        }

        private void StartEtwSession()
        {
            try
            {
                // var pid = Process.GetCurrentProcess().Id;
                var pid = 15908;
                ResetCounters();

                using (_EtwSession = new TraceEventSession("MyKernelAndClrEventsSession"))
                {
                    _EtwSession.EnableKernelProvider(KernelTraceEventParser.Keywords.NetworkTCPIP);

                    _EtwSession.Source.Kernel.TcpIpRecv += data =>
                    {
                        if (data.ProcessID == pid)
                        {
                            lock (_Counters)
                            {
                                _Counters.Received += data.size;
                            }
                        }
                    };

                    _EtwSession.Source.Kernel.TcpIpSend += data =>
                    {
                        if (data.ProcessID == pid)
                        {
                            lock (_Counters)
                            {
                                _Counters.Sent += data.size;
                            }
                        }
                    };

                    _EtwSession.Source.Process();
                }
            }
            catch
            {
                ResetCounters();
            }
        }

        public NetworkPerformanceData GetNetworkPerformanceData()
        {
            var timeDifferenceInSeconds = (DateTime.Now - _EtwStartTime).TotalSeconds;

            NetworkPerformanceData networkData;

            lock (_Counters)
            {
                networkData = new NetworkPerformanceData
                {
                    BytesReceived = Convert.ToInt64(_Counters.Received / timeDifferenceInSeconds),
                    BytesSent = Convert.ToInt64(_Counters.Sent / timeDifferenceInSeconds)
                };

            }

            // Reset the counters to get a fresh reading for next time this is called.
            ResetCounters();

            return networkData;
        }

        public sealed class NetworkPerformanceData
        {
            public long BytesReceived { get; set; }
            public long BytesSent { get; set; }
        }


        private void ResetCounters()
        {
            lock (_Counters)
            {
                _Counters.Sent = 0;
                _Counters.Received = 0;
            }
            _EtwStartTime = DateTime.Now;
        }

        public void Dispose()
        {
            _EtwSession?.Dispose();
        }
    }
}
