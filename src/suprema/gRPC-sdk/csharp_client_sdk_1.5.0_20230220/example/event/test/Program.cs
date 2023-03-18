using System;
using Grpc.Core;

namespace example
{
  class EventTest
  {
    private const string GATEWAY_CA_FILE = "../../../../cert/gateway/ca.crt";
    private const string GATEWAY_ADDR = "localhost";
    private const int GATEWAY_PORT = 4000;

    private const string DEVICE_ADDR = "192.168.1.74";
    private const int DEVICE_PORT = 51211;
    private const bool USE_SSL = true;        

    private GatewayClient gatewayClient;
    private ConnectSvc connectSvc;
    private EventSvc eventSvc;

    public EventTest(GatewayClient client) {
      gatewayClient = client;

      connectSvc = new ConnectSvc(gatewayClient.GetChannel());
      eventSvc = new EventSvc(gatewayClient.GetChannel());
    }

    public static void Main(string[] args)
    {
      GatewayClient gatewayClient = null;
      EventTest eventTest = null;
      uint devID = 0;

      try {
        gatewayClient = new GatewayClient();
        gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

        eventTest = new EventTest(gatewayClient);

        var connectInfo = new ConnectInfo{ IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
        devID = eventTest.connectSvc.Connect(connectInfo);
      } catch (RpcException e) {
				Console.WriteLine("Cannot connect to the device: {0}", e);
        gatewayClient.Close();
        Environment.Exit(1);
			}

      uint[] devIDs = { devID };

      try {
        LogTest logTest = new LogTest(eventTest.eventSvc);

        logTest.Test(devID);
      } catch (RpcException e) {
				Console.WriteLine("Cannot complete the event test for device {0}: {1}", devID, e);
      } finally {
        eventTest.connectSvc.Disconnect(devIDs);
        gatewayClient.Close();
      }
    }
  }
}
