using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.ApbZone;
using Gsdk.Zone;

namespace example
{
    public class ApbZoneSvc
    {
        private APBZone.APBZoneClient apbClient;

        public ApbZoneSvc(Channel channel)
        {
            apbClient = new APBZone.APBZoneClient(channel);
        }

        public RepeatedField<ZoneInfo> Get(uint deviceID)
        {
            var request = new GetRequest { DeviceID = deviceID };
            var response = apbClient.Get(request);

            return response.Zones;
        }

        public RepeatedField<ZoneStatus> GetStatus(uint deviceID)
        {
            var request = new GetStatusRequest { DeviceID = deviceID };
            var response = apbClient.GetStatus(request);

            return response.Status;
        }


        public void Add(uint deviceID, ZoneInfo[] zones)
        {
            var request = new AddRequest { DeviceID = deviceID };
            request.Zones.AddRange(zones);
            apbClient.Add(request);
        }

        public void DeleteAll(uint deviceID)
        {
            var request = new DeleteAllRequest { DeviceID = deviceID };
            apbClient.DeleteAll(request);
        }

        public void ClearAll(uint deviceID, uint zoneID)
        {
            var request = new ClearAllRequest { DeviceID = deviceID, ZoneID = zoneID };
            apbClient.ClearAll(request);
        }
    }
}