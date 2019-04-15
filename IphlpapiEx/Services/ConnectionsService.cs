using IphlpapiEx.Framworks;
using IphlpapiEx.Models;
using IphlpapiEx.NetworkPerformance.ETW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphlpapiEx.Services
{
    public class ConnectionsService
    {
        #region Private Fields

        public ConnectionsService()
        {

        }

        #endregion

        #region Constructor



        #endregion

        #region Private Methods

        /// <summary>
        /// Cached function to get process name with pid.
        /// </summary>
        private readonly CachedFunc<int, string> _process_cache =
            new CachedFunc<int, string>(pid =>
            {
                if (0 == pid)
                    return "Unknown";
                try
                {
                    using (Process p = Process.GetProcessById(pid))
                        return p.ProcessName;
                }
                catch
                {
                    return "Unknown";
                }
            })
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

        public List<Connection> GetTcpTableV4Ex()
        {
            List<Connection> tcpConnections = new List<Connection>();
            foreach (var tcpInfo in TcpInfo.GetActiveTcpConnectionEx(TcpInfo.AF_INET.V4))
            {
                NetworkPerformanceData etwData;
                if (tcpInfo.Pid == 6132)
                {
                    etwData = NetworkPerformanceETW.Create(tcpInfo.Pid).GetNetworkPerformanceData();
                }
                else
                {
                    etwData = new NetworkPerformanceData { BytesRecv = 0, BytesSent = 0 };
                }
                tcpConnections.Add(new Connection
                {
                    LocalEndPoint = tcpInfo.LocalEndPoint,
                    RemoteEndPoint = tcpInfo.RemoteEndPoint,
                    LocalIPAddr = tcpInfo.LocalIP,
                    LocalPort = tcpInfo.LocalPort,
                    RemoteIPAddr = tcpInfo.RemoteIP,
                    RemotePort = tcpInfo.RemotePort,
                    State = tcpInfo.State.ToString(),
                    Pid = tcpInfo.Pid,
                    ProcessName = _process_cache[tcpInfo.Pid],
                    ConnectionType = "TCP",
                    NetSendBytes = etwData.BytesSent,
                    NetRecvBytes = etwData.BytesRecv
                });
            }
            return tcpConnections;
        }

        public List<Connection> GetUdpTableV4Ex()
        {
            var query = from conn in
                            (
                            from udpInfo in UdpInfo.GetActiveTcpConnectionEx(UdpInfo.AF_INET.V4)
                            select new Connection
                            {
                                LocalEndPoint = udpInfo.LocalEndPoint,
                                RemoteEndPoint = udpInfo.RemoteEndPoint,
                                LocalIPAddr = udpInfo.LocalIP,
                                LocalPort = udpInfo.LocalPort,
                                RemoteIPAddr = udpInfo.RemoteIP,
                                RemotePort = udpInfo.RemotePort,
                                State = udpInfo.State.ToString(),
                                Pid = udpInfo.Pid,
                                ProcessName = _process_cache[udpInfo.Pid],
                                ConnectionType = "UDP"
                            }
                            )
                        select conn;
            return query.ToList();
        }

        #endregion
    }
}
