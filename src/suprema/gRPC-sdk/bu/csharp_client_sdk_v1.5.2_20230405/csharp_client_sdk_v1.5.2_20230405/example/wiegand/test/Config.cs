using System;
using Gsdk.Wiegand;
using Google.Protobuf;

namespace example
{
	class ConfigTest
	{
		private WiegandSvc wiegandSvc;

		public ConfigTest(WiegandSvc svc) {
			wiegandSvc = svc;
		}

		public void Test(uint deviceID) {    
      // Backup the original configuration
      WiegandConfig origConfig = wiegandSvc.GetConfig(deviceID);
      Console.WriteLine(Environment.NewLine + ">>> Original Wiegand Config" + Environment.NewLine);    
      wiegandSvc.PrintConfig(origConfig);

			Console.WriteLine(Environment.NewLine + "===== Wiegand Config Test" + Environment.NewLine);      

      Test26bit(deviceID);
      Test37bit(deviceID);

      // Restore the original configuration   
      wiegandSvc.SetConfig(deviceID, origConfig); 
    }

    // 26 bit standard
    // FC: 01 1111 1110 0000 0000 0000 0000 : 0x01fe0000
    // ID: 00 0000 0001 1111 1111 1111 1110 : 0x0001fffe
    // EP: 01 1111 1111 1110 0000 0000 0000 : 0x01ffe000, Pos 0, Type: Even
    // OP: 00 0000 0000 0001 1111 1111 1110 : 0x00001ffe, Pos 25, Type: Odd 

    void Test26bit(uint deviceID) {
      WiegandFormat default26bit = new WiegandFormat{
        Length = 26,
        IDFields = {
          ByteString.CopyFrom(new byte[]{0, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0}), // Facility Code
          ByteString.CopyFrom(new byte[]{0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 0}) // ID
        },
        ParityFields = {
          new ParityField{ ParityPos = 0, ParityType = WiegandParity.Even, Data = ByteString.CopyFrom(new byte[]{0, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0}) },
          new ParityField{ ParityPos = 25, ParityType = WiegandParity.Odd, Data = ByteString.CopyFrom(new byte[]{0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 0}) }
        }
      };

      WiegandConfig defaultConfig = new WiegandConfig{
        Mode = WiegandMode.WiegandInOnly,
        UseWiegandBypass = false,
        UseFailCode = false,

        OutPulseWidth = 40,
        OutPulseInterval = 10000,
        Formats = { default26bit },
        UseWiegandUserID = WiegandOutType.WiegandOutUserId
      };

      wiegandSvc.SetConfig(deviceID, defaultConfig); 

      Console.WriteLine(">>> Wiegand Config with Standard 26bit Format" + Environment.NewLine);    

      var newConfig = wiegandSvc.GetConfig(deviceID);
      wiegandSvc.PrintConfig(newConfig);
    }

    // 37 bit HID
    // FC: 0 1111 1111 1111 1111 0000 0000 0000 0000 0000 : 0x0ffff00000
    // ID: 0 0000 0000 0000 0000 1111 1111 1111 1111 1110 : 0x00000ffffe
    // EP: 0 1111 1111 1111 1111 1100 0000 0000 0000 0000 : 0x0ffffc0000, Pos 0, Type: Even
    // OP: 0 0000 0000 0000 0000 0111 1111 1111 1111 1110 : 0x000007fffe, Pos 36, Type: Odd    

    void Test37bit(uint deviceID) {
      WiegandFormat hid37bitFormat = new WiegandFormat{
        Length = 37,
        IDFields = {
          ByteString.CopyFrom(new byte[]{0, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0}), // Facility Code
          ByteString.CopyFrom(new byte[]{0, 0, 0, 0, 0, /**/ 0, 0 ,0 ,0, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 0}) // ID
        },
        ParityFields = {
          new ParityField{ ParityPos = 0, ParityType = WiegandParity.Even, Data = ByteString.CopyFrom(new byte[]{0, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0}) },
          new ParityField{ ParityPos = 36, ParityType = WiegandParity.Odd, Data = ByteString.CopyFrom(new byte[]{0, 0, 0, 0, 0, /**/ 0, 0 ,0 ,0, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 0, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 0}) }
        }
      };

      WiegandConfig hid37bitConfig = new WiegandConfig{
        Mode = WiegandMode.WiegandInOnly,
        UseWiegandBypass = false,
        UseFailCode = false,

        OutPulseWidth = 40,
        OutPulseInterval = 10000,
        Formats = { hid37bitFormat },
        UseWiegandUserID = WiegandOutType.WiegandOutUserId
      };

      wiegandSvc.SetConfig(deviceID, hid37bitConfig); 

      Console.WriteLine(Environment.NewLine + ">>> Wiegand Config with HID 37bit Format" + Environment.NewLine);    

      var newConfig = wiegandSvc.GetConfig(deviceID);
      wiegandSvc.PrintConfig(newConfig);
    }

	}
}

