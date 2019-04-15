﻿using System.Net;

namespace IphlpapiEx.Models
{
    public class Connection
    {
        public int Pid { get; set; }

        public string ProcessName { get; set; }

        public string ConnectionType { get; set; }

        public string State { get; set; }

        public IPEndPoint LocalEndPoint { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }

        public IPAddress LocalIPAddr { get; set; }

        public IPAddress RemoteIPAddr { get; set; }

        public int LocalPort { get; set; }

        public int RemotePort { get; set; }

        public long NetSendBytes { get; set; }

        public long NetRecvBytes { get; set; }
    }
}
