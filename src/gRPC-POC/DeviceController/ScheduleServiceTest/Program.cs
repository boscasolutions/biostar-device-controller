using Grpc.Core;
using Gsdk.Connect;
using System;

namespace example
{
    class ScheduleTest
    {
        private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "localhost";
        private const int GATEWAY_PORT = 4000;

        private const string DEVICE_ADDR = "192.168.1.74";
        private const int DEVICE_PORT = 51211;
        private const bool USE_SSL = false;

        private GatewayClient gatewayClient;
        private ConnectSvc connectSvc;
        private ScheduleSvc scheduleSvc;

        public ScheduleTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            scheduleSvc = new ScheduleSvc(gatewayClient.GetChannel());
        }

        public static void Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            ScheduleTest scheduleTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                scheduleTest = new ScheduleTest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
                devID = scheduleTest.connectSvc.Connect(connectInfo);
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
                new SampleTest(scheduleTest.scheduleSvc).Test(devID);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the schedule test for device {0}: {1}", devID, e);
            }
            finally
            {
                scheduleTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }
    }
}
