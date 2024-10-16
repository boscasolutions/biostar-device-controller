using System;
using System.IO;

namespace example
{
    class FingerTest
    {
        private const string FINGERPRINT_IMAGE_FILE = "./finger.bmp";
        private const uint QUALITY_THRESHOLD = 50;

        private FingerSvc fingerSvc;

        public FingerTest(FingerSvc svc)
        {
            fingerSvc = svc;
        }

        public void Test(uint deviceID)
        {
            var fingerConfig = fingerSvc.GetConfig(deviceID);

            Console.WriteLine("Finger config: {0}" + Environment.NewLine, fingerConfig);

            Console.WriteLine(">>> Scan a finger...");

            var templateData = fingerSvc.Scan(deviceID, fingerConfig.TemplateFormat, QUALITY_THRESHOLD);

            Console.WriteLine("Finger template: {0}" + Environment.NewLine, BitConverter.ToString(templateData.ToByteArray()));

            var bmpImage = fingerSvc.GetImage(deviceID);
            File.WriteAllBytes(FINGERPRINT_IMAGE_FILE, bmpImage.ToByteArray());

        }
    }
}

