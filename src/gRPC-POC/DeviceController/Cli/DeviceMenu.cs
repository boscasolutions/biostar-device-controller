using Gsdk.Connect;

namespace example
{
    class DeviceMenu
    {
        private Menu _menu;
        private ConnectService _connectService;

        public DeviceMenu(ConnectService connectService)
        {
            this._connectService = connectService;

            MenuItem[] items = new MenuItem[7];
            items[0] = new MenuItem("1", "Set connection mode", SetConnectionModeAsync, false);
            items[1] = new MenuItem("2", "Enable SSL", EnableSSLAsync, false);
            items[2] = new MenuItem("3", "Disable SSL", DisableSSLAsync, false);
            items[3] = new MenuItem("4", "Disconnect devices", DisconnectAsync, false);
            items[4] = new MenuItem("5", "Disconnect all devices", DisconnectAllAsync, false);
            items[5] = new MenuItem("6", "Refresh the managed device list", ShowDeviceListAsync, false);
            items[6] = new MenuItem("q", "Return to Main Menu", null, true);

            _menu = new Menu(items);
        }

        public async Task ShowAsync()
        {
            if (await GetDeviceListAsync() == 0)
            {
                Console.WriteLine("No connected device. Please connect to some devices first.");
                return;
            }

            await _menu.ShowAsync("Device Menu");
        }

        public async Task<int> GetDeviceListAsync()
        {
            Console.WriteLine("Getting the devices managed by the gateway...");

            try
            {
                var devList = await _connectService.GetDeviceListAsync();

                Console.WriteLine();
                
                Console.WriteLine("***** Managed Devices: {0}", devList.Count);
                
                for (int i = 0; i < devList.Count; i++)
                {
                    Console.WriteLine(devList[i]);
                }

                Console.WriteLine();

                return devList.Count;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot get device list: {0}", e);
                return 0;
            }
        }

        public async Task ShowDeviceListAsync()
        {
            await GetDeviceListAsync();
        }

        public async Task SetConnectionModeAsync()
        {
            Console.WriteLine("Enter the device IDs to to change the connection mode");

            uint[] deviceIDs = Menu.GetDeviceIDs();

            if (deviceIDs == null)
            {
                return;
            }

            Console.Write(">> Select the connection mode (0: Gateway to Device(default), 1: Device to Gateway): ");

            string modeStr = Console.ReadLine();

            int mode;

            if (!Int32.TryParse(modeStr, out mode))
            {
                Console.Write("Invalid connection mode: {0}", modeStr);
            }

            if (mode != 0 && mode != 1)
            {
                Console.Write("Invalid connection mode: {0}", modeStr);
            }

            Console.WriteLine("Changing the connection mode...");

            try
            {
                await _connectService.SetConnectionModeAsync(deviceIDs, (ConnectionMode)mode);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot set connection mode: {0}", e);
            }
        }

        public async Task EnableSSLAsync()
        {
            Console.WriteLine("Enter the device IDs to enable");

            uint[] deviceIDs = Menu.GetDeviceIDs();

            if (deviceIDs == null)
            {
                return;
            }

            Console.WriteLine("Enabling SSL...");

            try
            {
                await _connectService.EnableSSLAsync(deviceIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot enable SSL: {0}", e);
            }
        }

        public async Task DisableSSLAsync()
        {
            Console.WriteLine("Enter the device IDs to disable");

            uint[] deviceIDs = Menu.GetDeviceIDs();

            if (deviceIDs == null)
            {
                return;
            }

            Console.WriteLine("Disabling SSL...");

            try
            {
                await _connectService.DisableSSLAsync(deviceIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot disable SSL: {0}", e);
            }
        }

        public async Task DisconnectAsync()
        {
            Console.WriteLine("Enter the device IDs to disconnect");

            uint[] deviceIDs = Menu.GetDeviceIDs();

            if (deviceIDs == null)
            {
                return;
            }

            Console.WriteLine("Disconnecting devices...");

            try
            {
                await _connectService.Disconnect(deviceIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot disconnect devices: {0}", e);
            }
        }

        public async Task DisconnectAllAsync()
        {
            Console.WriteLine("Disconnecting all devices...");

            try
            {
                await _connectService.DisconnectAllAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot disconnect all devices: {0}", e);
            }
        }
    }
}