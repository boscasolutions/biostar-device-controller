using Gsdk.Status;
using Grpc.Core;

namespace example
{
  class StatusSvc
  {
    private Gsdk.Status.Status.StatusClient statusClient;

    public StatusSvc(Channel channel) {
      statusClient = new Gsdk.Status.Status.StatusClient(channel);
    }

    public StatusConfig GetConfig(uint deviceID) {
      var request = new GetConfigRequest{ DeviceID = deviceID };
      var response = statusClient.GetConfig(request);

      return response.Config;
    }

    public void SetConfig(uint deviceID, StatusConfig config) {
      var request = new SetConfigRequest{ DeviceID = deviceID, Config = config };
      statusClient.SetConfig(request);
    }
  }
}