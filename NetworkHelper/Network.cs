using OurTeamViewer.Helpers;
using OurTeamViewer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OurTeamViewer.NetworkHelper
{
    public class Network
    {
        public static int PORT = 27001;

        static TcpListener listener = null;
        static BinaryWriter bw = null;
        static BinaryReader br = null;
        public static List<Client> Clients { get; set; }

        public static string GetHostName()
        {
            string hostName = Dns.GetHostName();
            return hostName;
        }

        public static string GetLocalIpAddress()
        {
            UnicastIPAddressInformation mostSuitableIp = null;
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;
                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                    continue;
                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(address.Address))
                        continue;
                    if (!address.IsDnsEligible)
                    {
                        if (mostSuitableIp == null)
                            mostSuitableIp = address;
                        continue;
                    }
                    // The best IP is the IP got from DHCP server
                    if (address.PrefixOrigin != PrefixOrigin.Dhcp)
                    {
                        if (mostSuitableIp == null || !mostSuitableIp.IsDnsEligible)
                            mostSuitableIp = address;
                        continue;
                    }
                    return address.Address.ToString();
                }
            }
            return mostSuitableIp != null
                ? mostSuitableIp.Address.ToString()
                : "";
        }

        public static void Connect()
        {
            var ip = GetLocalIpAddress();
            var ipAdress = IPAddress.Parse(ip);
            Clients = new List<Client>();
            var ep = new IPEndPoint(ipAdress, PORT);
            listener = new TcpListener(ep);
            listener.Start();
            MessageBox.Show($"Listening on {listener.LocalEndpoint}");
            while (true)
            {
                var client = listener.AcceptTcpClient();
                var newClient = new Client
                {
                    TcpClient = client,
                };
                Clients.Add(newClient);
                Task.Delay(1000);
            }
        }
    }
}

