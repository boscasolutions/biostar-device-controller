using System;
using Gsdk.Display;
using Grpc.Core;

namespace example
{
	class DisplaySvc
	{
		private Display.DisplayClient displayClient;

		public DisplaySvc(Channel channel) {
			displayClient = new Display.DisplayClient(channel);
		}

		public DisplayConfig GetConfig(uint deviceID) {
			var request = new GetConfigRequest{ DeviceID = deviceID };
			var response = displayClient.GetConfig(request);

			return response.Config;
		}
	}
}
