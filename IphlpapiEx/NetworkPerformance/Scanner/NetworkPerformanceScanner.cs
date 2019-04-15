//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using static IphlpapiEx.NetworkPerformance.Scanner.NativeMethod;

//namespace IphlpapiEx.NetworkPerformance.Scanner
//{
//    public class NetworkPerformanceScanner
//    {
//        #region Public Methods

//        public List<NetworkPerformanceItem> ScanNetworkPerformance(AF_INET ipVersion = AF_INET.V4)
//        {
//            List<NetworkPerformanceItem> networkPerformanceItems;
//            List<MIB_TCPROW_OWNER_PID> rows;

//            uint ret = GetExtendedTcpTable(
//                IntPtr.Zero,
//                out uint buff_size,
//                true,
//                (uint)ipVersion,
//                TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
//            var pTableBuff = Marshal.AllocHGlobal((int)buff_size);

//            try
//            {
//                ret = GetExtendedTcpTable(
//                    pTableBuff,
//                    out buff_size,
//                    true,
//                    (uint)ipVersion,
//                    TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);

//                if (ret != 0)
//                    return null;

//                var tcpTable = (MIB_TCPTABLE_OWNER_PID)Marshal.PtrToStructure(
//                    pTableBuff,
//                    typeof(MIB_TCPTABLE_OWNER_PID));
//                IntPtr pRow = (IntPtr)((long)pTableBuff + Marshal.SizeOf(tcpTable.dwNumEntries));

//                rows = new List<MIB_TCPROW_OWNER_PID>();
//                networkPerformanceItems = new List<NetworkPerformanceItem>();
//                for (int i = 0; i < tcpTable.dwNumEntries; i++)
//                {
//                    var row = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(pRow, typeof(MIB_TCPROW_OWNER_PID));

//                    networkPerformanceItems[i].Pid = row.ProcessId;
//                    networkPerformanceItems[i].State = row.TcpState.ToString();
//                    networkPerformanceItems[i].LocalEndPoint = new IPEndPoint(row.LocalAddress, row.LocalPort);
//                    networkPerformanceItems[i].LocalPort = row.LocalPort;
//                    networkPerformanceItems[i].RemoteEndPoint = new IPEndPoint(row.RemoteAddress, row.RemotePort);
//                    networkPerformanceItems[i].RemotePort = row.RemotePort;

//                    if (0 != row.remoteAddr)
//                    {
//                        var buffRow = IntPtr.Zero;
//                        var buffRW = IntPtr.Zero;
//                        var buffROD = IntPtr.Zero;

//                        try
//                        {
//                            buffRow = Marshal.AllocHGlobal(Marshal.SizeOf(row));
//                            Marshal.StructureToPtr(row, buffRow, false);
//                            buffRW = Marshal.AllocHGlobal(Marshal.SizeOf(TRW));
//                        }
//                        finally
//                        {
                            
//                        }
//                    }

//                    // ptr -> next
//                    pRow = (IntPtr)((long)pRow + Marshal.SizeOf(rows[i]));
//                }
//            }
//            finally
//            {
//                // free all buff when u new them before
//                Marshal.FreeHGlobal(pTableBuff);
//            }
//        }

//        #endregion
//    }
//}
