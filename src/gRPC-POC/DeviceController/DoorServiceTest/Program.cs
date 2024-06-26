using Grpc.Core;
using Gsdk.Connect;
using System;
using System.Threading.Tasks;

namespace example
{
    class DoorTest
    {
        private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "localhost";
        private const int GATEWAY_PORT = 4000;

        private const string DEVICE_ADDR = "192.168.1.74";
        private const int DEVICE_PORT = 51211;
        private const bool USE_SSL = false;

        private const string CODE_MAP_FILE = "../../event/event_code.json";

        private GatewayClient gatewayClient;
        private ConnectSvc connectSvc;
        private DoorSvc doorSvc;
        private EventSvc eventSvc;
        private AccessSvc accessSvc;
        private UserSvc userSvc;

        public DoorTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            doorSvc = new DoorSvc(gatewayClient.GetChannel());
            eventSvc = new EventSvc(gatewayClient.GetChannel());
            accessSvc = new AccessSvc(gatewayClient.GetChannel());
            userSvc = new UserSvc(gatewayClient.GetChannel());
        }

        public static async Task Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            DoorTest doorTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                doorTest = new DoorTest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
                devID = await doorTest.connectSvc.ConnectAsync(connectInfo);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot connect to the device: {0}", e);
                gatewayClient.Close();
                Environment.Exit(1);
            }

            uint[] devIDs = { devID };

            try
            {
                LogTest logTest = new LogTest(doorTest.eventSvc);

                doorTest.eventSvc.InitCodeMap(CODE_MAP_FILE);
                doorTest.eventSvc.StartMonitoringAsync(devID);
                doorTest.eventSvc.SetCallback(logTest.EventCallback);

                new AccessTest(doorTest.doorSvc, doorTest.accessSvc, doorTest.userSvc, logTest).TestAsync(devID);

                doorTest.eventSvc.StopMonitoringAsync(devID);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the door test for device {0}: {1}", devID, e);
            }
            finally
            {
                doorTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }
    }
}
