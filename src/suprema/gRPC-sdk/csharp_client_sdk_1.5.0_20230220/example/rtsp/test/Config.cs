using System;
using Gsdk.Rtsp;

namespace example
{
	class ConfigTest
	{
		private RtspSvc rtspSvc;

		public ConfigTest(RtspSvc svc) {
			rtspSvc = svc;
		}

		public void Test(uint deviceID, RTSPConfig config) {    
			Console.WriteLine(Environment.NewLine + "===== Test for RTSPConfig =====" + Environment.NewLine);      

      // Backup the original configuration
      RTSPConfig origConfig = config.Clone();

      config.ServerURL = "rtsp.server.com";
      config.ServerPort = 554;
      config.UserID = "RTSP User ID";
      config.UserPW = "2378129307";
      config.Enabled = true;

      rtspSvc.SetConfig(deviceID, config);

      KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);      

      // Restore the original configuration   
      rtspSvc.SetConfig(deviceID, origConfig); 
    }
	}
}

