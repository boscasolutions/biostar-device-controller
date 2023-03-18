using Gsdk.Tna;
using System;

namespace example
{
    class ConfigTest
    {
        private TNASvc tnaSvc;

        public ConfigTest(TNASvc svc)
        {
            tnaSvc = svc;
        }

        public TNAConfig Test(uint deviceID)
        {
            // Backup the original configuration
            var origConfig = tnaSvc.GetConfig(deviceID);
            Console.WriteLine("Original Config: {0}" + Environment.NewLine, origConfig);

            Console.WriteLine(Environment.NewLine + "===== Test for TNAConfig =====" + Environment.NewLine);

            // (1) BY_USER
            var newConfig = new TNAConfig { Mode = Mode.ByUser, Labels = { "In", "Out", "Scheduled In", "Fixed Out" } };
            tnaSvc.SetConfig(deviceID, newConfig);

            Console.WriteLine("(1) The T&A mode is set to BY_USER(optional). You can select a T&A key before authentication. Try to authenticate after selecting a T&A key." + Environment.NewLine);
            KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);

            // (2) IsRequired
            newConfig.IsRequired = true;
            tnaSvc.SetConfig(deviceID, newConfig);

            Console.WriteLine("(2) The T&A mode is set to BY_USER(mandatory). Try to authenticate without selecting a T&A key." + Environment.NewLine);
            KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);

            // (3) LAST_CHOICE
            newConfig.Mode = Mode.LastChoice;
            tnaSvc.SetConfig(deviceID, newConfig);

            Console.WriteLine("(3) The T&A mode is set to LAST_CHOICE. The T&A key selected by the previous user will be used. Try to authenticate multiple users." + Environment.NewLine);
            KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);

            // (4) BY_SCHEDULE
            newConfig.Mode = Mode.BySchedule;
            newConfig.Schedules.AddRange(new uint[] { 0, 0, 1 }); // Always for KEY_3(Scheduled_In)
            tnaSvc.SetConfig(deviceID, newConfig);

            Console.WriteLine("(4) The T&A mode is set to BY_SCHEDULE. The T&A key will be determined automatically by schedule. Try to authenticate without selecting a T&A key." + Environment.NewLine);
            KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);

            // (5) FIXED
            newConfig.Mode = Mode.Fixed;
            newConfig.Key = Key._4;
            tnaSvc.SetConfig(deviceID, newConfig);

            Console.WriteLine("(5) The T&A mode is set to FIXED(KEY_4). Try to authenticate without selecting a T&A key." + Environment.NewLine);
            KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);

            return origConfig;
        }
    }
}

