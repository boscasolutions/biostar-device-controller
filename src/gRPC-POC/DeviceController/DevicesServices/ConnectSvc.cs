using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.Connect;

namespace example
{
    public class ConnectSvc
    {
        private const int SEARCH_TIMEOUT_MS = 5000;

        private Connect.ConnectClient connectClient;

        public ConnectSvc(Channel channel)
        {
            connectClient = new Connect.ConnectClient(channel);
        }

        public async Task<RepeatedField<DeviceInfo>> GetDeviceListAsync()
        {
            var request = new GetDeviceListRequest { };
            var response = await connectClient.GetDeviceListAsync(request);

            return response.DeviceInfos;
        }

        public async Task<RepeatedField<SearchDeviceInfo>> SearchDeviceAsync()
        {
            var request = new SearchDeviceRequest { Timeout = SEARCH_TIMEOUT_MS };
            var response = await connectClient.SearchDeviceAsync(request);

            return response.DeviceInfos;
        }

        public async Task<uint> ConnectAsync(ConnectInfo connectInfo)
        {
            var request = new ConnectRequest { ConnectInfo = connectInfo };
            var response = await connectClient.ConnectAsync(request);

            return response.DeviceID;
        }

        public async Task Disconnect(uint[] deviceIDs)
        {
            var request = new DisconnectRequest { };
            request.DeviceIDs.AddRange(deviceIDs);

            await connectClient.DisconnectAsync(request);
        }

        public async Task DisconnectAllAsync()
        {
            var request = new DisconnectAllRequest { };

            await connectClient.DisconnectAllAsync(request);
        }

        public async Task AddAsyncConnectionAsync(AsyncConnectInfo[] asyncConns)
        {
            var request = new AddAsyncConnectionRequest { };
            request.ConnectInfos.AddRange(asyncConns);

            await connectClient.AddAsyncConnectionAsync(request);
        }

        public void DeleteAsyncConnection(uint[] deviceIDs)
        {
            var request = new DeleteAsyncConnectionRequest { };
            request.DeviceIDs.AddRange(deviceIDs);

            connectClient.DeleteAsyncConnection(request);
        }

        public async Task<RepeatedField<PendingDeviceInfo>> GetPendingListAsync()
        {
            var request = new GetPendingListRequest { };
            var response = await connectClient.GetPendingListAsync(request);

            return response.DeviceInfos;
        }

        public async Task<AcceptFilter> GetAcceptFilterAsync()
        {
            var request = new GetAcceptFilterRequest { };
            var response = await connectClient.GetAcceptFilterAsync(request);

            return response.Filter;
        }

        public async Task SetAcceptFilterAsync(AcceptFilter filter)
        {
            var request = new SetAcceptFilterRequest { Filter = filter };
            await connectClient.SetAcceptFilterAsync(request);
        }

        public async Task SetConnectionModeAsync(uint[] deviceIDs, ConnectionMode mode)
        {
            var request = new SetConnectionModeMultiRequest { ConnectionMode = mode };
            request.DeviceIDs.AddRange(deviceIDs);

            await connectClient.SetConnectionModeMultiAsync(request);
        }

        public async Task EnableSSLAsync(uint[] deviceIDs)
        {
            var request = new EnableSSLMultiRequest { };
            request.DeviceIDs.AddRange(deviceIDs);

            await connectClient.EnableSSLMultiAsync(request);
        }

        public async Task DisableSSLAsync(uint[] deviceIDs)
        {
            var request = new DisableSSLMultiRequest { };
            request.DeviceIDs.AddRange(deviceIDs);

            await connectClient.DisableSSLMultiAsync(request);
        }

        public async Task<IAsyncStreamReader<StatusChange>> SubscribeAsync(int queueSize)
        {
            var request = new SubscribeStatusRequest { QueueSize = queueSize };
            var streamCall = connectClient.SubscribeStatus(request);

            return streamCall.ResponseStream;
        }
    }
}