using Gsdk.Device;
using System;

namespace example
{
    class DeviceTest
    {
        private DeviceSvc deviceSvc;

        public DeviceTest(DeviceSvc svc)
        {
            deviceSvc = svc;
        }

        public CapabilityInfo Test(uint deviceID)
        {
            var info = deviceSvc.GetInfo(deviceID);

            Console.WriteLine("Device info: {0}" + Environment.NewLine, info);

            var capabilityInfo = deviceSvc.GetCapabilityInfo(deviceID);

            Console.WriteLine("Capability info: {0}" + Environment.NewLine, capabilityInfo);

            return capabilityInfo;
        }
    }
}

