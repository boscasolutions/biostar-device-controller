using System;
using System.IO;
using Grpc.Core;

namespace example
{
  class GrpcClient
  {
    protected Channel channel;

    public Channel GetChannel() {
      return channel;
    }

    public void Close() {
      channel.ShutdownAsync().Wait();
    }
  }

  class GatewayClient : GrpcClient {
    public void Connect(string caFile, string serverAddr, int serverPort) {
      var channelCredentials = new SslCredentials(File.ReadAllText(caFile));

      channel = new Channel(serverAddr, serverPort, channelCredentials);
    } 
  }
}