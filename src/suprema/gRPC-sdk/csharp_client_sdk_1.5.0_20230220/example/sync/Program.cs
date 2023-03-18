using System;
using System.Threading;
using Grpc.Core;

namespace example
{
	class SyncTest
	{
		private const string GATEWAY_CA_FILE = "../../../cert/gateway/ca.crt";
		private const string GATEWAY_ADDR = "192.168.0.2";
		private const int GATEWAY_PORT = 4000;

    private const string SYNC_CONFIG_FILE = "./sync_config.json";
    private const string CODE_MAP_FILE = "../event/event_code.json";

		private GatewayClient gatewayClient;
		private ConnectSvc connectSvc;
		private UserSvc userSvc;
		private CardSvc cardSvc;
		private EventSvc eventSvc;
		private DeviceMgr deviceMgr;
		private EventMgr eventMgr;
		private UserMgr userMgr;

		public SyncTest(GatewayClient client, TestConfig config) {
			gatewayClient = client;

			connectSvc = new ConnectSvc(gatewayClient.GetChannel());
			userSvc = new UserSvc(gatewayClient.GetChannel());
			cardSvc = new CardSvc(gatewayClient.GetChannel());
			eventSvc = new EventSvc(gatewayClient.GetChannel());
      eventSvc.InitCodeMap(CODE_MAP_FILE);

			deviceMgr = new DeviceMgr(connectSvc, config);
			eventMgr = new EventMgr(eventSvc, config);
			userMgr = new UserMgr(userSvc, cardSvc, config, deviceMgr, eventMgr);
		}

		public static void Main(string[] args)
		{
      GatewayClient client = null;

			try {
        client = new GatewayClient();
        client.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT); 
      } catch (RpcException e) {
				Console.WriteLine("Cannot connect to the gateway: {0}", e);
        Environment.Exit(1);
			}        

			TestConfig config = new TestConfig();
			try {
				config.Read(SYNC_CONFIG_FILE);
			} catch (Exception e) {
				Console.WriteLine("Cannot read the configuration file {0}: {1}", SYNC_CONFIG_FILE, e);
				client.Close();
        Environment.Exit(1);				
			}

      SyncTest syncTest = new SyncTest(client, config);

      try {
				Console.WriteLine("Trying to connect to the devices..." + Environment.NewLine);

				syncTest.deviceMgr.HandleConnection(syncTest.eventMgr.HandleConnection);
				syncTest.deviceMgr.ConnectToDevices();
				syncTest.eventMgr.HandleEvent(syncTest.userMgr.SyncUser);

	      KeyInput.PressEnter(Environment.NewLine + ">>> Press ENTER to show the test menu" + Environment.NewLine + Environment.NewLine);				

				new TestMenu(syncTest.deviceMgr, syncTest.eventMgr, syncTest.userMgr, config).Show();
			} catch (RpcException e) {
				Console.WriteLine("gRPC Error: {0}", e);
			} finally {
				syncTest.deviceMgr.DeleteConnection();
				syncTest.eventMgr.StopHandleEvent();
				client.Close();
      }
		}
	}
}

