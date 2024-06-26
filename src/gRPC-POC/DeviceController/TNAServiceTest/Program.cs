using Grpc.Core;
using Gsdk.Connect;
using System;
using System.Threading.Tasks;

namespace example
{
    class TNATest
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
        private TNASvc tnaSvc;
        private EventSvc eventSvc;
        private DeviceSvc deviceSvc;

        public TNATest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            tnaSvc = new TNASvc(gatewayClient.GetChannel());
            eventSvc = new EventSvc(gatewayClient.GetChannel());
            deviceSvc = new DeviceSvc(gatewayClient.GetChannel());
        }

        public static async Task Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            TNATest tnaTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                tnaTest = new TNATest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
                devID = await tnaTest.connectSvc.ConnectAsync(connectInfo);
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
                var capability = await tnaTest.deviceSvc.GetCapabilityAsync(devID);

                if (!capability.DisplaySupported)
                {
                    Console.WriteLine("TNA service is not supported by the device {0}: {1}", devID, capability);
                    await tnaTest.connectSvc.Disconnect(devIDs);
                    gatewayClient.Close();
                    Environment.Exit(1);
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot get the device info: {0}", e);
                await tnaTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
                Environment.Exit(1);
            }

            try
            {
                LogTest logTest = new LogTest(tnaTest.tnaSvc, tnaTest.eventSvc);

                tnaTest.eventSvc.InitCodeMap(CODE_MAP_FILE);
                await tnaTest.eventSvc.StartMonitoringAsync(devID);
                await tnaTest.eventSvc.SetCallback(logTest.EventCallback);

                var origConfig = new ConfigTest(tnaTest.tnaSvc).Test(devID);
                logTest.Test(devID);

                tnaTest.tnaSvc.SetConfig(devID, origConfig);

                await tnaTest.eventSvc.StopMonitoringAsync(devID);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the tna test for device {0}: {1}", devID, e);
            }
            finally
            {
                await tnaTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }
    }
}
