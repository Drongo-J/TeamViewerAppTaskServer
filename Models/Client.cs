using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OurTeamViewer.Models
{
    public class Client
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public TcpClient TcpClient { get; set; }
    }
}
