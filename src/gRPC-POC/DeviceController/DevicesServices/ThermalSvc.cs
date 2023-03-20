using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.Thermal;

namespace example
{
    public class ThermalSvc
    {
        private Thermal.ThermalClient thermalClient;

        public ThermalSvc(Channel channel)
        {
            thermalClient = new Thermal.ThermalClient(channel);
        }

        public ThermalConfig GetConfig(uint deviceID)
        {
            var request = new GetConfigRequest { DeviceID = deviceID };
            var response = thermalClient.GetConfig(request);

            return response.Config;
        }

        public void SetConfig(uint deviceID, ThermalConfig config)
        {
            var request = new SetConfigRequest { DeviceID = deviceID, Config = config };
            thermalClient.SetConfig(request);
        }

        public RepeatedField<TemperatureLog> GetTemperatureLog(uint deviceID, uint startEventID, uint maxNumOfLog)
        {
            var request = new GetTemperatureLogRequest { DeviceID = deviceID, StartEventID = startEventID, MaxNumOfLog = maxNumOfLog };
            var response = thermalClient.GetTemperatureLog(request);

            return response.TemperatureEvents;
        }
    }
}