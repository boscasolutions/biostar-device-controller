using Gsdk.Device;
using Grpc.Core;

namespace example
{
    class DeviceSvc
	{
		private Device.DeviceClient deviceClient;

		public DeviceSvc(Channel channel) {
			deviceClient = new Device.DeviceClient(channel);
		}

		public FactoryInfo GetInfo(uint deviceID) {
			var request = new GetInfoRequest{ DeviceID = deviceID };
			var response = deviceClient.GetInfo(request);

			return response.Info;
		}

		public DeviceCapability GetCapability(uint deviceID) {
			var request = new GetCapabilityRequest{ DeviceID = deviceID };
			var response = deviceClient.GetCapability(request);

			return response.DeviceCapability;
		}

		public CapabilityInfo GetCapabilityInfo(uint deviceID) {
			var request = new GetCapabilityInfoRequest{ DeviceID = deviceID };
			var response = deviceClient.GetCapabilityInfo(request);

			return response.CapInfo;
		}
	}
}
