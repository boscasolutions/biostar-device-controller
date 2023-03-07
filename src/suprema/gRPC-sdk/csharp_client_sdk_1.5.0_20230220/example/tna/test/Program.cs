using System;
using Gsdk.Tna;
using Gsdk.Connect;
using Gsdk.Event;
using Gsdk.Device;
using Grpc.Core;

namespace example
{
  class TNATest
  {
    private const string GATEWAY_CA_FILE = "../../../../cert/gateway/ca.crt";
    private const string GATEWAY_ADDR = "192.168.0.2";
    private const int GATEWAY_PORT = 4000;

    private const string DEVICE_ADDR = "192.168.0.110";
    private const int DEVICE_PORT = 51211;
    private const bool USE_SSL = false;        

    private const string CODE_MAP_FILE = "../../event/event_code.json";

    private GatewayClient gatewayClient;
    private ConnectSvc connectSvc;
    private TNASvc tnaSvc;
    private EventSvc eventSvc;
    private DeviceSvc deviceSvc;

    public TNATest(GatewayClient client) {
      gatewayClient = client;

      connectSvc = new ConnectSvc(gatewayClient.GetChannel());
      tnaSvc = new TNASvc(gatewayClient.GetChannel());
      eventSvc = new EventSvc(gatewayClient.GetChannel());
      deviceSvc = new DeviceSvc(gatewayClient.GetChannel());
    }

    public static void Main(string[] args)
    {
      GatewayClient gatewayClient = null;
      TNATest tnaTest = null;
      uint devID = 0;

      try {
        gatewayClient = new GatewayClient();
        gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

        tnaTest = new TNATest(gatewayClient);

        var connectInfo = new ConnectInfo{ IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
        devID = tnaTest.connectSvc.Connect(connectInfo);
      } catch (RpcException e) {
				Console.WriteLine("Cannot connect to the device: {0}", e);
        gatewayClient.Close();
        Environment.Exit(1);
			}

      uint[] devIDs = { devID };

      try {
        var capability = tnaTest.deviceSvc.GetCapability(devID);

        if (!capability.DisplaySupported) {
          Console.WriteLine("TNA service is not supported by the device {0}: {1}", devID, capability);
          tnaTest.connectSvc.Disconnect(devIDs);
          gatewayClient.Close();
          Environment.Exit(1);          
        }
      } catch (RpcException e) {
				Console.WriteLine("Cannot get the device info: {0}", e);
        tnaTest.connectSvc.Disconnect(devIDs);
        gatewayClient.Close();
        Environment.Exit(1);
      }

      try {
        LogTest logTest = new LogTest(tnaTest.tnaSvc, tnaTest.eventSvc);

        tnaTest.eventSvc.InitCodeMap(CODE_MAP_FILE);
        tnaTest.eventSvc.StartMonitoring(devID);
        tnaTest.eventSvc.SetCallback(logTest.EventCallback);

        var origConfig = new ConfigTest(tnaTest.tnaSvc).Test(devID);
        logTest.Test(devID);

        tnaTest.tnaSvc.SetConfig(devID, origConfig);

        tnaTest.eventSvc.StopMonitoring(devID);
      } catch (RpcException e) {
				Console.WriteLine("Cannot complete the tna test for device {0}: {1}", devID, e);
      } finally {
        tnaTest.connectSvc.Disconnect(devIDs);
        gatewayClient.Close();
      }
    }
  }
}
