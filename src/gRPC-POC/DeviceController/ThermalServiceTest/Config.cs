using Gsdk.Thermal;
using System;

namespace example
{
    class ConfigTest
    {
        private ThermalSvc thermalSvc;

        public ConfigTest(ThermalSvc svc)
        {
            thermalSvc = svc;
        }

        public void Test(uint deviceID, ThermalConfig config)
        {
            Console.WriteLine(Environment.NewLine + "===== Test for ThermalConfig =====" + Environment.NewLine);

            // Backup the original configuration
            ThermalConfig origConfig = config.Clone();

            // Set options for the test
            config.AuditTemperature = true;
            config.CheckMode = CheckMode.Hard;

            // (1) Set check order to AFTER_AUTH
            config.CheckOrder = CheckOrder.AfterAuth;
            thermalSvc.SetConfig(deviceID, config);

            Console.WriteLine("(1) The Check Order is set to AFTER_AUTH. The device will measure the temperature only after successful authentication. Try to authenticate faces." + Environment.NewLine);
            KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);

            // (2) Set check order to BEFORE_AUTH
            config.CheckOrder = CheckOrder.BeforeAuth;
            thermalSvc.SetConfig(deviceID, config);

            Console.WriteLine("(2) The Check Order is set to BEFORE_AUTH. The device will try to authenticate a user only when the user's temperature is within the threshold. Try to authenticate faces." + Environment.NewLine);
            KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);

            // (3) Set check order to WITHOUT_AUTH
            config.CheckOrder = CheckOrder.WithoutAuth;
            thermalSvc.SetConfig(deviceID, config);

            Console.WriteLine("(3) The Check Order is set to WITHOUT_AUTH. Any user whose temperature is within the threshold will be allowed to access. Try to authenticate faces." + Environment.NewLine);
            KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);

            // (4) Set check order to AFTER_AUTH with too low threshold
            config.CheckOrder = CheckOrder.AfterAuth;
            config.TemperatureThresholdHigh = 3500;
            config.TemperatureThresholdLow = 3000;
            thermalSvc.SetConfig(deviceID, config);

            Console.WriteLine("(4) To reproduce the case of high temperature, the Check Order is set to AFTER_AUTH with the threshold of 35 degree Celsius. Most temperature check will fail, now. Try to authenticate faces." + Environment.NewLine);
            KeyInput.PressEnter(">> Press ENTER if you finish testing this mode." + Environment.NewLine);

            // Restore the original configuration   
            thermalSvc.SetConfig(deviceID, origConfig);
        }
    }
}

