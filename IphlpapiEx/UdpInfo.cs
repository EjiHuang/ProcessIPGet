using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Linq;
using System.Net.NetworkInformation;

namespace IphlpapiEx
{
    public static class UdpInfo
    {
        #region Win32Api

        /// <summary>
        /// Iphlpapi.dll win32Api
        /// </summary>
        public static class SafeNativeMethods
        {
            /// <summary>
            /// GetExtendedUdpTable
            /// </summary>
            /// <seealso cref="https://docs.microsoft.com/en-us/windows/desktop/api/iphlpapi/nf-iphlpapi-getextendedudptable"/>
            /// <returns>
            /// 0 ... NO_ERROR, 
            /// 122 ... ERROR_INSUFFICIENT_BUFFER, 
            /// 87 ... ERROR_INVALID_PARAMETER
            /// </returns>
            [DllImport("iphlpapi.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.U4)]
            public static extern uint GetExtendedUdpTable(
                IntPtr pUdpTable,
                out uint dwOutBufLen,
                bool sort,
                uint ipVersion,
                UDP_TABLE_CLASS tableClass,
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
        /// Enum about udp table info
        /// </summary>
        public enum UDP_TABLE_CLASS
        {
            UDP_TABLE_BASIC,
            UDP_TABLE_OWNER_PID,
            UDP_TABLE_OWNER_MODULE
        }

        /// <summary>
        /// UDP_TABLE_CLASS.MIB_UDPROW_OWNER_PID
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_UDPROW_OWNER_PID
        {
            public uint localAddr;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] localPort;

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
        }

        public struct MIB_UDPTABLE_OWNER_PID
        {
            public int dwNumEntries;
            public MIB_UDPROW_OWNER_PID[] table;
        }

        #endregion

        public static List<MIB_UDPROW_OWNER_PID> GetAllUdpConnections(
            AF_INET ipVersion = AF_INET.V4)
        {
            List<MIB_UDPROW_OWNER_PID> rows;

            var ret = SafeNativeMethods.GetExtendedUdpTable(
                IntPtr.Zero,
                out uint buff_size,
                true,
                (uint)ipVersion,
                UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID);
            var pTableBuff = Marshal.AllocHGlobal((int)buff_size);

            try
            {
                ret = SafeNativeMethods.GetExtendedUdpTable(
                pTableBuff,
                out buff_size,
                true,
                (uint)ipVersion,
                UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID);

                if (ret != 0)
                    return null;

                var udpTable = (MIB_UDPTABLE_OWNER_PID)Marshal.PtrToStructure(
                    pTableBuff,
                    typeof(MIB_UDPTABLE_OWNER_PID));
                IntPtr pRow = (IntPtr)((long)pTableBuff + Marshal.SizeOf(udpTable.dwNumEntries));

                rows = new List<MIB_UDPROW_OWNER_PID>();

                for (int i = 0; i < udpTable.dwNumEntries; i++)
                {
                    rows.Add(
                        (MIB_UDPROW_OWNER_PID)Marshal.PtrToStructure(
                            pRow,
                            typeof(MIB_UDPROW_OWNER_PID)));
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
        /// Get udp connections with pid
        /// </summary>
        /// <param name="ipVersion"></param>
        public static IEnumerable<UdpConnectionInformationEx> GetActiveTcpConnectionEx(AF_INET ipVersion = AF_INET.V4)
        {
            return from conn in GetAllUdpConnections(ipVersion)
                   select new UdpConnectionInformationEx(
                       remoteIp: new IPAddress(0),
                       localIp: conn.LocalAddress,
                       remotePort: 0,
                       localPort: conn.LocalPort,
                       state: TcpState.Listen,
                       pid: (int)conn.ProcessId);
        }
    }
}
