using Grpc.Core;
using Gsdk.Auth;

namespace example
{
    public class AuthSvc
    {
        private Auth.AuthClient authClient;

        public AuthSvc(Channel channel)
        {
            authClient = new Auth.AuthClient(channel);
        }

        public AuthConfig GetConfig(uint deviceID)
        {
            var request = new GetConfigRequest { DeviceID = deviceID };
            var response = authClient.GetConfig(request);

            return response.Config;
        }

        public void SetConfig(uint deviceID, AuthConfig config)
        {
            var request = new SetConfigRequest { DeviceID = deviceID, Config = config };
            var response = authClient.SetConfig(request);
        }
    }
}