using Gsdk.Wiegand;
using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System;

namespace example
{
  class WiegandSvc
  {
    private Wiegand.WiegandClient wiegandClient;

    public WiegandSvc(Channel channel) {
      wiegandClient = new Wiegand.WiegandClient(channel);
    }

    public WiegandConfig GetConfig(uint deviceID) {
      var request = new GetConfigRequest{ DeviceID = deviceID };
      var response = wiegandClient.GetConfig(request);

      return response.Config;
    }

    public void SetConfig(uint deviceID, WiegandConfig config) {
      var request = new SetConfigRequest{ DeviceID = deviceID, Config = config };
      wiegandClient.SetConfig(request);
    }

    public void PrintConfig(WiegandConfig config) {
      Console.WriteLine("Wiegand Config: {0}" + Environment.NewLine, config);

      for(int i = 0; i < config.Formats.Count; i++) {
        Console.WriteLine("Format {0}", i);
        PrintWiegandFormat(config.Formats[i]);
      }

      for(int i = 0; i < config.SlaveFormats.Count; i++) {
        Console.WriteLine("Slave Format {0}", i);
        PrintWiegandFormat(config.SlaveFormats[i]);
      }
      
      if(config.CSNFormat != null) {
        Console.WriteLine("CSN Format");
        PrintWiegandFormat(config.CSNFormat);
      }
    }

    public void PrintWiegandFormat(WiegandFormat format) {
      Console.WriteLine("Format ID: {0}, Length: {1}", format.FormatID, format.Length);
      for(int i = 0; i < format.IDFields.Count; i++) {
        Console.WriteLine("ID Field {0}: Bit Mask - {1}", i, string.Join(", ", format.IDFields[i].ToByteArray()));
      }
      for(int i = 0; i < format.ParityFields.Count; i++) {
        Console.WriteLine("Parity Field {0}: Pos - {1}, Type - {2}, Bit Mask - {3}", i, format.ParityFields[i].ParityPos, format.ParityFields[i].ParityType, string.Join(", ", format.ParityFields[i].Data.ToByteArray()));
      }
    }    
  }
}