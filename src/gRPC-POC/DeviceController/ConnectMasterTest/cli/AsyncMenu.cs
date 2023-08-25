using Gsdk.Connect;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace example
{
    class AsyncMenu
    {
        private Menu menu;
        private ConnectMasterSvc connectMasterSvc;
        private string gatewayID;

        public AsyncMenu(ConnectMasterSvc svc, string id)
        {
            connectMasterSvc = svc;
            gatewayID = id;

            MenuItem[] items = new MenuItem[4];
            items[0] = new MenuItem("1", "Add async connections", AddConnectionAsync, false);
            items[1] = new MenuItem("2", "Delete async connections", DeleteConnectionAsync, false);
            items[2] = new MenuItem("3", "Refresh the connection list", ShowConnectionAsync, false);
            items[3] = new MenuItem("q", "Return to Main Menu", null, true);

            menu = new Menu(items);
        }

        public async Task ShowAsync()
        {
            await ShowConnectionAsync();

            menu.Show("Async Menu");
        }

        public async Task ShowConnectionAsync()
        {
            Console.WriteLine("Getting the async connections...");

            try
            {
                var devList = await connectMasterSvc.GetDeviceListAsync(gatewayID);
                var asyncConns = new List<DeviceInfo>();

                for (int i = 0; i < devList.Count; i++)
                {
                    if (devList[i].AutoReconnect)
                    {
                        asyncConns.Add(devList[i]);
                    }
                }

                Console.WriteLine();
                Console.WriteLine("***** Async Connections: {0}", asyncConns.Count);
                for (int i = 0; i < asyncConns.Count; i++)
                {
                    Console.WriteLine(asyncConns[i]);
                }

                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot get the async connections: {0}", e);
            }
        }

        public async Task AddConnectionAsync()
        {
            List<AsyncConnectInfo> asyncConns = new List<AsyncConnectInfo>();

            while (true)
            {
                Console.Write(">> Enter the device ID (Press just ENTER if no more device): ");
                string input = Console.ReadLine();

                if (input.Trim().Equals(""))
                {
                    break;
                }

                uint devID = 0;

                if (!UInt32.TryParse(input, out devID))
                {
                    Console.WriteLine("Invalid device ID: {0}", input);
                    return;
                }

                var connInfo = MasterMainMenu.GetConnectInfo();

                if (connInfo == null)
                {
                    return;
                }

                AsyncConnectInfo asyncInfo = new AsyncConnectInfo { DeviceID = devID, IPAddr = connInfo.IPAddr, Port = connInfo.Port, UseSSL = connInfo.UseSSL };
                asyncConns.Add(asyncInfo);
            }

            if (asyncConns.Count == 0)
            {
                Console.WriteLine("You have to enter at least one async connection");
                return;
            }

            try
            {
                Console.WriteLine("Adding async connections...");
                await connectMasterSvc.AddAsyncConnectionAsync(gatewayID, asyncConns.ToArray());
                await ShowConnectionAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot add the async connections: {0}", e);
            }
        }

        public async Task DeleteConnectionAsync()
        {
            uint[] deviceIDs = Menu.GetDeviceIDs();

            if (deviceIDs == null)
            {
                return;
            }

            try
            {
                Console.WriteLine("Deleting async connections...");
                await connectMasterSvc.DeleteAsyncConnectionAsync(gatewayID, deviceIDs);
                await ShowConnectionAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot delete the async connections: {0}", e);
            }
        }
    }
}


