using System.Net;
using System.Net.NetworkInformation;

namespace IphlpapiEx
{
    /// <summary>
    /// Reference
    /// https://github.com/ssashir06/TcpConnectionInformationEx autor：ssashir06
    /// TcpConnectionInformation class extend
    /// </summary>
    public class TcpConnectionInformationEx : TcpConnectionInformation
    {
        #region Private Fields

        /// <summary>
        /// Connection state
        /// </summary>
        private readonly TcpState _state;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionInformationEx"/> class.
        /// </summary>
        /// <param name="remoteIp">Remote IP address and port.</param>
        /// <param name="localIp">Local IP address and port.</param>
        /// <param name="state">Connection State.</param>
        /// <param name="pid">Process Id.</param>
        public TcpConnectionInformationEx(
            IPAddress remoteIp,
            IPAddress localIp,
            int remotePort,
            int localPort,
            TcpState state,
            int pid)
        {
            RemoteIP = remoteIp;
            LocalIP = localIp;
            RemotePort = remotePort;
            LocalPort = localPort;
            _state = state;
            Pid = pid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionInformationEx" /> class.
        /// </summary>
        /// <param name="src">The source.</param>
        protected TcpConnectionInformationEx(TcpConnectionInformationEx src)
        {
            RemoteIP = src.RemoteIP;
            LocalIP = src.LocalIP;
            _state = src._state;
            RemotePort = src.RemotePort;
            LocalPort = src.LocalPort;
            Pid = src.Pid;
        }

        #endregion

        #region Public Fields

        /// <summary>
        /// Get its process id.
        /// </summary>
        public int Pid { get; }

        /// <summary>
        /// Get its local ip address.
        /// </summary>
        public IPAddress LocalIP { get; }

        /// <summary>
        /// Get its remote ip address.
        /// </summary>
        public IPAddress RemoteIP { get; }

        /// <summary>
        /// Get its local port;
        /// </summary>
        public int LocalPort { get; }

        /// <summary>
        /// Get its remote port;
        /// </summary>
        public int RemotePort { get; }

        #endregion

        #region Override Fields

        /// <summary>
        /// Get local endpoint of Transmission Control Protocol (TCP) connection.
        /// </summary>
        public override IPEndPoint LocalEndPoint => new IPEndPoint(LocalIP, LocalPort);

        /// <summary>
        /// Get remote endpoint of Transmission Control Protocol (TCP) connection.
        /// </summary>
        public override IPEndPoint RemoteEndPoint => new IPEndPoint(RemoteIP, RemotePort);

        /// <summary>
        /// Get state of Transmission Control Protocol (TCP) connection.
        /// </summary>
        public override TcpState State => _state;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Pid;
        }

        #endregion
    }
}
