using System;
using Gsdk.Connect;
using Gsdk.Device;
using Grpc.Core;
using Gsdk.Operator;

namespace example
{
  class DeviceOperatorTest
  {
    private const string GATEWAY_CA_FILE = "../../../../cert/gateway/ca.crt";
    private const string GATEWAY_ADDR = "192.168.8.54";
    private const int GATEWAY_PORT = 4000;

    private const string DEVICE_ADDR = "192.168.8.237";
    private const int DEVICE_PORT = 51211;
    private const bool USE_SSL = false;        

    private const string CODE_MAP_FILE = "../../event/event_code.json";

    private GatewayClient gatewayClient;
    private ConnectSvc connectSvc;
    private DeviceSvc deviceSvc;
    private OperatorSvc operatorSvc;

    public DeviceOperatorTest(GatewayClient client) {
      gatewayClient = client;

      connectSvc = new ConnectSvc(gatewayClient.GetChannel());
      deviceSvc = new DeviceSvc(gatewayClient.GetChannel());
      operatorSvc = new OperatorSvc(gatewayClient.GetChannel());
    }

    public static void Main(string[] args)
    {
      GatewayClient gatewayClient = null;
      DeviceOperatorTest deviceOperatorTest = null;
      uint devID = 0;

      try {
        gatewayClient = new GatewayClient();
        gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

        deviceOperatorTest = new DeviceOperatorTest(gatewayClient);

        var connectInfo = new ConnectInfo{ IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
        devID = deviceOperatorTest.connectSvc.Connect(connectInfo);
      } catch (RpcException e) {
				Console.WriteLine("Cannot connect to the device: {0}", e);
        gatewayClient.Close();
        Environment.Exit(1);
			}

      uint[] devIDs = { devID };

      try {
        OperatorTest operatorTest = new OperatorTest(deviceOperatorTest.operatorSvc);

        operatorTest.GetOperator(devID);
        operatorTest.AddOperator(devID);
        operatorTest.GetOperator(devID);

        operatorTest.RemoveOperator(devID);
        operatorTest.GetOperator(devID);
      } catch (RpcException e) {
				Console.WriteLine("Cannot complete the user test for device {0}: {1}", devID, e);
      } finally {
        deviceOperatorTest.connectSvc.Disconnect(devIDs);
        gatewayClient.Close();
      }
    }
  }
}
