using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.Rs485;

namespace example
{
    public class Rs485Svc
    {
        private RS485.RS485Client rs485Client;

        public Rs485Svc(Channel channel)
        {
            rs485Client = new RS485.RS485Client(channel);
        }

        public RS485Config GetConfig(uint deviceID)
        {
            var request = new GetConfigRequest { DeviceID = deviceID };
            var response = rs485Client.GetConfig(request);

            return response.Config;
        }

        public void SetConfig(uint deviceID, RS485Config config)
        {
            var request = new SetConfigRequest { DeviceID = deviceID, Config = config };
            rs485Client.SetConfig(request);
        }

        public RepeatedField<SlaveDeviceInfo> SearchSlave(uint deviceID)
        {
            var request = new SearchDeviceRequest { DeviceID = deviceID };
            var response = rs485Client.SearchDevice(request);

            return response.SlaveInfos;
        }

        public RepeatedField<SlaveDeviceInfo> GetSlave(uint deviceID)
        {
            var request = new GetDeviceRequest { DeviceID = deviceID };
            var response = rs485Client.GetDevice(request);

            return response.SlaveInfos;
        }

        public void SetSlave(uint deviceID, SlaveDeviceInfo[] slaveInfos)
        {
            var request = new SetDeviceRequest { DeviceID = deviceID };
            request.SlaveInfos.AddRange(slaveInfos);

            rs485Client.SetDevice(request);
        }
    }
}