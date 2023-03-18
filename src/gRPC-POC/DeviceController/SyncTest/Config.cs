using Gsdk.Connect;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace example
{
    class TestDeviceInfo
    {
        public uint device_id { get; set; }
        public string ip_addr { get; set; }
        public int port { get; set; }
        public bool use_ssl { get; set; }
        public uint last_event_id { get; set; }

        public AsyncConnectInfo convertToConnectInfo()
        {
            return new AsyncConnectInfo { DeviceID = device_id, IPAddr = ip_addr, Port = port, UseSSL = use_ssl };
        }
    }

    class ConfigData
    {
        public TestDeviceInfo enroll_device { get; set; }
        public IList<TestDeviceInfo> devices { get; set; }
    }

    class TestConfig
    {
        public ConfigData configData;
        private string configFile;

        public void Read(string configFile)
        {
            this.configFile = configFile;

            var jsonData = File.ReadAllText(configFile);
            configData = JsonSerializer.Deserialize<ConfigData>(jsonData);
        }

        public void Write()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonData = JsonSerializer.Serialize(configData, options);

            File.WriteAllText(configFile, jsonData);
        }

        public string GetConfigData()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(configData, options);
        }

        public AsyncConnectInfo[] GetAsyncConnectInfo()
        {
            List<AsyncConnectInfo> infos = new List<AsyncConnectInfo>();

            infos.Add(configData.enroll_device.convertToConnectInfo());

            for (int i = 0; i < configData.devices.Count; i++)
            {
                infos.Add(configData.devices[i].convertToConnectInfo());
            }

            return infos.ToArray();
        }

        public uint[] GetTargetDeviceIDs(uint[] connectedIDs)
        {
            List<uint> targetIDs = new List<uint>();

            foreach (uint devID in connectedIDs)
            {
                for (int i = 0; i < configData.devices.Count; i++)
                {
                    if (devID == configData.devices[i].device_id)
                    {
                        targetIDs.Add(devID);
                        break;
                    }
                }
            }

            return targetIDs.ToArray();
        }

        public TestDeviceInfo GetDeviceInfo(uint deviceID)
        {
            if (deviceID == configData.enroll_device.device_id)
            {
                return configData.enroll_device;
            }

            for (int i = 0; i < configData.devices.Count; i++)
            {
                if (deviceID == configData.devices[i].device_id)
                {
                    return configData.devices[i];
                }
            }

            return null;
        }

        public void UpdateLastEventID(uint deviceID, uint lastEventID)
        {
            bool updated = false;

            if (deviceID == configData.enroll_device.device_id)
            {
                configData.enroll_device.last_event_id = lastEventID;
                updated = true;
            }
            else
            {
                for (int i = 0; i < configData.devices.Count; i++)
                {
                    if (deviceID == configData.devices[i].device_id)
                    {
                        configData.devices[i].last_event_id = lastEventID;
                        updated = true;
                        break;
                    }
                }
            }

            if (updated)
            {
                Write();
            }
        }
    }
}

