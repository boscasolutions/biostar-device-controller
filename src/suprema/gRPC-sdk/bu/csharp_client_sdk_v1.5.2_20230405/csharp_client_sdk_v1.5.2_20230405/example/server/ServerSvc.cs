using System;
using System.Collections.Generic;
using Gsdk.Server;
using Gsdk.User;
using Grpc.Core;
using Google.Protobuf.Collections;

namespace example
{
  class ServerSvc
  {
    private Gsdk.Server.Server.ServerClient serverClient;

    public ServerSvc(Channel channel) {
      serverClient = new Gsdk.Server.Server.ServerClient(channel);
    }

    public IAsyncStreamReader<ServerRequest> Subscribe(int queueSize) {
      var request = new SubscribeRequest{ QueueSize = queueSize };
      var response = serverClient.Subscribe(request);

      return response.ResponseStream;
    }

    public void Unsubscribe() {
      var request = new UnsubscribeRequest();
      serverClient.Unsubscribe(request);
    }

    public void HandleVerify(ServerRequest serverReq, ServerErrorCode errCode, UserInfo userInfo) {
      var request = new HandleVerifyRequest{ DeviceID = serverReq.DeviceID, SeqNO = serverReq.SeqNO, ErrCode = errCode };
      if(userInfo != null) {
        request.User = userInfo;
      }

      serverClient.HandleVerify(request);
    }

    public void HandleIdentify(ServerRequest serverReq, ServerErrorCode errCode, UserInfo userInfo) {
      var request = new HandleIdentifyRequest{ DeviceID = serverReq.DeviceID, SeqNO = serverReq.SeqNO, ErrCode = errCode };
      if(userInfo != null) {
        request.User = userInfo;
      }

      serverClient.HandleIdentify(request);
    }
  }
}