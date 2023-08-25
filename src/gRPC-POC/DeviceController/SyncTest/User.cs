using Google.Protobuf.Collections;
using Gsdk.Event;
using Gsdk.User;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace example
{
    class UserMgr
    {
        private UserSvc userSvc;
        private CardSvc cardSvc;
        private TestConfig testConfig;
        private DeviceMgr deviceMgr;
        private EventMgr eventMgr;

        private List<string> enrolledIDs;

        private const int BS2_EVENT_USER_ENROLL_SUCCESS = 0x2000;
        private const int BS2_EVENT_USER_UPDATE_SUCCESS = 0x2200;
        private const int BS2_EVENT_USER_DELETE_SUCCESS = 0x2400;
        private const int BS2_EVENT_USER_DELETE_ALL_SUCCESS = 0x2600;

        public UserMgr(UserSvc userSvc, CardSvc cardSvc, TestConfig config, DeviceMgr deviceMgr, EventMgr eventMgr)
        {
            this.userSvc = userSvc;
            this.cardSvc = cardSvc;
            this.testConfig = config;
            this.deviceMgr = deviceMgr;
            this.eventMgr = eventMgr;

            enrolledIDs = new List<string>();
        }

        public async Task EnrollUserAsync(string userID)
        {
            uint enrollDeviceID = testConfig.configData.enroll_device.device_id;

            Console.WriteLine(">>> Place a unregistered CSN card on the device {0}...", enrollDeviceID);

            var cardData = cardSvc.Scan(enrollDeviceID);

            if (cardData.CSNCardData == null)
            {
                Console.WriteLine("This test does not support a smart card");
                return;
            }

            var hdr = new UserHdr { ID = userID, NumOfCard = 1 };
            var userInfo = new UserInfo { Hdr = hdr };
            userInfo.Cards.Add(cardData.CSNCardData);

            UserInfo[] userInfos = { userInfo };
            await userSvc.EnrollAsync(enrollDeviceID, userInfos);
        }

        public async Task DeleteUserAsync(string userID)
        {
            string[] userIDs = { userID };
            await userSvc.DeleteAsync(testConfig.configData.enroll_device.device_id, userIDs);
        }

        public async Task<RepeatedField<UserInfo>> GetNewUserAsync(uint deviceID)
        {
            if (enrolledIDs.Count == 0)
            {
                Console.WriteLine("No new user");
                return null;
            }

            return await userSvc.GetUserAsync(deviceID, enrolledIDs.ToArray());
        }

        public async Task SyncUserAsync(EventLog eventLog)
        {
            eventMgr.PrintEvent(eventLog);

            // Handle only the events of the enrollment device
            if (eventLog.DeviceID != testConfig.configData.enroll_device.device_id)
            {
                return;
            }

            var connectedIDs = await deviceMgr.GetConnectedDevicesAsync(false);
            var targetDeviceIDs = testConfig.GetTargetDeviceIDs(connectedIDs);

            if (targetDeviceIDs.Length == 0)
            {
                Console.WriteLine("No device to sync");
                return;
            }

            if (eventLog.EventCode == BS2_EVENT_USER_ENROLL_SUCCESS || eventLog.EventCode == BS2_EVENT_USER_UPDATE_SUCCESS)
            {
                Console.WriteLine("Trying to synchronize the enrolled user {0}...", eventLog.UserID);

                string[] userIDs = { eventLog.UserID };
                UserInfo[] newUserInfos = { (await userSvc.GetUserAsync(eventLog.DeviceID, userIDs))[0] };

                await userSvc.EnrollMultiAsync(targetDeviceIDs, newUserInfos);

                if (!enrolledIDs.Contains(eventLog.UserID))
                {
                    enrolledIDs.Add(eventLog.UserID);
                }

                // Generate a MultiErrorResponse 
                // It should fail since the users are duplicated.
                /* try {
                  userSvc.EnrollMulti(targetDeviceIDs, newUserInfos);
                } catch (RpcException e) {
                  var multiErr = ErrSvc.GetMultiError(e);
                  Console.WriteLine("Multi errors: {0}", multiErr);
                } */
            }
            else if (eventLog.EventCode == BS2_EVENT_USER_DELETE_SUCCESS)
            {
                Console.WriteLine("Trying to synchronize the deleted user {0}...", eventLog.UserID);
                string[] userIDs = { eventLog.UserID };
                await userSvc.DeleteMultiAsync(targetDeviceIDs, userIDs);

                enrolledIDs.Remove(eventLog.UserID);
            }
        }
    }
}