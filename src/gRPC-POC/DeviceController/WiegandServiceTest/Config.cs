using Google.Protobuf;
using Gsdk.Wiegand;
using System;

namespace example
{
    class ConfigTest
    {
        private WiegandSvc wiegandSvc;

        public ConfigTest(WiegandSvc svc)
        {
            wiegandSvc = svc;
        }

        public void Test(uint deviceID)
        {
            // Backup the original configuration
            WiegandConfig origConfig = wiegandSvc.GetConfig(deviceID);
            Console.WriteLine(Environment.NewLine + ">>> Original Wiegand Config" + Environment.NewLine);
            wiegandSvc.PrintConfig(origConfig);

            Console.WriteLine(Environment.NewLine + "===== Wiegand Config Test" + Environment.NewLine);

            // Test26bit(deviceID);
            // Test37bit(deviceID);
            Test_Set_IO_To_Net2_Output(deviceID);

            // Restore the original configuration   
            // wiegandSvc.SetConfig(deviceID, origConfig);
        }

        // 26 bit standard
        // FC: 01 1111 1110 0000 0000 0000 0000 : 0x01fe0000
        // ID: 00 0000 0001 1111 1111 1111 1110 : 0x0001fffe
        // EP: 01 1111 1111 1110 0000 0000 0000 : 0x01ffe000, Pos 0, Type: Even
        // OP: 00 0000 0000 0001 1111 1111 1110 : 0x00001ffe, Pos 25, Type: Odd 

        void Test26bit(uint deviceID)
        {
            WiegandFormat default26bit = new WiegandFormat
            {
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

            WiegandConfig defaultConfig = new WiegandConfig
            {
                Mode = WiegandMode.WiegandInOnly,
                UseWiegandBypass = false,
                UseFailCode = false,

                OutPulseWidth = 40,
                OutPulseInterval = 10000,
                Formats = { default26bit }
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

        void Test37bit(uint deviceID)
        {
            WiegandFormat hid37bitFormat = new WiegandFormat
            {
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

            WiegandConfig hid37bitConfig = new WiegandConfig
            {
                Mode = WiegandMode.WiegandInOnly,
                UseWiegandBypass = false,
                UseFailCode = false,

                OutPulseWidth = 40,
                OutPulseInterval = 10000,
                Formats = { hid37bitFormat }
            };

            wiegandSvc.SetConfig(deviceID, hid37bitConfig);

            Console.WriteLine(Environment.NewLine + ">>> Wiegand Config with HID 37bit Format" + Environment.NewLine);

            var newConfig = wiegandSvc.GetConfig(deviceID);
            wiegandSvc.PrintConfig(newConfig);
        }
        
        void Test_Set_IO_To_Net2_Output(uint deviceID)
        {
            /*
>>> Original Wiegand Config

Wiegand Config: { "mode": "WIEGAND_OUT_ONLY", "outPulseWidth": 40, "outPulseInterval": 2000, 
            "formats": [ 
            { "length": 26, 
                "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], 
                "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] 
            } ], 
            "CSNFormat": 
            { "formatID": 1, "length": 26, 
                "IDFields": [ "AAEBAQEBAQEBAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAQEBAQEBAQEBAQEBAQEBAQA=" ], 
                "parityFields": [ { "parityType": "WIEGAND_PARITY_EVEN", "data": "AAEBAQEBAQEBAQEBAQAAAAAAAAAAAAAAAAA=" }, { "parityPos": 25, "parityType": "WIEGAND_PARITY_ODD", "data": "AAAAAAAAAAAAAAAAAAEBAQEBAQEBAQEBAQA=" } ] 
            }, "useWiegandUserID": "WIEGAND_OUT_CARD_ID" }

Format 0
Format ID: 0, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0

Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0

CSN Format
Format ID: 1, Length: 26
ID Field 0: Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
ID Field 1: Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0

Parity Field 0: Pos - 0, Type - Even, Bit Mask - 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
Parity Field 1: Pos - 25, Type - Odd, Bit Mask - 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0

===== Wiegand Config Test
            */
            WiegandFormat net226bit = new WiegandFormat
            {
                Length = 26,
                IDFields = {
                    ByteString.CopyFrom(new byte[]
                    {0, 1, /**/1, 1, 1, 1, 1, 1, 1, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0}), // Facility Code
                    ByteString.CopyFrom(new byte[]
                    {0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 0}) // ID
                    },
                ParityFields = {
                    new ParityField{ ParityPos = 0, ParityType = WiegandParity.Even, Data = ByteString.CopyFrom(new byte[]
                    {0, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0}) },
                    new ParityField{ ParityPos = 25, ParityType = WiegandParity.Odd, Data = ByteString.CopyFrom(new byte[]
                    {0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 0}) }
                }
            };

            WiegandFormat net2CsnFormat = new WiegandFormat
            {
                Length = 26,
                FormatID = 1,
                IDFields = {
                    ByteString.CopyFrom(new byte[]
                    {0, 1, /**/1, 1, 1, 1, 1, 1, 1, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0}), // 
                    ByteString.CopyFrom(new byte[]
                    {0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 0}) // 
                    },
                ParityFields = {
                    new ParityField{ ParityPos = 0, ParityType = WiegandParity.Even, Data = ByteString.CopyFrom(new byte[]
                    {0, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0}) },
                    new ParityField{ ParityPos = 25, ParityType = WiegandParity.Odd, Data = ByteString.CopyFrom(new byte[]
                    {0, 0, /**/ 0, 0, 0, 0, 0, 0, 0, 0, /**/ 0, 0, 0, 1, 1, 1, 1, 1, /**/ 1, 1, 1, 1, 1, 1, 1, 0}) }
                }
            };

            WiegandConfig defaultConfig = new WiegandConfig
            {
                Mode = WiegandMode.WiegandOutOnly,
                UseWiegandBypass = false,
                UseFailCode = false,
                UseWiegandUserID = WiegandOutType.WiegandOutUserId,

                OutPulseWidth = 40,
                OutPulseInterval = 2000,
                Formats = { net226bit, net2CsnFormat }
            };

            wiegandSvc.SetConfig(deviceID, defaultConfig);

            Console.WriteLine(">>> Wiegand Config with NET2 26bit Format" + Environment.NewLine);

            var newConfig = wiegandSvc.GetConfig(deviceID);
            wiegandSvc.PrintConfig(newConfig);
        }
    }
}