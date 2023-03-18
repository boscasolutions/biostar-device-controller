using Grpc.Core;
using Gsdk.Connect;
using System;

namespace example
{
    class ServerTest
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
        private ServerSvc serverSvc;
        private EventSvc eventSvc;
        private AuthSvc authSvc;

        public ServerTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            serverSvc = new ServerSvc(gatewayClient.GetChannel());
            eventSvc = new EventSvc(gatewayClient.GetChannel());
            authSvc = new AuthSvc(gatewayClient.GetChannel());
        }

        public static void Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            ServerTest serverTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                serverTest = new ServerTest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
                devID = serverTest.connectSvc.Connect(connectInfo);
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
                LogTest logTest = new LogTest(serverTest.eventSvc);

                serverTest.eventSvc.InitCodeMap(CODE_MAP_FILE);
                serverTest.eventSvc.StartMonitoring(devID);
                serverTest.eventSvc.SetCallback(logTest.EventCallback);

                new MatchingTest(serverTest.serverSvc, serverTest.authSvc).Test(devID);

                serverTest.eventSvc.StopMonitoring(devID);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the server test for device {0}: {1}", devID, e);
            }
            finally
            {
                serverTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }
    }
}
