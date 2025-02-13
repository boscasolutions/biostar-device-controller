using Grpc.Core;
using Gsdk.Connect;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace example
{
    class ConnectTest
    {
        private const int STATUS_QUEUE_SIZE = 16;

        private GatewayClient _gatewayClient;
        private ConnectService _connectService;

        public ConnectService GetConnectService()
        {
            return _connectService;
        }

        public ConnectTest(GatewayClient client, IConfiguration configuration)
        {
            _gatewayClient = client;

            _connectService = new ConnectService(_gatewayClient.GetChannel());
        }

        public static  async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var gatewayAddr = configuration.GetValue<string>("GatewaySettings:Address") ?? "localhost";

            var gatewayPort = configuration.GetValue<int>("GatewaySettings:Port", 4000);
            
            var gatewayCaFile = configuration.GetValue<string>("GatewaySettings:CaFile") ?? "/cert/ca.crt";
            
            var gatewayClient = new GatewayClient();
            
            gatewayClient.Connect(gatewayCaFile, gatewayAddr, gatewayPort);

            var connectTest = new ConnectTest(gatewayClient, configuration);

            var tokenSource = await connectTest.SubscribeDeviceStatusAsync();

            MainMenu mainMenu = new MainMenu(connectTest.GetConnectService());
            
            await mainMenu.ShowAsync();

            tokenSource.Cancel();

            gatewayClient.Close();
        }

        public async Task<CancellationTokenSource> SubscribeDeviceStatusAsync()
        {
            var devStatusStream = await _connectService.SubscribeAsync(STATUS_QUEUE_SIZE);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            ReceiveStatus(devStatusStream, cancellationTokenSource.Token);

            return cancellationTokenSource;
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