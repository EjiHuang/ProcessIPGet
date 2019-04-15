using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Threading.Tasks;

namespace IphlpapiEx.NetworkPerformance.ETW
{
    public sealed class NetworkPerformanceETW : IDisposable
    {
        #region Private Fields

        private DateTime _etwStartTime;

        private TraceEventSession _etwSession;

        private readonly Counter _counter = new Counter();

        #endregion

        #region Constructor

        private NetworkPerformanceETW(int pid)
            => Initialise(pid);

        #endregion

        #region Public Fields

        public static NetworkPerformanceETW Create(int pid)
            => new NetworkPerformanceETW(pid);

        #endregion

        #region Public Methods

        public NetworkPerformanceData GetNetworkPerformanceData()
        {
            var timeDifferenceInSeconds = (DateTime.Now - _etwStartTime).TotalSeconds;

            NetworkPerformanceData performanceData;

            lock (_counter)
            {
                performanceData = new NetworkPerformanceData
                {
                    BytesRecv = Convert.ToInt64(_counter.Recv / (timeDifferenceInSeconds <= 0 ? 1 : timeDifferenceInSeconds)),
                    BytesSent = Convert.ToInt64(_counter.Sent / (timeDifferenceInSeconds <= 0 ? 1 : timeDifferenceInSeconds))
                };
            }

            ResetCounter();
            return performanceData;
        }

        public void Dispose()
        {
            _etwSession?.Dispose();
        }

        #endregion

        #region Private Methods

        private void Initialise(int pid)
        {
            // 请注意，ETW类会阻止处理消息，因此如果您希望应用程序保持响应，则应在不同的线程上运行。
            Task.Run(() =>
            {
                try
                {
                    var processID = pid;
                    ResetCounter();

                    using (_etwSession = new TraceEventSession("MyKernelAndClrEventsSession"))
                    {
                        _etwSession.EnableKernelProvider(KernelTraceEventParser.Keywords.NetworkTCPIP);

                        _etwSession.Source.Kernel.TcpIpRecv += data =>
                        {
                            if (data.ProcessID == processID)
                            {
                                lock (_counter)
                                {
                                    _counter.Recv += data.size;
                                }
                            }
                        };

                        _etwSession.Source.Kernel.TcpIpSend += data =>
                        {
                            if (data.ProcessID == processID)
                            {
                                lock (_counter)
                                {
                                    _counter.Sent += data.size;
                                }
                            }
                        };

                        _etwSession.Source.Process();
                    }
                }
                catch
                {
                    ResetCounter();
                }
            });
        }

        private void ResetCounter()
        {
            lock (_counter)
            {
                _counter.Sent = 0;
                _counter.Recv = 0;
            }
            _etwStartTime = DateTime.Now;
        }

        #endregion
    }
}
