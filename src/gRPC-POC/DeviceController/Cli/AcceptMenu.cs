using Gsdk.Connect;

namespace example
{
    class AcceptMenu
    {
        private Menu menu;
        private ConnectSvc connectSvc;

        public AcceptMenu(ConnectSvc svc)
        {
            connectSvc = svc;

            MenuItem[] items = new MenuItem[6];
            items[0] = new MenuItem("1", "Add devices to the filter", AddDevicesAsync, false);
            items[1] = new MenuItem("2", "Delete devices from the filter", DeleteDevicesAsync, false);
            items[2] = new MenuItem("3", "Allow all devices", AllowAllAsync, false);
            items[3] = new MenuItem("4", "Disallow all devices", DisallowAllAsync, false);
            items[4] = new MenuItem("5", "Refresh the pending device list", ShowPendingListAsync, false);
            items[5] = new MenuItem("q", "Return to Main Menu", null, true);

            menu = new Menu(items);
        }

        public async Task ShowAsync()
        {
            await ShowAcceptFilterAsync();
            await ShowPendingListAsync();

            menu.Show("Accept Menu");
        }

        public async Task AddDevicesAsync()
        {
            Console.WriteLine("Enter the device IDs to add");

            uint[] deviceIDs = Menu.GetDeviceIDs();

            if (deviceIDs == null)
            {
                return;
            }

            try
            {
                AcceptFilter filter = await connectSvc.GetAcceptFilterAsync();

                for (int i = 0; i < deviceIDs.Length; i++)
                {
                    if (!filter.DeviceIDs.Contains(deviceIDs[i]))
                    {
                        filter.DeviceIDs.Add(deviceIDs[i]);
                    }
                }

                filter.AllowAll = false;

                await connectSvc.SetAcceptFilterAsync(filter);
                await ShowAcceptFilterAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot add the devices to the filter: {0}", e);
            }
        }

        public async Task DeleteDevicesAsync()
        {
            Console.WriteLine("Enter the device IDs to delete");

            uint[] deviceIDs = Menu.GetDeviceIDs();

            if (deviceIDs == null)
            {
                return;
            }

            try
            {
                AcceptFilter filter = await connectSvc.GetAcceptFilterAsync();

                for (int i = 0; i < deviceIDs.Length; i++)
                {
                    filter.DeviceIDs.Remove(deviceIDs[i]);
                }

                filter.AllowAll = false;

                await connectSvc.SetAcceptFilterAsync(filter);
                await ShowAcceptFilterAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot delete the devices from the filter: {0}", e);
            }
        }

        public async Task AllowAllAsync()
        {
            AcceptFilter filter = new AcceptFilter { AllowAll = true };

            try
            {
                await connectSvc.SetAcceptFilterAsync(filter);
                await ShowAcceptFilterAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot allow all devices: {0}", e);
            }
        }

        public async Task DisallowAllAsync()
        {
            AcceptFilter filter = new AcceptFilter { AllowAll = false };

            try
            {
                await connectSvc.SetAcceptFilterAsync(filter);
                await ShowAcceptFilterAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot disallow all devices: {0}", e);
            }
        }

        public async Task ShowAcceptFilterAsync()
        {
            Console.WriteLine("Getting the accept filter...");

            try
            {
                var filter = await connectSvc.GetAcceptFilterAsync();

                Console.WriteLine();
                Console.WriteLine("***** Accept Filter: {0}", filter);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot get the accept filter: {0}", e);
            }
        }

        public async Task ShowPendingListAsync()
        {
            Console.WriteLine("Getting the pending device list...");

            try
            {
                var devList = await connectSvc.GetPendingListAsync();

                Console.WriteLine();
                Console.WriteLine("***** Pending Devices: {0}", devList.Count);
                for (int i = 0; i < devList.Count; i++)
                {
                    Console.WriteLine(devList[i]);
                }

                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot get the pending list: {0}", e);
            }
        }
    }
}


