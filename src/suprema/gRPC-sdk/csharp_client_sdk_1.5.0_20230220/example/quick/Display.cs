using System;
using System.IO;
using Gsdk.Display;

namespace example
{
	class DisplayTest
	{
		private DisplaySvc displaySvc;

		public DisplayTest(DisplaySvc svc) {
			displaySvc = svc;
		}

		public void Test(uint deviceID) {
			var config = displaySvc.GetConfig(deviceID);

			Console.WriteLine("Display config: {0}" + Environment.NewLine, config);
		}        
	}
}

