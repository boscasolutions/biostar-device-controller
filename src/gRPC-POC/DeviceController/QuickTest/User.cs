using Gsdk.Device;
using Gsdk.Face;
using Gsdk.Finger;
using Gsdk.User;
using System;

namespace example
{
    class UserTest
    {
        private const int NUM_OF_NEW_USER = 3;
        private const uint QUALITY_THRESHOLD = 50;
        private UserSvc userSvc;
        private FingerSvc fingerSvc;
        private FaceSvc faceSvc;

        public UserTest(UserSvc uSvc, FingerSvc fSvc, FaceSvc faceSvc)
        {
            userSvc = uSvc;
            fingerSvc = fSvc;
            this.faceSvc = faceSvc;
        }

        public void Test(uint deviceID, CapabilityInfo capabilityInfo)
        {
            var userList = userSvc.GetList(deviceID);

            Console.WriteLine("User list: {0}" + Environment.NewLine, userList);

            string[] userIDs = new string[userList.Count];

            for (int i = 0; i < userList.Count; i++)
            {
                userIDs[i] = userList[i].ID;
            }

            var users = userSvc.GetUser(deviceID, userIDs);

            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine("User: {0} {1} {2}" + Environment.NewLine, users[i].Name, users[i].Hdr, users[i].Setting);
            }

            var newUsers = new UserInfo[NUM_OF_NEW_USER];
            var newUserIDs = new String[NUM_OF_NEW_USER];
            var rnd = new Random();

            for (int i = 0; i < NUM_OF_NEW_USER; i++)
            {
                var hdr = new UserHdr { ID = String.Format("{0}", rnd.Next()) };
                newUsers[i] = new UserInfo { Hdr = hdr };
                newUserIDs[i] = hdr.ID;
            }

            userSvc.Enroll(deviceID, newUsers);

            userList = userSvc.GetList(deviceID);
            Console.WriteLine("User list after enrolling new users: {0}" + Environment.NewLine, userList);

            if (capabilityInfo.FingerSupported)
            {
                TestFinger(deviceID, newUserIDs[0]);
            }

            if (capabilityInfo.FaceSupported)
            {
                TestFace(deviceID, newUserIDs[0]);
            }

            userSvc.Delete(deviceID, newUserIDs);

            userList = userSvc.GetList(deviceID);
            Console.WriteLine("User list after deleting new users: {0}" + Environment.NewLine, userList);
        }

        private void TestFinger(uint deviceID, String userID)
        {
            string[] userIDs = new string[1];
            userIDs[0] = userID;

            var users = userSvc.GetUser(deviceID, userIDs);
            Console.WriteLine("User without fingerprint: {0}" + Environment.NewLine, users[0]);

            var userFingers = new UserFinger[1];
            userFingers[0] = new UserFinger { UserID = userID };

            Console.WriteLine(">>> Scan a finger for {0}", userID);
            var firstTemplate = fingerSvc.Scan(deviceID, TemplateFormat.Suprema, QUALITY_THRESHOLD);

            Console.WriteLine(">>> Scan the same finger for {0}", userID);
            var secondTemplate = fingerSvc.Scan(deviceID, TemplateFormat.Suprema, QUALITY_THRESHOLD);

            var fingerData = new FingerData { Index = 0, Flag = 0 };
            fingerData.Templates.Add(firstTemplate);
            fingerData.Templates.Add(secondTemplate);

            userFingers[0].Fingers.Add(fingerData);

            userSvc.SetFinger(deviceID, userFingers);

            users = userSvc.GetUser(deviceID, userIDs);
            Console.WriteLine("User after adding fingerprints: {0}" + Environment.NewLine, users[0]);
        }

        private void TestFace(uint deviceID, String userID)
        {
            string[] userIDs = new string[1];
            userIDs[0] = userID;

            var users = userSvc.GetUser(deviceID, userIDs);
            Console.WriteLine("User without face: {0}" + Environment.NewLine, users[0]);

            var userFaces = new UserFace[1];
            userFaces[0] = new UserFace { UserID = userID };

            Console.WriteLine(">>> Scan a face for {0}", userID);
            var faceData = faceSvc.Scan(deviceID, FaceEnrollThreshold.Bs2FaceEnrollThresholdDefault);

            userFaces[0].Faces.Add(faceData);

            userSvc.SetFace(deviceID, userFaces);

            users = userSvc.GetUser(deviceID, userIDs);
            Console.WriteLine("User after adding faces: {0}" + Environment.NewLine, users[0]);
        }
    }
}

