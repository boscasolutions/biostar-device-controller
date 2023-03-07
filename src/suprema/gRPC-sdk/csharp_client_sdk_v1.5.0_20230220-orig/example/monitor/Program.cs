using System;
using System.Threading;
using Grpc.Core;

namespace example
{
	class QuickStart
	{
		private const string GATEWAY_CA_FILE = "../../../cert/gateway/ca.crt";
		private const string GATEWAY_ADDR = "192.168.8.54";
		private const int GATEWAY_PORT = 4000;

		private const string MASTER_CA_FILE = "../../../cert/master/ca.crt";
		private const string MASTER_ADDR = "192.168.8.54";
		private const int MASTER_PORT = 4010;
		private const string TENANT_CERT_FILE = "../../../cert/master/tenant1.crt";
		private const string TENANT_KEY_FILE = "../../../cert/master/tenant1_key.pem";
		private const string ADMIN_CERT_FILE = "../../../cert/master/admin.crt";
		private const string ADMIN_KEY_FILE = "../../../cert/master/admin_key.pem";

		private const string TENANT_ID = "tenant1";
		private const string GATEWAY_ID = "gateway1";        

		private const string DEVICE_ADDR_1 = "192.168.8.221";
		private const string DEVICE_ADDR_2 = "192.168.8.233";
		private const int DEVICE_PORT = 51211;
		private const bool USE_SSL = true;

		private GrpcClient grpcClient;
		private ConnectSvc connectSvc;
		private ConnectMasterSvc connectMasterSvc;
		private DeviceSvc deviceSvc;
		private UserSvc userSvc;
		private FingerSvc fingerSvc;
		private CardSvc cardSvc;
		private EventSvc eventSvc;

		public QuickStart(GrpcClient client) {
			grpcClient = client;

			connectSvc = new ConnectSvc(grpcClient.GetChannel());
			connectMasterSvc = new ConnectMasterSvc(grpcClient.GetChannel());
			deviceSvc = new DeviceSvc(grpcClient.GetChannel());
			userSvc = new UserSvc(grpcClient.GetChannel());
			fingerSvc = new FingerSvc(grpcClient.GetChannel());
			cardSvc = new CardSvc(grpcClient.GetChannel());
			eventSvc = new EventSvc(grpcClient.GetChannel());
		}

		public static void Main(string[] args)
		{
			bool masterMode = false;

			for(int i = 0; i < args.Length; i++) {
				if(args[i].Equals("-g")) { // enable gRPC debugging
					Environment.SetEnvironmentVariable("GRPC_TRACE", "api");
					Environment.SetEnvironmentVariable("GRPC_VERBOSITY", "debug");
					Grpc.Core.GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());
				}

				if(args[i].Equals("-mi")) {
					var masterClient = new MasterClient();
					masterClient.ConnectAdmin(MASTER_CA_FILE, ADMIN_CERT_FILE, ADMIN_KEY_FILE, MASTER_ADDR, MASTER_PORT);
					masterClient.InitTenant(TENANT_ID, GATEWAY_ID);

					return;
				}

				if(args[i].Equals("-m")) {
					masterMode = true;
					break;
				}
			}

			GrpcClient client = null;

			try {
				if(masterMode) {
					var masterClient = new MasterClient();
					masterClient.ConnectTenant(MASTER_CA_FILE, TENANT_CERT_FILE, TENANT_KEY_FILE, MASTER_ADDR, MASTER_PORT);
					client = masterClient; 
				} else {
					var gatewayClient = new GatewayClient();
					gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT); 
					client = gatewayClient; 
				}
			} catch (RpcException e) {
				Console.WriteLine("Cannot connect to the gateway: {0}", e);
				Environment.Exit(1);
			}

			QuickStart quickStart = null;
			uint devID1 = 0;
			uint devID2 = 0;

			try {
				quickStart = new QuickStart(client);

				if(masterMode) {
					devID1 = new ConnectMasterTest(quickStart.connectMasterSvc).Test(GATEWAY_ID, DEVICE_ADDR_1, DEVICE_PORT, false);
					devID2 = new ConnectMasterTest(quickStart.connectMasterSvc).Test(GATEWAY_ID, DEVICE_ADDR_2, DEVICE_PORT, false);
				} else {
					devID1 = new ConnectTest(quickStart.connectSvc).Test(DEVICE_ADDR_1, DEVICE_PORT, false);
					devID2 = new ConnectTest(quickStart.connectSvc).Test(DEVICE_ADDR_2, DEVICE_PORT, false);
				}
			} catch (RpcException e) {
				Console.WriteLine("Cannot connect to the device: {0}", e);
				client.Close();
				Environment.Exit(1);
			}

			uint[] devIDs = {devID1, devID2};

			try {
				var capInfo1 = new DeviceTest(quickStart.deviceSvc).Test(devID1);
				var capInfo2 = new DeviceTest(quickStart.deviceSvc).Test(devID2);

				if (capInfo1.FingerSupported) {
					new FingerTest(quickStart.fingerSvc).Test(devID1);
				}
				if (capInfo2.FingerSupported) {
					new FingerTest(quickStart.fingerSvc).Test(devID2);
				}
				
				new UserTest(quickStart.userSvc, quickStart.fingerSvc).Test(devIDs);

				new EventTest(quickStart.eventSvc).Test(devIDs);
			} catch (RpcException e) {
				Console.WriteLine("gRPC Error: {0}", e);
			} finally {
				if (masterMode)
				{
					quickStart.connectMasterSvc.Disconnect(devIDs);
				}
				else
				{
					quickStart.connectSvc.Disconnect(devIDs);
				}
				client.Close();
			}
		}
	}
}

