using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.ConnectMaster;

namespace example
{
    public class ConnectMasterSvc
    {
        private const int SEARCH_TIMEOUT_MS = 5000;

        private ConnectMaster.ConnectMasterClient connectMasterClient;

        public ConnectMasterSvc(Channel channel)
        {
            connectMasterClient = new ConnectMaster.ConnectMasterClient(channel);
        }

        public async Task<RepeatedField<Gsdk.Connect.DeviceInfo>> GetDeviceListAsync(string gatewayID)
        {
            var request = new GetDeviceListRequest { GatewayID = gatewayID };
            var response = await connectMasterClient.GetDeviceListAsync(request);

            return response.DeviceInfos;
        }

        public async Task<RepeatedField<Gsdk.Connect.SearchDeviceInfo>> SearchDeviceAsync(string gatewayID)
        {
            var request = new SearchDeviceRequest { GatewayID = gatewayID, Timeout = SEARCH_TIMEOUT_MS };
            var response = await connectMasterClient.SearchDeviceAsync(request);

            return response.DeviceInfos;
        }

        public async Task<uint> ConnectAsync(string gatewayID, Gsdk.Connect.ConnectInfo connectInfo)
        {
            var request = new ConnectRequest { GatewayID = gatewayID, ConnectInfo = connectInfo };
            var response = await connectMasterClient.ConnectAsync(request);

            return response.DeviceID;
        }

        public async Task DisconnectAsync(uint[] deviceIDs)
        {
            var request = new DisconnectRequest { };
            request.DeviceIDs.AddRange(deviceIDs);

            await connectMasterClient.DisconnectAsync(request);
        }

        public async Task DisconnectAllAsync(string gatewayID)
        {
            var request = new DisconnectAllRequest { GatewayID = gatewayID };

            await connectMasterClient.DisconnectAllAsync(request);
        }

        public async Task AddAsyncConnectionAsync(string gatewayID, Gsdk.Connect.AsyncConnectInfo[] asyncConns)
        {
            var request = new AddAsyncConnectionRequest { GatewayID = gatewayID };
            request.ConnectInfos.AddRange(asyncConns);

            await connectMasterClient.AddAsyncConnectionAsync(request);
        }

        public async Task DeleteAsyncConnectionAsync(string gatewayID, uint[] deviceIDs)
        {
            var request = new DeleteAsyncConnectionRequest { GatewayID = gatewayID };
            request.DeviceIDs.AddRange(deviceIDs);

            await connectMasterClient.DeleteAsyncConnectionAsync(request);
        }

        public async Task<RepeatedField<Gsdk.Connect.PendingDeviceInfo>> GetPendingListAsync(string gatewayID)
        {
            var request = new GetPendingListRequest { GatewayID = gatewayID };
            var response = await connectMasterClient.GetPendingListAsync(request);

            return response.DeviceInfos;
        }

        public async Task<Gsdk.Connect.AcceptFilter> GetAcceptFilterAsync(string gatewayID)
        {
            var request = new GetAcceptFilterRequest { GatewayID = gatewayID };
            var response = await connectMasterClient.GetAcceptFilterAsync(request);

            return response.Filter;
        }

        public async Task SetAcceptFilterAsync(string gatewayID, Gsdk.Connect.AcceptFilter filter)
        {
            var request = new SetAcceptFilterRequest { GatewayID = gatewayID, Filter = filter };
            await connectMasterClient.SetAcceptFilterAsync(request);
        }

        public async Task SetConnectionModeAsync(uint[] deviceIDs, Gsdk.Connect.ConnectionMode mode)
        {
            var request = new SetConnectionModeMultiRequest { ConnectionMode = mode };
            request.DeviceIDs.AddRange(deviceIDs);

            await connectMasterClient.SetConnectionModeMultiAsync(request);
        }

        public async Task EnableSSLAsync(uint[] deviceIDs)
        {
            var request = new EnableSSLMultiRequest { };
            request.DeviceIDs.AddRange(deviceIDs);

            await connectMasterClient.EnableSSLMultiAsync(request);
        }

        public async Task DisableSSLAsync(uint[] deviceIDs)
        {
            var request = new DisableSSLMultiRequest { };
            request.DeviceIDs.AddRange(deviceIDs);

            await connectMasterClient.DisableSSLMultiAsync(request);
        }

        public async Task<IAsyncStreamReader<Gsdk.Connect.StatusChange>> SubscribeAsync(int queueSize)
        {
            var request = new SubscribeStatusRequest { QueueSize = queueSize };
            var streamCall = connectMasterClient.SubscribeStatus(request);

            return streamCall.ResponseStream;
        }
    }
}