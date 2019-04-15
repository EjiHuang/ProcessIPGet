using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketCaptureEx
{
    public class PacketCaptureEx
    {

        #region Public fields

        /// <summary>
        /// Send packet's length per second.
        /// </summary>
        public int SendPerSecLen { set; get; }

        /// <summary>
        /// Receive packet's length per second.
        /// </summary>
        public int RevPerSecLen { set; get; }

        #endregion

        #region Private fields

        /// <summary>
        /// Capture device.
        /// </summary>
        private readonly ICaptureDevice _device;

        #endregion

        #region Constructor

        public PacketCaptureEx()
        {
            _device = CaptureDeviceList.New()[0];
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Capture flow send.
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="PortID"></param>
        public void CaptureFlowSend(string IP, int PortID)
        {
            // When the packet arrival. Invoke this cation.
            _device.OnPacketArrival += (sender, e) => SendPerSecLen = e.Packet.Data.Length;
            // Mode and timeout setting.
            _device.Open(DeviceMode.Promiscuous, read_timeout: 1000);
            // Filter setting.
            _device.Filter = string.Format("src host {0} and src port {1}", IP, PortID);
            // Start capture.
            _device.StartCapture();
        }

        /// <summary>
        /// Capture flow received.
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="PortID"></param>
        public void CaptureFlowRecv(string IP, int PortID)
        {
            // When the packet arrival. Invoke this cation.
            _device.OnPacketArrival += (sender, e) => SendPerSecLen = e.Packet.Data.Length;
            // Mode and timeout setting.
            _device.Open(DeviceMode.Promiscuous, read_timeout: 1000);
            // Filter setting.
            _device.Filter = string.Format("dst host {0} and dst port {1}", IP, PortID);
            // Start capture.
            _device.StartCapture();
        }

        #endregion
    }
}
