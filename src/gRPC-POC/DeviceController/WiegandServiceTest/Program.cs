using Grpc.Core;
using Gsdk.Connect;
using System;

namespace example
{
    class WiegandTest
    {
        private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "localhost";
        private const int GATEWAY_PORT = 4000;

        private const string DEVICE_ADDR = "192.168.1.46";
        private const int DEVICE_PORT = 51211;
        private const bool USE_SSL = true;

        private GatewayClient gatewayClient;
        private ConnectSvc connectSvc;
        private WiegandSvc wiegandSvc;

        public WiegandTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            wiegandSvc = new WiegandSvc(gatewayClient.GetChannel());
        }

        public static void Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            WiegandTest wiegandTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                wiegandTest = new WiegandTest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
                devID = wiegandTest.connectSvc.Connect(connectInfo);
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
                new ConfigTest(wiegandTest.wiegandSvc).Test(devID);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the wiegand test for device {0}: {1}", devID, e);
            }
            finally
            {
                wiegandTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }
    }
}
