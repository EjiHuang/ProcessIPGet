using DevExpress.Mvvm;
using IphlpapiEx.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using IphlpapiEx.Services;

/*
 filePath = process.MainModule.FileName;
                    image = Imaging.CreateBitmapSourceFromHIcon(System.Drawing.Icon.ExtractAssociatedIcon(filePath).Handle,
                        Int32Rect.Empty, BitmapSizeOptions.FromRotation(Rotation.Rotate0));
     */
namespace IphlpapiEx.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Public Fields

        /// <summary>
        /// The list of process ip infomations.
        /// </summary>
        public ObservableCollection<Connection> Connections
        {
            get => GetProperty(() => Connections);
            set => SetProperty(() => Connections, value);
        }

        /// <summary>
        /// Number of tcp connections.
        /// </summary>
        public int NumberOfTcp
        {
            get => GetProperty(() => NumberOfTcp);
            set => SetProperty(() => NumberOfTcp, value);
        }

        /// <summary>
        /// Number of udp connections.
        /// </summary>
        public int NumberOfUdp
        {
            get => GetProperty(() => NumberOfUdp);
            set => SetProperty(() => NumberOfUdp, value);
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Refresh tcp/udp info timer.
        /// </summary>
        private readonly DispatcherTimer _refreshInfoTimer =
            new DispatcherTimer { Interval = TimeSpan.FromSeconds(1), IsEnabled = true };

        /// <summary>
        /// Connections Service
        /// </summary>
        private readonly ConnectionsService _connectionsService;

        #endregion

        #region Constructor

        public MainViewModel()
        {
            // Init
            Connections = new ObservableCollection<Connection>();
            _connectionsService = new ConnectionsService();
            // Get the tcp table
            _refreshInfoTimer.Tick += (object sender, EventArgs e) => GetConnectionTables();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the tcp tables and refresh the connection list.
        /// </summary>
        public void GetConnectionTables()
        {
            // If they are lot of data, please use AddRange function.
            var tcpUdpList = _connectionsService.GetTcpTableV4Ex().ToList();
            // Record number of tcp connections.
            NumberOfTcp = tcpUdpList.Count;
            // Combine two list to one.
            _connectionsService.GetUdpTableV4Ex().ToList().ForEach(udpInfo => tcpUdpList.Add(udpInfo));
            // Sort them by pid.If you need descending sort, swap x and y on the right-hand side of the arrow =>.
            tcpUdpList.Sort((x, y) => x.ProcessName.CompareTo(y.ProcessName));
            // Replace check.
            for (int i = 0; i < Connections.Count - tcpUdpList.Count; i++)
            {
                Connections.RemoveAt(tcpUdpList.Count);
            }
            for (int i = 0; i < tcpUdpList.Count; i++)
            {
                if (i < Connections.Count && Connections[i] != tcpUdpList[i])
                {
                    Connections[i] = tcpUdpList[i];
                }
                else
                {
                    Connections.Add(tcpUdpList[i]);
                }
            }

            // Record number of udp connections.
            NumberOfUdp = tcpUdpList.Count - NumberOfTcp;
        }

        #endregion
    }
}
