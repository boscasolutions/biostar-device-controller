using Gsdk.Connect;
using System;
using System.Collections.Generic;

namespace example
{
    class AcceptMenu
    {
        private Menu menu;
        private ConnectMasterSvc connectMasterSvc;
        private string gatewayID;

        public AcceptMenu(ConnectMasterSvc svc, string id)
        {
            connectMasterSvc = svc;
            gatewayID = id;

            MenuItem[] items = new MenuItem[6];
            items[0] = new MenuItem("1", "Add devices to the filter", AddDevices, false);
            items[1] = new MenuItem("2", "Delete devices from the filter", DeleteDevices, false);
            items[2] = new MenuItem("3", "Allow all devices", AllowAll, false);
            items[3] = new MenuItem("4", "Disallow all devices", DisallowAll, false);
            items[4] = new MenuItem("5", "Refresh the pending device list", ShowPendingList, false);
            items[5] = new MenuItem("q", "Return to Main Menu", null, true);

            menu = new Menu(items);
        }

        public void Show()
        {
            ShowAcceptFilter();
            ShowPendingList();

            menu.Show("Accept Menu");
        }

        public void AddDevices()
        {
            Console.WriteLine("Enter the device IDs to add");

            uint[] deviceIDs = Menu.GetDeviceIDs();

            if (deviceIDs == null)
            {
                return;
            }

            try
            {
                AcceptFilter filter = connectMasterSvc.GetAcceptFilter(gatewayID);

                for (int i = 0; i < deviceIDs.Length; i++)
                {
                    if (!filter.DeviceIDs.Contains(deviceIDs[i]))
                    {
                        filter.DeviceIDs.Add(deviceIDs[i]);
                    }
                }

                filter.AllowAll = false;

                connectMasterSvc.SetAcceptFilter(gatewayID, filter);
                ShowAcceptFilter();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot add the devices to the filter: {0}", e);
            }
        }

        public void DeleteDevices()
        {
            Console.WriteLine("Enter the device IDs to delete");

            uint[] deviceIDs = Menu.GetDeviceIDs();

            if (deviceIDs == null)
            {
                return;
            }

            try
            {
                AcceptFilter filter = connectMasterSvc.GetAcceptFilter(gatewayID);

                for (int i = 0; i < deviceIDs.Length; i++)
                {
                    filter.DeviceIDs.Remove(deviceIDs[i]);
                }

                filter.AllowAll = false;

                connectMasterSvc.SetAcceptFilter(gatewayID, filter);
                ShowAcceptFilter();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot delete the devices from the filter: {0}", e);
            }
        }

        public void AllowAll()
        {
            AcceptFilter filter = new AcceptFilter { AllowAll = true };

            try
            {
                connectMasterSvc.SetAcceptFilter(gatewayID, filter);
                ShowAcceptFilter();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot allow all devices: {0}", e);
            }
        }

        public void DisallowAll()
        {
            AcceptFilter filter = new AcceptFilter { AllowAll = false };

            try
            {
                connectMasterSvc.SetAcceptFilter(gatewayID, filter);
                ShowAcceptFilter();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot disallow all devices: {0}", e);
            }
        }

        public void ShowAcceptFilter()
        {
            Console.WriteLine("Getting the accept filter...");

            try
            {
                var filter = connectMasterSvc.GetAcceptFilter(gatewayID);

                Console.WriteLine();
                Console.WriteLine("***** Accept Filter: {0}", filter);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot get the accept filter: {0}", e);
            }
        }

        public void ShowPendingList()
        {
            Console.WriteLine("Getting the pending device list...");

            try
            {
                var devList = connectMasterSvc.GetPendingList(gatewayID);

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


