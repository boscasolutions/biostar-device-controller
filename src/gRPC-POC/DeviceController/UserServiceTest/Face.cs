using Gsdk.Face;
using Gsdk.User;
using System;

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

            var faceData = faceSvc.Scan(deviceID, FaceEnrollThreshold.Bs2FaceEnrollThresholdDefault);

            var userFace = new UserFace { UserID = userID };
            userFace.Faces.Add(faceData);
            userSvc.SetFace(deviceID, new UserFace[] { userFace });

            KeyInput.PressEnter(">> Try to authenticate the enrolled face. And, press ENTER to end the test." + Environment.NewLine);
        }
    }
}

