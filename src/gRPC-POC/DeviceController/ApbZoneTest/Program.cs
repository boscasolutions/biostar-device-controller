using Grpc.Core;
using Gsdk.Connect;
using System;
using System.Threading.Tasks;

namespace example
{
    class ApbZoneTest
    {
        private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "localhost";
        private const int GATEWAY_PORT = 4000;

        private const string DEVICE_ADDR = "192.168.0.120";
        private const int DEVICE_PORT = 51211;
        private const bool USE_SSL = false;

        private const string CODE_MAP_FILE = "../../event/event_code.json";

        private GatewayClient gatewayClient;
        private ConnectSvc connectSvc;
        private ApbZoneSvc apbSvc;
        private EventSvc eventSvc;
        private Rs485Svc rs485Svc;

        public ApbZoneTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            apbSvc = new ApbZoneSvc(gatewayClient.GetChannel());
            eventSvc = new EventSvc(gatewayClient.GetChannel());
            rs485Svc = new Rs485Svc(gatewayClient.GetChannel());
        }

        public static async Task Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            ApbZoneTest apbTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                apbTest = new ApbZoneTest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
                devID = await apbTest.connectSvc.ConnectAsync(connectInfo);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot connect to the device: {0}", e);
                gatewayClient.Close();
                Environment.Exit(1);
            }

            uint[] devIDs = { devID };
            RS485Test rs485Test = new RS485Test(apbTest.rs485Svc);

            try
            {
                if (!rs485Test.CheckSlaves(devID))
                {
                    return;
                }

                LogTest logTest = new LogTest(apbTest.eventSvc);

                apbTest.eventSvc.InitCodeMap(CODE_MAP_FILE);
                apbTest.eventSvc.StartMonitoringAsync(devID);
                apbTest.eventSvc.SetCallback(logTest.EventCallback);

                new APBTest(apbTest.apbSvc).Test(devID, rs485Test.GetSlaves());

                apbTest.eventSvc.StopMonitoringAsync(devID);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the apb test for device {0}: {1}", devID, e);
            }
            finally
            {
                rs485Test.RestoreSlaves(devID);
                await apbTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }
    }
}
