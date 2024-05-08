using Grpc.Core;

namespace example
{
    public class GrpcClient
    {
        protected Channel? _channel;

        public Channel GetChannel()
        {
            return _channel;
        }

        public void Close()
        {
            _channel.ShutdownAsync().Wait();
        }
    }

    public class GatewayClient : GrpcClient
    {
        public void Connect(string caFile, string serverAddr, int serverPort)
        {
            var channelCredentials = new SslCredentials(File.ReadAllText(AppContext.BaseDirectory + caFile));

            _channel = new Channel(serverAddr, serverPort, channelCredentials);
        }
    }
}