using System;
using Gsdk.Status;
using Gsdk.Action;
using Google.Protobuf;

namespace example
{
	class ConfigTest
	{
		private StatusSvc statusSvc;

		public ConfigTest(StatusSvc svc) {
			statusSvc = svc;
		}

		public void Test(uint deviceID) {    
      // Backup the original configuration
      var origConfig = statusSvc.GetConfig(deviceID);
      Console.WriteLine(Environment.NewLine + "Original Config: {0}" + Environment.NewLine, origConfig);    

      var testConfig = origConfig.Clone();
      TestLED(deviceID, testConfig);
      TestBuzzer(deviceID, testConfig);

      // Restore the original configuration   
      statusSvc.SetConfig(deviceID, origConfig); 
    }

    void TestLED(uint deviceID, StatusConfig config) {
      Console.WriteLine(Environment.NewLine + "===== LED Status Test =====" + Environment.NewLine);    

      // Change the LED color of the normal status to yellow
      foreach(LEDStatus ledStatus in config.LEDState) {
        if(ledStatus.DeviceStatus == DeviceStatus.Normal) {
          ledStatus.Count = 0;
          ledStatus.Signals.Clear();
          ledStatus.Signals.Add(new LEDSignal{ Color = Gsdk.Device.LEDColor.Yellow, Duration = 2000, Delay = 0});
          break;
        }
      }

      statusSvc.SetConfig(deviceID, config);

      var newConfig = statusSvc.GetConfig(deviceID);
      Console.WriteLine(Environment.NewLine + "New Config: {0}" + Environment.NewLine, newConfig); 

      Console.WriteLine(">> The LED color of the normal status is changed to yellow.");
      KeyInput.PressEnter(">> Press ENTER for the next test." + Environment.NewLine);
    }

    void TestBuzzer(uint deviceID, StatusConfig config) {
      Console.WriteLine(Environment.NewLine + "===== Buzzer Status Test =====" + Environment.NewLine);    

      // Change the buzzer signal for FAIL
      foreach(BuzzerStatus buzzerStatus in config.BuzzerState) {
        if(buzzerStatus.DeviceStatus == DeviceStatus.Fail) { // 2 x 500ms beeps
          buzzerStatus.Count = 1;
          buzzerStatus.Signals.Clear();
          buzzerStatus.Signals.Add(new BuzzerSignal{ Tone = Gsdk.Device.BuzzerTone.High, Duration = 500, Delay = 2});
          buzzerStatus.Signals.Add(new BuzzerSignal{ Tone = Gsdk.Device.BuzzerTone.High, Duration = 500, Delay = 2});
          break;
        }
      }

      statusSvc.SetConfig(deviceID, config);

      var newConfig = statusSvc.GetConfig(deviceID);
      Console.WriteLine(Environment.NewLine + "New Config: {0}" + Environment.NewLine, newConfig); 

      Console.WriteLine(">> The buzzer for the FAIL status is changed to two 500ms beeps. Try to authenticate unregistered credentials for the test.");
      KeyInput.PressEnter(">> Press ENTER for the next test." + Environment.NewLine);
    }
	}
}

