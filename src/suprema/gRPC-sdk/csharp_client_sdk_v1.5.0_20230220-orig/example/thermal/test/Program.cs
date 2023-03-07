using System;
using Gsdk.Thermal;
using Gsdk.Connect;
using Gsdk.Event;
using Grpc.Core;

namespace example
{
  class ThermalTest
  {
    private const string GATEWAY_CA_FILE = "../../../../cert/gateway/ca.crt";
    private const string GATEWAY_ADDR = "192.168.8.98";
    private const int GATEWAY_PORT = 4000;

    private const string DEVICE_ADDR = "192.168.8.205";
    private const int DEVICE_PORT = 51211;
    private const bool USE_SSL = false;        

    private const string CODE_MAP_FILE = "../../event/event_code.json";

    private GatewayClient gatewayClient;
    private ConnectSvc connectSvc;
    private ThermalSvc thermalSvc;
    private EventSvc eventSvc;

    public ThermalTest(GatewayClient client) {
      gatewayClient = client;

      connectSvc = new ConnectSvc(gatewayClient.GetChannel());
      thermalSvc = new ThermalSvc(gatewayClient.GetChannel());
      eventSvc = new EventSvc(gatewayClient.GetChannel());
    }

    public static void Main(string[] args)
    {
      GatewayClient gatewayClient = null;
      ThermalTest thermalTest = null;
      uint devID = 0;

      try {
        gatewayClient = new GatewayClient();
        gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

        thermalTest = new ThermalTest(gatewayClient);

        var connectInfo = new ConnectInfo{ IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
        devID = thermalTest.connectSvc.Connect(connectInfo);
      } catch (RpcException e) {
				Console.WriteLine("Cannot connect to the device: {0}", e);
        gatewayClient.Close();
        Environment.Exit(1);
			}

      ThermalConfig config = null;
      uint[] devIDs = { devID };

      try {
        config = thermalTest.thermalSvc.GetConfig(devID);

        Console.WriteLine("Thermal Config: {0}" + Environment.NewLine, config);
      } catch (RpcException e) {
				Console.WriteLine("Thermal service is not supported by the device {0}: {1}", devID, e);
        thermalTest.connectSvc.Disconnect(devIDs);
        gatewayClient.Close();
        Environment.Exit(1);
      }

      try {
        LogTest logTest = new LogTest(thermalTest.thermalSvc, thermalTest.eventSvc);

        thermalTest.eventSvc.InitCodeMap(CODE_MAP_FILE);
        thermalTest.eventSvc.StartMonitoring(devID);
        thermalTest.eventSvc.SetCallback(logTest.EventCallback);

        new ConfigTest(thermalTest.thermalSvc).Test(devID, config);
        logTest.Test(devID);

        thermalTest.eventSvc.StopMonitoring(devID);
      } catch (RpcException e) {
				Console.WriteLine("Cannot complete the thermal test for device {0}: {1}", devID, e);
      } finally {
        thermalTest.connectSvc.Disconnect(devIDs);
        gatewayClient.Close();
      }
    }
  }
}
