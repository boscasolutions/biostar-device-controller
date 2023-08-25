using Gsdk.Access;
using Gsdk.Device;
using Gsdk.Door;
using Gsdk.User;
using System;
using System.Threading.Tasks;

namespace example
{
    class AccessTest
    {
        private const int TEST_DOOR_ID = 1;
        private const int TEST_ACCESS_LEVEL_ID = 1;
        private const int TEST_ACCESS_GROUP_ID = 1;
        private const int ALWAYS_SCHEDULE_ID = 1; // ID 1 is reserved for 'always'    

        private DoorSvc doorSvc;
        private AccessSvc accessSvc;
        private UserSvc userSvc;
        private LogTest logTest;

        public AccessTest(DoorSvc doorSvc, AccessSvc accessSvc, UserSvc userSvc, LogTest logTest)
        {
            this.doorSvc = doorSvc;
            this.accessSvc = accessSvc;
            this.userSvc = userSvc;
            this.logTest = logTest;
        }

        public async Task TestAsync(uint deviceID)
        {
            // Backup the original doors
            var origDoors = doorSvc.GetList(deviceID);
            Console.WriteLine(Environment.NewLine + "Original Doors: {0}" + Environment.NewLine, origDoors);

            TestDoor(deviceID);
            await TestAccessGroupAsync(deviceID);

            // Restore the original doors
            doorSvc.DeleteAll(deviceID);
            if (origDoors.Count > 0)
            {
                var doorArray = new DoorInfo[origDoors.Count];
                origDoors.CopyTo(doorArray, 0);
                doorSvc.Add(deviceID, doorArray);
            }
        }

        void TestDoor(uint deviceID)
        {
            var door = new DoorInfo
            {
                DoorID = TEST_DOOR_ID,
                Name = "Test Door",
                EntryDeviceID = deviceID,
                Relay = new Relay
                {
                    DeviceID = deviceID,
                    Port = 0 // 1st relay
                },
                Sensor = new Sensor
                {
                    DeviceID = deviceID,
                    Port = 0, // 1st input port
                    Type = SwitchType.NormallyOpen
                },
                Button = new ExitButton
                {
                    DeviceID = deviceID,
                    Port = 1, // 2nd input port
                    Type = SwitchType.NormallyOpen
                },
                AutoLockTimeout = 3, // locked after 3 seconds
                HeldOpenTimeout = 10 // held open alarm after 10 seconds
            };

            doorSvc.DeleteAll(deviceID);
            doorSvc.Add(deviceID, new DoorInfo[] { door });

            Console.WriteLine(Environment.NewLine + "===== Door Test =====}" + Environment.NewLine);

            var testDoors = doorSvc.GetList(deviceID);
            Console.WriteLine("Test Doors: {0}" + Environment.NewLine, testDoors);

            Console.WriteLine(">> Try to authenticate a registered credential. It should fail since you can access a door only with a proper access group.");
            KeyInput.PressEnter(">> Press ENTER for the next test." + Environment.NewLine);
        }

