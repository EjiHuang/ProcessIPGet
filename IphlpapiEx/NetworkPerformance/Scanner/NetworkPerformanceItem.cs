using System.Net;

namespace IphlpapiEx.NetworkPerformance.Scanner
{
    public class NetworkPerformanceItem
    {
        public uint Pid { get; set; }

        public string ProcessName { get; set; }

        public string ConnectionType { get; set; }

        public string State { get; set; }

        public IPEndPoint LocalEndPoint { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }

        public uint LocalPort { get; set; }

        public uint RemotePort { get; set; }

        public long BytesOut { get; set; }

        public long BytesIn { get; set; }

        public long OutboundBandwidth { get; set; }

        public long InboundBandwidth { get; set; }

        public uint Pass = 0;

        public string CollectionTime { get; set; }
    }
}
