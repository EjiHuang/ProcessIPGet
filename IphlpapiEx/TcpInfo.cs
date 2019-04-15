using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace IphlpapiEx
{
    public static class TcpInfo
    {
        #region Win32Api

        /// <summary>
        /// Iphlpapi.dll win32Api
        /// </summary>
        public static class SafeNativeMethods
        {
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

        /// <summary>
        /// Get all tcp connections info
        /// </summary>
        /// <param name="ipVersion"></param>
        /// <returns>The list of MIB_TCPROW_OWNER_PID</returns>
        public static List<MIB_TCPROW_OWNER_PID> GetAllTcpConnections(
            AF_INET ipVersion = AF_INET.V4)
        {
            List<MIB_TCPROW_OWNER_PID> rows;

            uint ret = SafeNativeMethods.GetExtendedTcpTable(
                IntPtr.Zero,
                out uint buff_size,
                true,
                (uint)ipVersion,
                TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
            var pTableBuff = Marshal.AllocHGlobal((int)buff_size);

            try
            {
                ret = SafeNativeMethods.GetExtendedTcpTable(
                    pTableBuff,
                    out buff_size,
                    true,
                    (uint)ipVersion,
                    TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);

                if (ret != 0)
                    return null;

                var tcpTable = (MIB_TCPTABLE_OWNER_PID)Marshal.PtrToStructure(
                    pTableBuff,
                    typeof(MIB_TCPTABLE_OWNER_PID));
                IntPtr pRow = (IntPtr)((long)pTableBuff + Marshal.SizeOf(tcpTable.dwNumEntries));
                // rows = new List<MIB_TCPROW_OWNER_PID>((int)tcpTable.dwNumEntries);
                rows = new List<MIB_TCPROW_OWNER_PID>();

                for (int i = 0; i < tcpTable.dwNumEntries; i++)
                {
                    rows.Add(
                        (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(
                            pRow,
                            typeof(MIB_TCPROW_OWNER_PID)));
                    // ptr -> next
                    pRow = (IntPtr)((long)pRow + Marshal.SizeOf(rows[i]));
                }
            }
            finally
            {
                // free all buff when u new them before
                Marshal.FreeHGlobal(pTableBuff);
            }
            return rows;
        }

        /// <summary>
        /// Get tcp connections with pid
        /// </summary>
        /// <returns>Enumerable of TcpConnectionInformation</returns>
        public static IEnumerable<TcpConnectionInformationEx> GetActiveTcpConnectionEx(AF_INET ipVersion = AF_INET.V4)
        {
            return from conn in GetAllTcpConnections(ipVersion)
                   select new TcpConnectionInformationEx(
                       remoteIp: conn.RemoteAddress,
                       localIp: conn.LocalAddress,
                       remotePort: conn.RemotePort,
                       localPort: conn.LocalPort,
                       state: conn.TcpState,
                       pid: (int)conn.ProcessId);
        }
    }
}
