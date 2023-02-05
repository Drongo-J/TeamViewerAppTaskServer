using OurTeamViewer.Commands;
using OurTeamViewer.Helpers;
using OurTeamViewer.Models;
using OurTeamViewer.NetworkHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OurTeamViewer.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public RelayCommand StartServerCommand { get; set; }

        private ObservableCollection<Client> allClients;

        public ObservableCollection<Client> AllClients
        {
            get { return allClients; }
            set { allClients = value; OnPropertyChanged(); }
        }

        public HomeViewModel()
        {

            StartServerCommand = new RelayCommand((obj) =>
            {
                // Create folder for images

                ImageHelper helper = new ImageHelper();

                // Start server in a new thread
                Task.Run(() =>
                {
                    Network.Connect();
                }).Wait(100);

                // Show clients in a new thread
                Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(3000);
                        await App.Current.Dispatcher.BeginInvoke(() =>
                        {
                            try
                            {
                                AllClients = new ObservableCollection<Client>(Network.Clients);
                                foreach (var item in Network.Clients)
                                {
                                    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\{item.TcpClient.Client.RemoteEndPoint.ToString().Replace(":", ".")}.Images";
                                    if (!Directory.Exists(folderPath))
                                    {
                                        helper.CreateFolder(folderPath);
                                    }

                                    Task.Run(() =>
                                    {
                                        var stream = item.TcpClient.GetStream();
                                        var br = new BinaryReader(stream);
                                        while (true)
                                        {
                                            try
                                            {
                                                br = new BinaryReader(stream);
                                                var imageBytes = br.ReadBytes(500000);
                                                var path = helper.GetImagePath(imageBytes, folderPath);

                                                App.Current.Dispatcher.BeginInvoke(() =>
                                                {
                                                    AllClients[Network.Clients.IndexOf(item)] = new Client
                                                    {
                                                        TcpClient = item.TcpClient,
                                                        Title = "Monitor " + item.TcpClient.Client.RemoteEndPoint.ToString(),
                                                        ImagePath = path
                                                    };
                                                });
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine($"{item.TcpClient.Client.RemoteEndPoint}  disconnected");
                                            }
                                        }
                                    });
                                }
                            }
                            catch (Exception)
                            {
                                AllClients.Clear();
                            }
                        });
                    }
                });
            });
        }
    }
}
