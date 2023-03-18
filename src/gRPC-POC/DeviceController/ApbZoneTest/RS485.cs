using Google.Protobuf.Collections;
using Gsdk.Rs485;
using System;

namespace example
{
    public class RS485Test
    {
        private Rs485Svc rs485Svc;
        private RepeatedField<SlaveDeviceInfo> slaves;
        private RepeatedField<SlaveDeviceInfo> registeredSlaves;

        public RS485Test(Rs485Svc svc)
        {
            rs485Svc = svc;
            slaves = null;
            registeredSlaves = null;
        }

        public RepeatedField<SlaveDeviceInfo> GetSlaves()
        {
            return slaves;
        }

        public bool CheckSlaves(uint deviceID)
        {
            RS485Config config = rs485Svc.GetConfig(deviceID);

            bool hasMasterChannel = false;

            foreach (RS485Channel channel in config.Channels)
            {
                if (channel.Mode == Mode.Master)
                {
                    hasMasterChannel = true;
                    break;
                }
            }

            if (!hasMasterChannel)
            {
                Console.WriteLine("!! Only a master device can have slave devices. Skip the test.");
                return false;
            }

            slaves = rs485Svc.SearchSlave(deviceID);
            if (slaves.Count == 0)
            {
                Console.WriteLine("!! No slave device is configured. Configure and wire the slave devices first.");
                return false;
            }

            Console.WriteLine("Found Slaves: {0}", slaves);

            registeredSlaves = rs485Svc.GetSlave(deviceID);
            Console.WriteLine("Registered Slaves: {0}", registeredSlaves);

            if (registeredSlaves.Count == 0)
            {
                slaves[0].Enabled = true;

                var slaveArray = new SlaveDeviceInfo[slaves.Count];
                slaves.CopyTo(slaveArray, 0);
                rs485Svc.SetSlave(deviceID, slaveArray);
            }

            for (int i = 0; i < 10; i++)
            {
                var newSlaves = rs485Svc.SearchSlave(deviceID);

                if (newSlaves[0].Connected)
                {
                    break;
                }

                Console.WriteLine("Waiting for the slave to be connected {0}...", i);
            }

            return true;
        }

        public void RestoreSlaves(uint deviceID)
        {
            if (registeredSlaves != null)
            {
                var slaveArray = new SlaveDeviceInfo[registeredSlaves.Count];
                registeredSlaves.CopyTo(slaveArray, 0);

                rs485Svc.SetSlave(deviceID, slaveArray);
            }
        }
    }
}

