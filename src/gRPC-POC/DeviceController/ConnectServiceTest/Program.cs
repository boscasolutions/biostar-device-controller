using Grpc.Core;
using Gsdk.Connect;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace example
{
    class ConnectTest
    {
        private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "localhost";
        private const int GATEWAY_PORT = 4000;

        private const int STATUS_QUEUE_SIZE = 16;

        private GatewayClient gatewayClient;
        private ConnectSvc connectSvc;

        public ConnectSvc GetConnectSvc()
        {
            return connectSvc;
        }

        public ConnectTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
        }

        public async Task<CancellationTokenSource> SubscribeDeviceStatusAsync()
        {
            var devStatusStream = await connectSvc.SubscribeAsync(STATUS_QUEUE_SIZE);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            ReceiveStatus(devStatusStream, cancellationTokenSource.Token);

            return cancellationTokenSource;
        }

        public static async Task Main(string[] args)
        {
            var gatewayClient = new GatewayClient();
            gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

            var connectTest = new ConnectTest(gatewayClient);

            var tokenSource = await connectTest.SubscribeDeviceStatusAsync();

            MainMenu mainMenu = new MainMenu(connectTest.GetConnectSvc());
            mainMenu.Show();

            tokenSource.Cancel();
            gatewayClient.Close();
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
