using Grpc.Core;
using Gsdk.Connect;
using System;
using System.Threading;

namespace example
{
    class ConnectMasterTest
    {
        private const string MASTER_CA_FILE = "../../../../cert/master/ca.crt";
        private const string MASTER_ADDR = "192.168.0.2";
        private const int MASTER_PORT = 4010;
        private const string TENANT_CERT_FILE = "../../../../cert/master/tenant1.crt";
        private const string TENANT_KEY_FILE = "../../../../cert/master/tenant1_key.pem";
        private const string ADMIN_CERT_FILE = "../../../../cert/master/admin.crt";
        private const string ADMIN_KEY_FILE = "../../../../cert/master/admin_key.pem";

        private const string TENANT_ID = "tenant1";
        private const string GATEWAY_ID = "gateway1";

        private const int STATUS_QUEUE_SIZE = 16;

        private MasterClient masterClient;
        private ConnectMasterSvc connectMasterSvc;

        public ConnectMasterSvc GetConnectMasterSvc()
        {
            return connectMasterSvc;
        }

        public ConnectMasterTest(MasterClient client)
        {
            masterClient = client;

            connectMasterSvc = new ConnectMasterSvc(masterClient.GetChannel());
        }

        public CancellationTokenSource SubscribeDeviceStatus()
        {
            var devStatusStream = connectMasterSvc.Subscribe(STATUS_QUEUE_SIZE);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            ReceiveStatus(devStatusStream, cancellationTokenSource.Token);

            return cancellationTokenSource;
        }

        public static void Main(string[] args)
        {
            var masterClient = new MasterClient();
            masterClient.ConnectTenant(MASTER_CA_FILE, TENANT_CERT_FILE, TENANT_KEY_FILE, MASTER_ADDR, MASTER_PORT);

            var connectMasterTest = new ConnectMasterTest(masterClient);

            var tokenSource = connectMasterTest.SubscribeDeviceStatus();

            MasterMainMenu mainMenu = new MasterMainMenu(connectMasterTest.GetConnectMasterSvc(), GATEWAY_ID);
            mainMenu.Show();

            tokenSource.Cancel();
            masterClient.Close();
        }

        static async void ReceiveStatus(IAsyncStreamReader<StatusChange> stream, CancellationToken token)
        {
            Console.WriteLine("Start receiving device status");

            try
            {
                while (await stream.MoveNext(token))
                {
                    var statusChange = stream.Current;
                    if (statusChange.Status != Gsdk.Connect.Status.TlsNotAllowed && statusChange.Status != Gsdk.Connect.Status.TcpNotAllowed)
                    {
                        Console.WriteLine("\n\nStatus: {0}\n", statusChange);
                    }
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                {
                    Console.WriteLine("Monitoring is cancelled");
                }
                else
                {
                    Console.WriteLine("Monitoring error: {0}", e);
                }
            }
            finally
            {
                Console.WriteLine("Stop receiving device status");
            }
        }
    }

}
