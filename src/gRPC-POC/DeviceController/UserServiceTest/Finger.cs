using Google.Protobuf;
using Gsdk.Finger;
using Gsdk.User;
using System;

namespace example
{
    class FingerTest
    {
        private FingerSvc fingerSvc;
        private UserSvc userSvc;

        private const uint QUALITY_THRESHOLD = 50;
        private const int NUM_OF_TEMPLATE = 2;

        public FingerTest(FingerSvc fingerSvc, UserSvc userSvc)
        {
            this.fingerSvc = fingerSvc;
            this.userSvc = userSvc;
        }

        public void Test(uint deviceID, string userID)
        {
            Console.WriteLine(Environment.NewLine + "===== Finger Test =====" + Environment.NewLine);

            var templateData = new ByteString[NUM_OF_TEMPLATE];

            for (int i = 0; i < NUM_OF_TEMPLATE; i++)
            {
                if (i == 0)
                {
                    Console.WriteLine(">> Scan a finger on the device...");
                }
                else
                {
                    Console.WriteLine(">> Scan the same finger again on the device...");
                }

                templateData[i] = fingerSvc.Scan(deviceID, TemplateFormat.Suprema, QUALITY_THRESHOLD);
            }

            var fingerData = new FingerData();
            fingerData.Templates.AddRange(templateData);

            var userFinger = new UserFinger { UserID = userID };
            userFinger.Fingers.Add(fingerData);

            userSvc.SetFingerAsync(deviceID, new UserFinger[] { userFinger });

            KeyInput.PressEnter(">> Try to authenticate the enrolled finger. And, press ENTER to end the test." + Environment.NewLine);
        }
    }
}

