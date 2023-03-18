using Google.Protobuf.Collections;
using Gsdk.Action;
using Gsdk.ApbZone;
using Gsdk.Rs485;
using System;

namespace example
{
    class APBTest
    {
        private const int TEST_ZONE_ID = 1;

        private ApbZoneSvc apbSvc;

        public APBTest(ApbZoneSvc apbSvc)
        {
            this.apbSvc = apbSvc;
        }

        public void Test(uint deviceID, RepeatedField<SlaveDeviceInfo> slaves)
        {
            // Backup the original zones
            var origApbZones = apbSvc.Get(deviceID);
            Console.WriteLine("Original APB Zones: {0}", origApbZones);
            apbSvc.DeleteAll(deviceID);

            var testZone = MakeZone(deviceID, slaves);
            apbSvc.Add(deviceID, new ZoneInfo[] { testZone });

            Console.WriteLine(Environment.NewLine + "===== Anti Passback Zone Test =====" + Environment.NewLine);
            Console.WriteLine("Test Zone: {0}" + Environment.NewLine, testZone);

            Console.WriteLine(">> Authenticate a regsistered credential on the entry device({0}) and/or the exit device({1}) to test if the APB zone works correctly", deviceID, slaves[0].DeviceID);
            KeyInput.PressEnter(">> Press ENTER for the next test." + Environment.NewLine);

            KeyInput.PressEnter(">> Press ENTER after generating an APB violation." + Environment.NewLine);

            apbSvc.ClearAll(deviceID, TEST_ZONE_ID);

            Console.WriteLine(">> The APB records are cleared. Try to authenticate the last credential which caused the APB violation. It should succeed since the APB records are cleared.");
            KeyInput.PressEnter(">> Press ENTER to finish the test." + Environment.NewLine);

            // Restore the original zones
            apbSvc.DeleteAll(deviceID);
            if (origApbZones.Count > 0)
            {
                var apbArray = new ZoneInfo[origApbZones.Count];
                origApbZones.CopyTo(apbArray, 0);
                apbSvc.Add(deviceID, apbArray);
            }
        }

        public ZoneInfo MakeZone(uint deviceID, RepeatedField<SlaveDeviceInfo> slaves)
        {
            // Make a zone with the master device and the 1st slave device
            var zone = new ZoneInfo
            {
                ZoneID = TEST_ZONE_ID,
                Name = "Test APB Zone",
                Type = Gsdk.ApbZone.Type.Hard,
                ResetDuration = 0, // indefinite
            };

            zone.Members.Add(new Member { DeviceID = deviceID, ReaderType = ReaderType.Entry });
            zone.Members.Add(new Member { DeviceID = slaves[0].DeviceID, ReaderType = ReaderType.Exit });

            var relaySignal = new Signal { Count = 3, OnDuration = 500, OffDuration = 500 };
            zone.Actions.Add(new Gsdk.Action.Action
            {
                DeviceID = deviceID,
                Type = ActionType.ActionRelay,
                Relay = new RelayAction { RelayIndex = 0, Signal = relaySignal }
            });

            return zone;
        }
    }
}

