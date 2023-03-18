using Grpc.Core;
using Gsdk.Connect;
using Gsdk.Device;
using System;

namespace example
{
    class StatusTest
    {
        private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "192.168.8.98";
        private const int GATEWAY_PORT = 4000;

        private const string DEVICE_ADDR = "192.168.8.205";
        private const int DEVICE_PORT = 51211;
        private const bool USE_SSL = false;

        private GatewayClient gatewayClient;
        private ConnectSvc connectSvc;
        private StatusSvc statusSvc;
        private DeviceSvc deviceSvc;

        public StatusTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            statusSvc = new StatusSvc(gatewayClient.GetChannel());
            deviceSvc = new DeviceSvc(gatewayClient.GetChannel());
        }

        public static void Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            StatusTest statusTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                statusTest = new StatusTest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
                devID = statusTest.connectSvc.Connect(connectInfo);
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
                DeviceCapability capability = statusTest.deviceSvc.GetCapability(devID);

                if (capability.DisplaySupported)
                {
                    Console.WriteLine("Status configuration is effective only for headless devices: {0}", capability.DisplaySupported);
                    statusTest.connectSvc.Disconnect(devIDs);
                    gatewayClient.Close();
                    Environment.Exit(1);
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot get the device info: {0}", e);
                statusTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
                Environment.Exit(1);
            }

            try
            {
                new ConfigTest(statusTest.statusSvc).Test(devID);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the status test for device {0}: {1}", devID, e);
            }
            finally
            {
                statusTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }

        static bool IsHeadless(CapabilityInfo capInfo)
        {
            switch (capInfo.Type)
            {
                case Gsdk.Device.Type.BioentryP2:
                case Gsdk.Device.Type.BioentryR2:
                case Gsdk.Device.Type.BioentryW2:
                case Gsdk.Device.Type.Xpass2:
                case Gsdk.Device.Type.Xpass2Keypad:
                case Gsdk.Device.Type.XpassD2:
                case Gsdk.Device.Type.XpassD2Keypad:
                case Gsdk.Device.Type.XpassS2:
                    return true;

                default:
                    return false;
            }
        }
    }
}
