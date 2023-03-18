using Grpc.Core;
using Gsdk.Connect;
using System;

namespace example
{
    class ActionTest
    {
        private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "localhost";
        private const int GATEWAY_PORT = 4000;

        private const string DEVICE_ADDR = "192.168.1.74";
        private const int DEVICE_PORT = 51211;
        private const bool USE_SSL = false;

        private GatewayClient gatewayClient;
        private ConnectSvc connectSvc;
        private ActionSvc actionSvc;

        public ActionTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            actionSvc = new ActionSvc(gatewayClient.GetChannel());
        }

        public static void Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            ActionTest actionTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                actionTest = new ActionTest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
                devID = actionTest.connectSvc.Connect(connectInfo);
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
                new ConfigTest(actionTest.actionSvc).Test(devID);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the action test for device {0}: {1}", devID, e);
            }
            finally
            {
                actionTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }
    }
}
