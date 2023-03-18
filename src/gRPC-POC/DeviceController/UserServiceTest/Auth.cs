using Gsdk.Auth;
using System;

namespace example
{
    class AuthTest
    {
        private AuthSvc authSvc;

        public AuthTest(AuthSvc svc)
        {
            authSvc = svc;
        }

        public AuthConfig PrepareAuthConfig(uint deviceID)
        {
            // Backup the original configuration
            AuthConfig origConfig = authSvc.GetConfig(deviceID);
            Console.WriteLine("Original Config: {0}" + Environment.NewLine, origConfig);

            // Enable private authentication for test
            AuthConfig testConfig = origConfig.Clone();
            testConfig.UsePrivateAuth = true;
            authSvc.SetConfig(deviceID, testConfig);

            return origConfig;
        }

        public void Test(uint deviceID, bool extendedAuthSupported)
        {
            Console.WriteLine(Environment.NewLine + "===== Auth Mode Test =====" + Environment.NewLine);

            AuthConfig config = new AuthConfig { MatchTimeout = 10, AuthTimeout = 15 };

            if (extendedAuthSupported)
            {
                config.AuthSchedules.Add(new AuthSchedule { Mode = AuthMode.AuthExtModeCardOnly, ScheduleID = 1 }); // Card Only, Always
                config.AuthSchedules.Add(new AuthSchedule { Mode = AuthMode.AuthExtModeFingerprintOnly, ScheduleID = 1 }); // Fingerprint Only, Always
                config.AuthSchedules.Add(new AuthSchedule { Mode = AuthMode.AuthExtModeFaceOnly, ScheduleID = 1 }); // Face Only, Always
            }
            else
            {
                config.AuthSchedules.Add(new AuthSchedule { Mode = AuthMode.CardOnly, ScheduleID = 1 }); // Card Only, Always
                config.AuthSchedules.Add(new AuthSchedule { Mode = AuthMode.BiometricOnly, ScheduleID = 1 }); // Biometric Only, Always
            }

            authSvc.SetConfig(deviceID, config);

            KeyInput.PressEnter(">> Try to authenticate card or fingerprint or face. And, press ENTER for the next test." + Environment.NewLine);

            config.AuthSchedules.Clear();

            if (extendedAuthSupported)
            {
                config.AuthSchedules.Add(new AuthSchedule { Mode = AuthMode.AuthExtModeCardFace, ScheduleID = 1 }); // Card + Face, Always
                config.AuthSchedules.Add(new AuthSchedule { Mode = AuthMode.AuthExtModeCardFingerprint, ScheduleID = 1 }); // Card + Fingerprint, Always
            }
            else
            {
                config.AuthSchedules.Add(new AuthSchedule { Mode = AuthMode.CardBiometric, ScheduleID = 1 }); // Card + Biometric, Always
            }

            authSvc.SetConfig(deviceID, config);

            KeyInput.PressEnter(">> Try to authenticate (card + fingerprint) or (card + face). And, press ENTER for the next test." + Environment.NewLine);
        }
    }
}

