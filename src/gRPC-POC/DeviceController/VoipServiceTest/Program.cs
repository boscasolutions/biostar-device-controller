using Grpc.Core;
using Gsdk.Connect;
using Gsdk.Voip;
using System;
using System.Threading.Tasks;

namespace example
{
    class VoipTest
    {
        private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "192.168.8.98";
        private const int GATEWAY_PORT = 4000;

        private const string DEVICE_ADDR = "192.168.8.227";
        private const int DEVICE_PORT = 51211;
        private const bool USE_SSL = false;
        private GatewayClient gatewayClient;
        private ConnectSvc connectSvc;
        private VoipSvc voipSvc;

        public VoipTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            voipSvc = new VoipSvc(gatewayClient.GetChannel());
        }

        public static async Task Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            VoipTest voipTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                voipTest = new VoipTest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };

                devID = await voipTest.connectSvc.ConnectAsync(connectInfo);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot connect to the device: {0}", e);
                gatewayClient.Close();
                Environment.Exit(1);
            }

            VOIPConfig config = null;
            uint[] devIDs = { devID };

            try
            {
                config = voipTest.voipSvc.GetConfig(devID);

                Console.WriteLine("Voip Config: {0}" + Environment.NewLine, config);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Voip service is not supported by the device {0}: {1}", devID, e);

                await voipTest.connectSvc.Disconnect(devIDs);
                
                gatewayClient.Close();
                Environment.Exit(1);
            }
        }
    }
}
