using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.Connect;
using System.Runtime.CompilerServices;

namespace example
{
    public class MainMenu
    {
        private Menu _menu;
        private ConnectService _connectService;
        private DeviceMenu _deviceMenu;
        private AsyncMenu _asyncMenu;
        private AcceptMenu _acceptMenu;
        
        public MainMenu(ConnectService connectService)
        {
            _connectService = connectService;

            MenuItem[] items = new MenuItem[7];
            items[0] = new MenuItem("1", "Search devices", SearchDevice, false);
            items[1] = new MenuItem("2", "Connect to a device synchronously", ConnectToDeviceAsync, false);
            items[2] = new MenuItem("3", "Manage asynchronous connections", ShowAsyncMenu, false);
            items[3] = new MenuItem("4", "Accept devices", ShowAcceptMenuAsync, false);
            items[4] = new MenuItem("5", "Device menu", ShowDeviceMenuAsync, false);
            items[5] = new MenuItem("6", "Events", StreamEventsAsync, false);
            items[6] = new MenuItem("q", "Quit", null, true);

            _menu = new Menu(items);

            _deviceMenu = new DeviceMenu(connectService);
            
            _asyncMenu = new AsyncMenu(connectService);
            
            _acceptMenu = new AcceptMenu(connectService);
        }

        public async Task ShowAsync()
        {
            await _menu.ShowAsync("Main Menu");
        }

        public async Task SearchDevice()
        {
            Console.WriteLine("Searching devices in the subnet...");

            try
            {
                RepeatedField<SearchDeviceInfo> devList = await _connectService.SearchDeviceAsync().ConfigureAwait(false);

                Console.WriteLine();
                Console.WriteLine("***** Found Devices: {0}", devList.Count);
                for (int i = 0; i < devList.Count; i++)
                {
                    Console.WriteLine(devList[i]);
                }

                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot search devices: {0}", e);
            }
        }

        public async Task ConnectToDeviceAsync()
        {
            var connInfo = GetConnectInfo();

            if (connInfo != null)
            {
                try
                {
                    Console.WriteLine("Connecting to the device...");

                    uint devID = await _connectService.ConnectAsync(connInfo);
                    Console.WriteLine("Connected to {0}", devID);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot connect to the device: {0}", e);
                }
            }
        }

        public async Task StreamEventsAsync()
        {
            var connInfo = GetConnectInfo();
            uint devID = await _connectService.ConnectAsync(connInfo);

            GatewayClient gatewayClient = new GatewayClient();
            
            EventService eventSvc = new EventService(gatewayClient.GetChannel());

            uint[] devIDs = { devID };

            try
            {
                LogCli logTest = new LogCli(eventSvc);

                await logTest.EventsLoggerAsync(devID);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the event test for device {0}: {1}", devID, e);
            }
            finally
            {
                await _connectService.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }

        public async Task ShowDeviceMenuAsync()
        {
            await _deviceMenu.ShowAsync();
        }

        public async Task ShowAsyncMenu()
        {
            await _asyncMenu.Show();
        }

        public async Task ShowAcceptMenuAsync()
        {
            await _acceptMenu.ShowAsync();
        }

        public static ConnectInfo GetConnectInfo()
        {
            InputItem[] items = new InputItem[3];
            items[0] = new InputItem { text = "Enter the IP address of the device", defaultVal = "" };
            items[1] = new InputItem { text = "Enter the port of the device (default: 51211)", defaultVal = "51211" };
            items[2] = new InputItem { text = "Use SSL y/n (default: y)", defaultVal = "y" };

            var userInputs = Menu.GetUserInput(items);

            int port = 0;
            if (!Int32.TryParse(userInputs[1], out port))
            {
                Console.WriteLine("Invalid port number: {0}", userInputs[1]);
                return null;
            }

            bool useSSL = userInputs[2].Trim().ToLower().Equals("y");

            return new ConnectInfo { IPAddr = userInputs[0], Port = port, UseSSL = useSSL };
        }
    }
}


