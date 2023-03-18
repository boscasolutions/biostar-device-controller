using Gsdk.Voip;
using System;

namespace example
{
    class ConfigTest
    {
        private VoipSvc voipSvc;

        public ConfigTest(VoipSvc svc)
        {
            voipSvc = svc;
        }

        public void Test(uint deviceID, VOIPConfig config)
        {
            Console.WriteLine(Environment.NewLine + "===== Test for VOIPConfig =====" + Environment.NewLine);

            // Backup the original configuration
            VOIPConfig origConfig = config.Clone();

            config.ServerURL = "rtsp.server.com";
            config.ServerPort = 554;
            config.UserID = "RTSP User ID";
            config.UserPW = "2378129307";
            config.Enabled = true;

            voipSvc.SetConfig(deviceID, config);

            KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);

            // Restore the original configuration   
            voipSvc.SetConfig(deviceID, origConfig);
        }
    }
}

