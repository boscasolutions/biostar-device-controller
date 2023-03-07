using Gsdk.Voip;
using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System;

namespace example
{
  class VoipSvc
  {
    private VOIP.VOIPClient voipClient;

    public VoipSvc(Channel channel) {
      voipClient = new VOIP.VOIPClient(channel);
    }

    public VOIPConfig GetConfig(uint deviceID) {
      var request = new GetConfigRequest{ DeviceID = deviceID };
      var response = voipClient.GetConfig(request);

      return response.Config;
    }

    public void SetConfig(uint deviceID, VOIPConfig config) {
      var request = new SetConfigRequest{ DeviceID = deviceID, Config = config };
      voipClient.SetConfig(request);
    }
  }
}