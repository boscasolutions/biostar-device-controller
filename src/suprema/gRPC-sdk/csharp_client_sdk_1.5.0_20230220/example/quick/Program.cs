using System;
using System.Threading;
using Grpc.Core;

namespace example
{
	class QuickStart
	{
		private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "localhost";
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

		private const string DEVICE_ADDR = "192.168.1.100";
		private const int DEVICE_PORT = 51211;
		private const bool USE_SSL = false;

		private GrpcClient grpcClient;
		private ConnectSvc connectSvc;
		private ConnectMasterSvc connectMasterSvc;
		private DeviceSvc deviceSvc;
		private DisplaySvc displaySvc;
		private UserSvc userSvc;
		private FingerSvc fingerSvc;
		private FaceSvc faceSvc;
		private CardSvc cardSvc;
		private EventSvc eventSvc;

		public QuickStart(GrpcClient client) {
			grpcClient = client;

			connectSvc = new ConnectSvc(grpcClient.GetChannel());
			connectMasterSvc = new ConnectMasterSvc(grpcClient.GetChannel());
			deviceSvc = new DeviceSvc(grpcClient.GetChannel());
			displaySvc = new DisplaySvc(grpcClient.GetChannel());
			userSvc = new UserSvc(grpcClient.GetChannel());
			fingerSvc = new FingerSvc(grpcClient.GetChannel());
			faceSvc = new FaceSvc(grpcClient.GetChannel());
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
			uint devID = 0;

			try {
				quickStart = new QuickStart(client);

				if(masterMode) {
					devID = new ConnectMasterTest(quickStart.connectMasterSvc).Test(GATEWAY_ID, DEVICE_ADDR, DEVICE_PORT, USE_SSL);
				} else {
					Console.WriteLine("Do you want to connect in SSL? [y/n]");
					string input = Console.ReadLine().Trim();
					if (input.Equals(""))
					{
						return;
					}
					else if (input.Equals("y"))
					{
						Console.WriteLine("Do you want to try to enable SSL on the connected device? [y/n]");
						string input2 = Console.ReadLine().Trim();
						if (input2.Equals(""))
						{
							return;
						}
						else if (input2.Equals("y"))
						{
							devID = new ConnectTest(quickStart.connectSvc).Test(DEVICE_ADDR, DEVICE_PORT, false);
							uint[] devIDs = {devID};
							quickStart.connectSvc.EnableSSL(devIDs);

							if(masterMode) {
								quickStart.connectMasterSvc.Disconnect(devIDs);
							} else {
								quickStart.connectSvc.Disconnect(devIDs);
							}
							client.Close();
							return;
						}

						devID = new ConnectTest(quickStart.connectSvc).Test(DEVICE_ADDR, DEVICE_PORT, USE_SSL);
					}
					else
					{
						devID = new ConnectTest(quickStart.connectSvc).Test(DEVICE_ADDR, DEVICE_PORT, USE_SSL);
					}
				}
			} catch (RpcException e) {
				Console.WriteLine("Cannot connect to the device: {0}", e);
				client.Close();
				Environment.Exit(1);
			}

			try {
				var capabilityInfo = new DeviceTest(quickStart.deviceSvc).Test(devID);

				if (capabilityInfo.FaceSupported) {
					new FaceTest(quickStart.faceSvc).Test(devID);
				}

				if (capabilityInfo.FingerSupported) {
					new FingerTest(quickStart.fingerSvc).Test(devID);
				}

				if (capabilityInfo.CardSupported) {
					new CardTest(quickStart.cardSvc).Test(devID, capabilityInfo); 
				}

				new UserTest(quickStart.userSvc, quickStart.fingerSvc, quickStart.faceSvc).Test(devID, capabilityInfo);
				new EventTest(quickStart.eventSvc).Test(devID); 
			} catch (RpcException e) {
				Console.WriteLine("gRPC Error: {0}", e);
			} finally {
				uint[] deviceIDs = { devID };

				if(masterMode) {
					quickStart.connectMasterSvc.Disconnect(deviceIDs);
				} else {
					quickStart.connectSvc.Disconnect(deviceIDs);
				}
				
				client.Close();
			}
		}
	}
}

