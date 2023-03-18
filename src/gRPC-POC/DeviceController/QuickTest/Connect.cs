using Gsdk.Connect;
using System;

namespace example
{
    class ConnectTest
    {
        private ConnectSvc connectSvc;

        public ConnectTest(ConnectSvc svc)
        {
            connectSvc = svc;
        }

        public uint Test(string deviceAddr, int port, bool useSSL)
        {
            var devList = connectSvc.GetDeviceList();

            Console.WriteLine("Device list before connection: {0}" + Environment.NewLine, devList);

            var connectInfo = new ConnectInfo { IPAddr = deviceAddr, Port = port, UseSSL = useSSL };
            var devID = connectSvc.Connect(connectInfo);

            devList = connectSvc.GetDeviceList();

            Console.WriteLine("Device list after connection: {0}" + Environment.NewLine, devList);

            return devID;
        }
    }

    class ConnectMasterTest
    {
        private ConnectMasterSvc connectMasterSvc;

        public ConnectMasterTest(ConnectMasterSvc svc)
        {
            connectMasterSvc = svc;
        }

        public uint Test(string gatewayID, string deviceAddr, int port, bool useSSL)
        {
            var devList = connectMasterSvc.GetDeviceList(gatewayID);

            Console.WriteLine("Device list before connection: {0}" + Environment.NewLine, devList);

            var connectInfo = new ConnectInfo { IPAddr = deviceAddr, Port = port, UseSSL = useSSL };
            var devID = connectMasterSvc.Connect(gatewayID, connectInfo);

            devList = connectMasterSvc.GetDeviceList(gatewayID);

            Console.WriteLine("Device list after connection: {0}" + Environment.NewLine, devList);

            return devID;
        }
    }
}

