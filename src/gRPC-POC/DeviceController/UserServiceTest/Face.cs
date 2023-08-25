using Gsdk.Face;
using Gsdk.User;
using System;
using System.IO;

namespace example
{
    class FaceTest
    {
        private FaceSvc faceSvc;
        private UserSvc userSvc;

        public FaceTest(FaceSvc faceSvc, UserSvc userSvc)
        {
            this.faceSvc = faceSvc;
            this.userSvc = userSvc;
        }

        public void Test(uint deviceID, string userID)
        {
            Console.WriteLine(Environment.NewLine + "===== Face Test =====" + Environment.NewLine);

            Console.WriteLine(">> Enroll a unregistered face on the device...");

            FaceData faceData = faceSvc.Scan(deviceID, FaceEnrollThreshold.Bs2FaceEnrollThresholdDefault);

            var userFace = new UserFace { UserID = userID };
            userFace.Faces.Add(faceData);
            userSvc.SetFace(deviceID, new UserFace[] { userFace });

            // save the files for the test:
            string FACE_DATA_PATH = @"c:\facedata\";
            using (var ms = new MemoryStream(faceData.ImageData.ToByteArray()))
            {
                using (var fs = new FileStream($"{FACE_DATA_PATH}ImageData.jpg", FileMode.Create))
                {
                    ms.WriteTo(fs);
                }
            }

            using (var ms = new MemoryStream(faceData.IrImageData.ToByteArray()))
            {
                using (var fs = new FileStream($"{FACE_DATA_PATH}IrImageData.jpg", FileMode.Create))
                {
                    ms.WriteTo(fs);
                }
            }
            
            int ti = 0;
            foreach (var template in faceData.Templates)
            {
                ti++;
                using (var ms = new MemoryStream(template.ToByteArray()))
                {
                    using (var fs = new FileStream($"{FACE_DATA_PATH}templateData{ti}.teplate", FileMode.Create))
                    {
                        ms.WriteTo(fs);
                    }
                }
            }

            ti = 0;
            foreach (var irTemplate in faceData.IrTemplates)
            {
                ti++;
                using (var ms = new MemoryStream(irTemplate.ToByteArray()))
                {
                    using (var fs = new FileStream($"{FACE_DATA_PATH}irTemplateData{ti}.irteplate", FileMode.Create))
                    {
                        ms.WriteTo(fs);
                    }
                }
            }

            KeyInput.PressEnter(">> Try to authenticate the enrolled face. And, press ENTER to end the test." + Environment.NewLine);
        }
    }
}