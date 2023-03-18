using Gsdk.Rtsp;
using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System;

namespace example
{
  class RtspSvc
  {
    private RTSP.RTSPClient rtspClient;

    public RtspSvc(Channel channel) {
      rtspClient = new RTSP.RTSPClient(channel);
    }

    public RTSPConfig GetConfig(uint deviceID) {
      var request = new GetConfigRequest{ DeviceID = deviceID };
      var response = rtspClient.GetConfig(request);

      return response.Config;
    }

    public void SetConfig(uint deviceID, RTSPConfig config) {
      var request = new SetConfigRequest{ DeviceID = deviceID, Config = config };
      rtspClient.SetConfig(request);
    }
  }
}