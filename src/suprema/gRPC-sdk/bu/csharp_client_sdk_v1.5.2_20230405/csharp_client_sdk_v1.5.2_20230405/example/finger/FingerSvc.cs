using Gsdk.Finger;
using Grpc.Core;
using Google.Protobuf;
using System;

namespace example
{
  class FingerSvc
  {
    private Finger.FingerClient fingerClient;

    public FingerSvc(Channel channel) {
      fingerClient = new Finger.FingerClient(channel);
    }

    public ByteString Scan(uint deviceID, TemplateFormat templateFormat, uint qualityThreshold) {
      var request = new ScanRequest{ DeviceID = deviceID, TemplateFormat = templateFormat, QualityThreshold = qualityThreshold };
      var response = fingerClient.Scan(request);

      Console.WriteLine("Template Score: {0}", response.QualityScore);

      return response.TemplateData;
    }

    public ByteString GetImage(uint deviceID) {
      var request = new GetImageRequest{ DeviceID = deviceID };
      var response = fingerClient.GetImage(request);

      return response.BMPImage;
    }

    public FingerConfig GetConfig(uint deviceID) {
      var request = new GetConfigRequest{ DeviceID = deviceID };
      var response = fingerClient.GetConfig(request);

      return response.Config;
    }
  }
}