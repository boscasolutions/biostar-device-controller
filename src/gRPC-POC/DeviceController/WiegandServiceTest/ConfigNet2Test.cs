using Google.Protobuf;
using Gsdk.Wiegand;
using System;

namespace example
{
    class ConfigNet2Test
    {
        private WiegandSvc wiegandSvc;

        public ConfigNet2Test(WiegandSvc svc)
        {
            wiegandSvc = svc;
        }

        public void Test(uint deviceID)
        {
            // Backup the original configuration
            WiegandConfig origConfig = wiegandSvc.GetConfig(deviceID);
            Console.WriteLine(Environment.NewLine + ">>> Original Wiegand Config" + Environment.NewLine);
            wiegandSvc.PrintConfig(origConfig);

            Console.WriteLine(Environment.NewLine + "===== Wiegand NET2 Config Test" + Environment.NewLine);

            Test_Set_IO_To_Net2_Output(deviceID);

            // Restore the original configuration   
            // wiegandSvc.SetConfig(deviceID, origConfig);
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
                FormatID = 1,
                Length = 26,
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
                UseWiegandUserID = WiegandOutType.WiegandOutCardId,
                CSNFormat = net2CsnFormat,
                OutPulseWidth = 40,
                OutPulseInterval = 2000,
                Formats = { net226bit }
            };

            wiegandSvc.SetConfig(deviceID, defaultConfig);

            Console.WriteLine(">>> Wiegand Config with NET2 26bit Format" + Environment.NewLine);

            var newConfig = wiegandSvc.GetConfig(deviceID);
            wiegandSvc.PrintConfig(newConfig);
        }
    }
}