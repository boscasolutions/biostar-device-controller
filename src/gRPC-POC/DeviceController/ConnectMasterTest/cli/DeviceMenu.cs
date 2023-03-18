using Gsdk.Connect;
using System;

namespace example
{
    class DeviceMenu
    {
        private Menu menu;
        private ConnectMasterSvc connectMasterSvc;
        private string gatewayID;

        public DeviceMenu(ConnectMasterSvc svc, string id)
        {
            connectMasterSvc = svc;
            gatewayID = id;

            MenuItem[] items = new MenuItem[7];
            items[0] = new MenuItem("1", "Set connection mode", SetConnectionMode, false);
            items[1] = new MenuItem("2", "Enable SSL", EnableSSL, false);
            items[2] = new MenuItem("3", "Disable SSL", DisableSSL, false);
            items[3] = new MenuItem("4", "Disconnect devices", Disconnect, false);
            items[4] = new MenuItem("5", "Disconnect all devices", DisconnectAll, false);
            items[5] = new MenuItem("6", "Refresh the managed device list", ShowDeviceList, false);
            items[6] = new MenuItem("q", "Return to Main Menu", null, true);

            menu = new Menu(items);
        }

        public void Show()
        {
            if (GetDeviceList() == 0)
            {
                Console.WriteLine("No connected device. Please connect to some devices first.");
                return;
            }

            menu.Show("Device Menu");
        }

        public int GetDeviceList()
        {
            Console.WriteLine("Getting the devices managed by the gateway...");

            try
            {
                var devList = connectMasterSvc.GetDeviceList(gatewayID);

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

        public void ShowDeviceList()
        {
            GetDeviceList();
        }

        public void SetConnectionMode()
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
                connectMasterSvc.SetConnectionMode(deviceIDs, (ConnectionMode)mode);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot set connection mode: {0}", e);
            }
        }



        public void EnableSSL()
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
                connectMasterSvc.EnableSSL(deviceIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot enable SSL: {0}", e);
            }
        }

        public void DisableSSL()
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
                connectMasterSvc.DisableSSL(deviceIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot disable SSL: {0}", e);
            }
        }

        public void Disconnect()
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
                connectMasterSvc.Disconnect(deviceIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot disconnect devices: {0}", e);
            }
        }

        public void DisconnectAll()
        {
            Console.WriteLine("Disconnecting all devices...");

            try
            {
                connectMasterSvc.DisconnectAll(gatewayID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot disconnect all devices: {0}", e);
            }
        }
    }
}


