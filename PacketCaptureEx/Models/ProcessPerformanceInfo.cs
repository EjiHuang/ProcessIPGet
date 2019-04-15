using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketCaptureEx.Models
{
    public class ProcessPerformanceInfo
    {
        public int ProcessID { get; set; }

        public long NetSendBytes { get; set; }

        public long NetRecvBytes { get; set; }

        public long NetTotalBytes { get; set; }
    }
}
