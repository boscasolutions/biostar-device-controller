using Grpc.Core;
using Gsdk.Device;

namespace example
{
    public class DeviceSvc
    {
        private Device.DeviceClient deviceClient;

        public DeviceSvc(Channel channel)
        {
            deviceClient = new Device.DeviceClient(channel);
        }

        public FactoryInfo GetInfo(uint deviceID)
        {
            var request = new GetInfoRequest { DeviceID = deviceID };
            var response = deviceClient.GetInfo(request);

            return response.Info;
        }

        public async Task<DeviceCapability> GetCapabilityAsync(uint deviceID)
        {
            var request = new GetCapabilityRequest { DeviceID = deviceID };
            var response = await deviceClient.GetCapabilityAsync(request);

            return response.DeviceCapability;
        }

        public CapabilityInfo GetCapabilityInfo(uint deviceID)
        {
            var request = new GetCapabilityInfoRequest { DeviceID = deviceID };
            var response = deviceClient.GetCapabilityInfo(request);

            return response.CapInfo;
        }
    }
}
