using Gsdk.Action;
using Grpc.Core;

namespace example
{
  class ActionSvc
  {
    private Gsdk.Action.TriggerAction.TriggerActionClient actionClient;

    public ActionSvc(Channel channel) {
      actionClient = new Gsdk.Action.TriggerAction.TriggerActionClient(channel);
    }

    public TriggerActionConfig GetConfig(uint deviceID) {
      var request = new GetConfigRequest{ DeviceID = deviceID };
      var response = actionClient.GetConfig(request);

      return response.Config;
    }

    public void SetConfig(uint deviceID, TriggerActionConfig config) {
      var request = new SetConfigRequest{ DeviceID = deviceID, Config = config };
      actionClient.SetConfig(request);
    }
  }
}