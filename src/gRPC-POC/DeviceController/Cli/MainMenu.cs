using Grpc.Core;
using Gsdk.Connect;
using System.Runtime.CompilerServices;

namespace example
{
    public class MainMenu
    {
        private Menu menu;
        private ConnectSvc connectSvc;
        private DeviceMenu deviceMenu;
        private AsyncMenu asyncMenu;
        private AcceptMenu acceptMenu;
        
        public MainMenu(ConnectSvc svc)
        {
            connectSvc = svc;

            MenuItem[] items = new MenuItem[7];
            items[0] = new MenuItem("1", "Search devices", SearchDevice, false);
            items[1] = new MenuItem("2", "Connect to a device synchronously", ConnectToDeviceAsync, false);
            items[2] = new MenuItem("3", "Manage asynchronous connections", ShowAsyncMenu, false);
            items[3] = new MenuItem("4", "Accept devices", ShowAcceptMenuAsync, false);
            items[4] = new MenuItem("5", "Device menu", ShowDeviceMenuAsync, false);
            items[5] = new MenuItem("6", "Events", StreamEventsAsync, false);
            items[6] = new MenuItem("q", "Quit", null, true);

            menu = new Menu(items);

            deviceMenu = new DeviceMenu(svc);
            asyncMenu = new AsyncMenu(svc);
            acceptMenu = new AcceptMenu(svc);
        }

        public void Show()
        {
            menu.Show("Main Menu");
        }

        public async Task SearchDevice()
        {
            Console.WriteLine("Searching devices in the subnet...");

            try
            {
                var devList = await connectSvc.SearchDeviceAsync();

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

                    uint devID = await connectSvc.ConnectAsync(connInfo);
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
            uint devID = await connectSvc.ConnectAsync(connInfo);

            GatewayClient gatewayClient = new GatewayClient();
            
            EventSvc eventSvc = new EventSvc(gatewayClient.GetChannel());

            uint[] devIDs = { devID };

            try
            {
                LogCli logTest = new LogCli(eventSvc);

                await logTest.EventsLogggerAsync(devID);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the event test for device {0}: {1}", devID, e);
            }
            finally
            {
                await connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }

        public async Task ShowDeviceMenuAsync()
        {
            await deviceMenu.ShowAsync();
        }

        public async Task ShowAsyncMenu()
        {
            await asyncMenu.Show();
        }

        public async Task ShowAcceptMenuAsync()
        {
            await acceptMenu.ShowAsync();
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


