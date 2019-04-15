using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace IphlpapiEx.NetworkPerformance.Scanner
{
    public static class NativeMethod
    {
        #region Win32Api

        /// <summary>
        /// GetExtendedTcpTable
        /// </summary>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365928(v=vs.85).aspx"/>
        /// <returns>
        /// 0 ... NO_ERROR, 
        /// 122 ... ERROR_INSUFFICIENT_BUFFER, 
        /// 87 ... ERROR_INVALID_PARAMETER
        /// </returns>
        [DllImport("iphlpapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern uint GetExtendedTcpTable(
            IntPtr pTcpTable,
            out uint dwOutBufLen,
            bool sort,
            uint ipVersion,
            TCP_TABLE_CLASS tableClass,
            uint reserved = 0);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern uint GetPerTcpConnectionEStats(IntPtr row, TCP_ESTATS_TYPE statsType,
            IntPtr rw, uint rwVersion, uint rwSize,
            IntPtr ros, uint rosVersion, uint rosSize,
            IntPtr rod, uint rodVersion, uint rodSize);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern uint SetPerTcpConnectionEStats(IntPtr row, TCP_ESTATS_TYPE statsType,
            IntPtr rw, uint rwVersion, uint rwSize, uint offset);

        /// <summary>
        /// The TCP_ESTATS_TYPE enumeration defines the type of extended statistics for 
        /// a TCP connection that is requested or being set.
        /// </summary>
        /// <seealso cref="https://docs.microsoft.com/en-us/windows/desktop/api/tcpestats/ne-tcpestats-tcp_estats_type"/>
        public enum TCP_ESTATS_TYPE
        {
            TcpConnectionEstatsSynOpts,
            TcpConnectionEstatsData,
            TcpConnectionEstatsSndCong,
            TcpConnectionEstatsPath,
            TcpConnectionEstatsSendBuff,
            TcpConnectionEstatsRec,
            TcpConnectionEstatsObsRec,
            TcpConnectionEstatsBandwidth,
            TcpConnectionEstatsFineRtt,
            TcpConnectionEstatsMaximum,
        }

        /// <summary>
        /// Ip version
        /// </summary>
        public enum AF_INET
        {
            V4 = 2, // IP_v4 = System.Net.Sockets.AddressFamily.InterNetwork
            V6 = 23 // IP_v6 = System.Net.Sockets.AddressFamily.InterNetworkV6
        }

        /// <summary>
        /// Enum about tcp table info
        /// </summary>
        public enum TCP_TABLE_CLASS
        {
            TCP_TABLE_BASIC_LISTENER,
            TCP_TABLE_BASIC_CONNECTIONS,
            TCP_TABLE_BASIC_ALL,
            TCP_TABLE_OWNER_PID_LISTENER,
            TCP_TABLE_OWNER_PID_CONNECTIONS,
            TCP_TABLE_OWNER_PID_ALL,
            TCP_TABLE_OWNER_MODULE_LISTENER,
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,
            TCP_TABLE_OWNER_MODULE_ALL
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW_OWNER_PID
        {
            public uint state;
            public uint localAddr;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] localPort;

            public uint remoteAddr;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] remotePort;

            public uint owningPid;

            public uint ProcessId
            {
                get { return owningPid; }
            }

            public IPAddress LocalAddress
            {
                get { return new IPAddress(localAddr); }
            }

            public ushort LocalPort
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[2] { localPort[1], localPort[0] }, 0);
                }
            }

            public IPAddress RemoteAddress
            {
                get { return new IPAddress(remoteAddr); }
            }

            public ushort RemotePort
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[2] { remotePort[1], remotePort[0] }, 0);
                }
            }

            public TcpState TcpState
            {
                get
                {
                    if (state > 0 && state < 13) return (TcpState)state;
                    else return TcpState.Unknown;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPTABLE_OWNER_PID
        {
            public uint dwNumEntries;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
            public MIB_TCPROW_OWNER_PID[] table;
        }

        #endregion
    }
}