        async Task TestAccessGroupAsync(uint deviceID)
        {
            var userID = logTest.GetUserID(deviceID);

            if (userID == null)
            {
                Console.WriteLine("!! Cannot find ACCESS_DENIED event. You should have tried to authenticate a registered credentail for the test.");
                return;
            }

            // Backup access groups
            var origGroups = accessSvc.GetList(deviceID);
            var origLevels = accessSvc.GetLevelList(deviceID);
            var origUserAccessGroups = await userSvc.GetAccessGroupAsync(deviceID, new string[] { userID });

            Console.WriteLine("Original Access Groups: {0}", origGroups);
            Console.WriteLine("Original Access Levels: {0}", origLevels);
            Console.WriteLine("Original User Access Groups: {0}" + Environment.NewLine, origUserAccessGroups);

            accessSvc.DeleteAll(deviceID);
            accessSvc.DeleteAllLevel(deviceID);

            // Make an access group and assign it to the user
            var doorSchedule = new DoorSchedule { DoorID = TEST_DOOR_ID, ScheduleID = ALWAYS_SCHEDULE_ID }; // can access the test door all the time
            var accessLevel = new AccessLevel { ID = TEST_ACCESS_LEVEL_ID };
            accessLevel.DoorSchedules.Add(doorSchedule);

            accessSvc.AddLevel(deviceID, new AccessLevel[] { accessLevel });

            var accessGroup = new AccessGroup { ID = TEST_ACCESS_GROUP_ID };
            accessGroup.LevelIDs.Add(TEST_ACCESS_LEVEL_ID);

            accessSvc.Add(deviceID, new AccessGroup[] { accessGroup });

            var userAccessGroup = new UserAccessGroup { UserID = userID };
            userAccessGroup.AccessGroupIDs.Add(TEST_ACCESS_GROUP_ID);

            await userSvc.SetAccessGroupAsync(deviceID, new UserAccessGroup[] { userAccessGroup });

            var newGroups = accessSvc.GetList(deviceID);
            var newLevels = accessSvc.GetLevelList(deviceID);
            var newUserAccessGroups = await userSvc.GetAccessGroupAsync(deviceID, new string[] { userID });

            Console.WriteLine("Test Access Groups: {0}", newGroups);
            Console.WriteLine("Test Access Levels: {0}" + Environment.NewLine, newLevels);
            Console.WriteLine("Test User Access Groups: {0}", newUserAccessGroups);

            Console.WriteLine(">> Try to authenticate the same registered credential. It should succeed since the access group is added.");
            KeyInput.PressEnter(">> Press ENTER for the next test." + Environment.NewLine);

            TestLock(deviceID);

            // Restore access groups
            var userArray = new UserAccessGroup[origUserAccessGroups.Count];
            origUserAccessGroups.CopyTo(userArray, 0);
            await userSvc.SetAccessGroupAsync(deviceID, userArray);
            accessSvc.DeleteAll(deviceID);
            accessSvc.DeleteAllLevel(deviceID);

            if (origGroups.Count > 0)
            {
                var groupArray = new AccessGroup[origGroups.Count];
                origGroups.CopyTo(groupArray, 0);
                accessSvc.Add(deviceID, groupArray);
            }

            if (origLevels.Count > 0)
            {
                var levelArray = new AccessLevel[origLevels.Count];
                origLevels.CopyTo(levelArray, 0);
                accessSvc.AddLevel(deviceID, levelArray);
            }
        }

        void TestLock(uint deviceID)
        {
            KeyInput.PressEnter(">> Press ENTER to unlock the door." + Environment.NewLine);

            var doorIDs = new uint[] { TEST_DOOR_ID };
            doorSvc.Unlock(deviceID, doorIDs, DoorFlag.Operator);
            var doorStatus = doorSvc.GetStatus(deviceID);
            Console.WriteLine("Status after unlocking the door: {0}", doorStatus);

            KeyInput.PressEnter(">> Press ENTER to lock the door." + Environment.NewLine);

            doorSvc.Lock(deviceID, doorIDs, DoorFlag.Operator);
            doorStatus = doorSvc.GetStatus(deviceID);
            Console.WriteLine("Status after locking the door: {0}", doorStatus);

            Console.WriteLine(">> Try to authenticate the same registered credential. The relay should not work since the door is locked by the operator with the higher priority.");
            KeyInput.PressEnter(">> Press ENTER to release the door." + Environment.NewLine);

            doorSvc.Release(deviceID, doorIDs, DoorFlag.Operator);
            doorStatus = doorSvc.GetStatus(deviceID);
            Console.WriteLine("Status after releasing the door flag: {0}", doorStatus);

            Console.WriteLine(">> Try to authenticate the same registered credential. The relay should work since the door flag is cleared.");
            KeyInput.PressEnter(">> Press ENTER for the next test." + Environment.NewLine);
        }
    }
}

