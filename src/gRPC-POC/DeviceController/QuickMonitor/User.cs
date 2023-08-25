using Gsdk.Finger;
using Gsdk.User;

namespace example
{
    class UserTest
    {
        private const int NUM_OF_NEW_USER = 1;
        private const uint QUALITY_THRESHOLD = 50;
        private UserSvc userSvc;
        private FingerSvc fingerSvc;

        public UserTest(UserSvc uSvc, FingerSvc fSvc)
        {
            userSvc = uSvc;
            fingerSvc = fSvc;
        }

        public async Task TestAsync(uint[] deviceIDs)
        {
            foreach (uint deviceID in deviceIDs)
            {

                var newUsers = new UserInfo[NUM_OF_NEW_USER];
                var newUserIDs = new String[NUM_OF_NEW_USER];
                var userID = 10;

                var hdr = new UserHdr { ID = String.Format("{0}", userID) };
                newUsers[0] = new UserInfo { Hdr = hdr };
                newUserIDs[0] = hdr.ID;

                await userSvc.DeleteAsync(deviceID, newUserIDs);

                await userSvc.EnrollAsync(deviceID, newUsers);
                await TestFingerAsync(deviceID, newUserIDs[0]);
            }
        }

        private async Task TestFingerAsync(uint deviceID, String userID)
        {
            string[] userIDs = new string[1];
            userIDs[0] = userID;

            var users = await userSvc.GetUserAsync(deviceID, userIDs);
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

            await userSvc.SetFingerAsync(deviceID, userFingers);
        }
    }
}

