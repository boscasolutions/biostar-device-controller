// See https://aka.ms/new-console-template for more information
using example;
using Grpc.Core;
using Gsdk.Connect;

// Console.WriteLine("Hello, World!");



// TODO: Get the divece list
// Subscibe to device events
// show the events in a console in real time

class ConnectToGrpc
{
    private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
    private const string GATEWAY_ADDR = "localhost";
    private const int GATEWAY_PORT = 4000;
    private const int STATUS_QUEUE_SIZE = 16;

    private GatewayClient gatewayClient;
    private ConnectSvc connectSvc;

    public static void Main(string[] args)
    {
        // connect to the gateway
        var gatewayClient = new GatewayClient();
        
        gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

        // connect to gRpc
        var connectToGrpc = new ConnectToGrpc(gatewayClient);

        // get the gRpc status
        var tokenSource = connectToGrpc.SubscribeDeviceStatus();

        // TODO: add more functional stuff here to mimic a process




        MainMenu mainMenu = new MainMenu(connectToGrpc.GetConnectSvc());
        mainMenu.Show();

        tokenSource.Cancel();
        gatewayClient.Close();
    }

    public ConnectSvc GetConnectSvc()
    {
        return connectSvc;
    }

    public ConnectToGrpc(GatewayClient client)
    {
        gatewayClient = client;

        connectSvc = new ConnectSvc(gatewayClient.GetChannel());
    }

    public CancellationTokenSource SubscribeDeviceStatus()
    {
        var devStatusStream = connectSvc.Subscribe(STATUS_QUEUE_SIZE);

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