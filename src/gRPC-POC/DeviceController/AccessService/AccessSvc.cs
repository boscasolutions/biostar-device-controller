using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.Access;

namespace example
{
    public class AccessSvc
    {
        private Access.AccessClient accessClient;

        public AccessSvc(Channel channel)
        {
            accessClient = new Access.AccessClient(channel);
        }

        public RepeatedField<AccessGroup> GetList(uint deviceID)
        {
            var request = new GetListRequest { DeviceID = deviceID };
            var response = accessClient.GetList(request);

            return response.Groups;
        }

        public void Add(uint deviceID, AccessGroup[] groups)
        {
            var request = new AddRequest { DeviceID = deviceID };
            request.Groups.AddRange(groups);
            accessClient.Add(request);
        }

        public void DeleteAll(uint deviceID)
        {
            var request = new DeleteAllRequest { DeviceID = deviceID };
            accessClient.DeleteAll(request);
        }

        public RepeatedField<AccessLevel> GetLevelList(uint deviceID)
        {
            var request = new GetLevelListRequest { DeviceID = deviceID };
            var response = accessClient.GetLevelList(request);

            return response.Levels;
        }

        public void AddLevel(uint deviceID, AccessLevel[] levels)
        {
            var request = new AddLevelRequest { DeviceID = deviceID };
            request.Levels.AddRange(levels);
            accessClient.AddLevel(request);
        }

        public void DeleteAllLevel(uint deviceID)
        {
            var request = new DeleteAllLevelRequest { DeviceID = deviceID };
            accessClient.DeleteAllLevel(request);
        }
    }
}