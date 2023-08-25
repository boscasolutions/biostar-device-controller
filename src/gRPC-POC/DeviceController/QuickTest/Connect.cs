using Gsdk.Connect;
using System;
using System.Threading.Tasks;

namespace example
{
    class ConnectTest
    {
        private ConnectSvc connectSvc;

        public ConnectTest(ConnectSvc svc)
        {
            connectSvc = svc;
        }

        public async Task<uint> TestAsync(string deviceAddr, int port, bool useSSL)
        {
            var devList = await connectSvc.GetDeviceListAsync();

            Console.WriteLine("Device list before connection: {0}" + Environment.NewLine, devList);

            var connectInfo = new ConnectInfo { IPAddr = deviceAddr, Port = port, UseSSL = useSSL };
            var devID = await connectSvc.ConnectAsync(connectInfo);

            devList = await connectSvc.GetDeviceListAsync();

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

        public async Task<uint> TestAsync(string gatewayID, string deviceAddr, int port, bool useSSL)
        {
            var devList = await connectMasterSvc.GetDeviceListAsync(gatewayID);

            Console.WriteLine("Device list before connection: {0}" + Environment.NewLine, devList);

            var connectInfo = new ConnectInfo { IPAddr = deviceAddr, Port = port, UseSSL = useSSL };
            var devID = await connectMasterSvc.ConnectAsync(gatewayID, connectInfo);

            devList = await connectMasterSvc.GetDeviceListAsync(gatewayID);

            Console.WriteLine("Device list after connection: {0}" + Environment.NewLine, devList);

            return devID;
        }
    }
}