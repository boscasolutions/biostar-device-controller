using Grpc.Core;
using Gsdk.Connect;
using Gsdk.Rtsp;
using System;
using System.Threading.Tasks;

namespace example
{
    class RtspTest
    {
        private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "192.168.8.98";
        private const int GATEWAY_PORT = 4000;

        private const string DEVICE_ADDR = "192.168.8.227";
        private const int DEVICE_PORT = 51211;
        private const bool USE_SSL = false;
        private GatewayClient gatewayClient;
        private ConnectSvc connectSvc;
        private RtspSvc rtspSvc;

        public RtspTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            rtspSvc = new RtspSvc(gatewayClient.GetChannel());
        }

        public static async Task Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            RtspTest rtspTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                rtspTest = new RtspTest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
                devID = await rtspTest.connectSvc.ConnectAsync(connectInfo);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot connect to the device: {0}", e);
                gatewayClient.Close();
                Environment.Exit(1);
            }

            RTSPConfig config = null;
            uint[] devIDs = { devID };

            try
            {
                config = rtspTest.rtspSvc.GetConfig(devID);

                Console.WriteLine("Rtsp Config: {0}" + Environment.NewLine, config);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Rtsp service is not supported by the device {0}: {1}", devID, e);
                
                await rtspTest.connectSvc.Disconnect(devIDs);
                
                gatewayClient.Close();
                Environment.Exit(1);
            }
        }
    }
}
