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

        public RepeatedField<Gsdk.Connect.DeviceInfo> GetDeviceList(string gatewayID)
        {
            var request = new GetDeviceListRequest { GatewayID = gatewayID };
            var response = connectMasterClient.GetDeviceList(request);

            return response.DeviceInfos;
        }

        public RepeatedField<Gsdk.Connect.SearchDeviceInfo> SearchDevice(string gatewayID)
        {
            var request = new SearchDeviceRequest { GatewayID = gatewayID, Timeout = SEARCH_TIMEOUT_MS };
            var response = connectMasterClient.SearchDevice(request);

            return response.DeviceInfos;
        }

        public uint Connect(string gatewayID, Gsdk.Connect.ConnectInfo connectInfo)
        {
            var request = new ConnectRequest { GatewayID = gatewayID, ConnectInfo = connectInfo };
            var response = connectMasterClient.Connect(request);

            return response.DeviceID;
        }

        public void Disconnect(uint[] deviceIDs)
        {
            var request = new DisconnectRequest { };
            request.DeviceIDs.AddRange(deviceIDs);

            connectMasterClient.Disconnect(request);
        }

        public void DisconnectAll(string gatewayID)
        {
            var request = new DisconnectAllRequest { GatewayID = gatewayID };

            connectMasterClient.DisconnectAll(request);
        }

        public void AddAsyncConnection(string gatewayID, Gsdk.Connect.AsyncConnectInfo[] asyncConns)
        {
            var request = new AddAsyncConnectionRequest { GatewayID = gatewayID };
            request.ConnectInfos.AddRange(asyncConns);

            connectMasterClient.AddAsyncConnection(request);
        }

        public void DeleteAsyncConnection(string gatewayID, uint[] deviceIDs)
        {
            var request = new DeleteAsyncConnectionRequest { GatewayID = gatewayID };
            request.DeviceIDs.AddRange(deviceIDs);

            connectMasterClient.DeleteAsyncConnection(request);
        }

        public RepeatedField<Gsdk.Connect.PendingDeviceInfo> GetPendingList(string gatewayID)
        {
            var request = new GetPendingListRequest { GatewayID = gatewayID };
            var response = connectMasterClient.GetPendingList(request);

            return response.DeviceInfos;
        }

        public Gsdk.Connect.AcceptFilter GetAcceptFilter(string gatewayID)
        {
            var request = new GetAcceptFilterRequest { GatewayID = gatewayID };
            var response = connectMasterClient.GetAcceptFilter(request);

            return response.Filter;
        }

        public void SetAcceptFilter(string gatewayID, Gsdk.Connect.AcceptFilter filter)
        {
            var request = new SetAcceptFilterRequest { GatewayID = gatewayID, Filter = filter };
            connectMasterClient.SetAcceptFilter(request);
        }

        public void SetConnectionMode(uint[] deviceIDs, Gsdk.Connect.ConnectionMode mode)
        {
            var request = new SetConnectionModeMultiRequest { ConnectionMode = mode };
            request.DeviceIDs.AddRange(deviceIDs);

            connectMasterClient.SetConnectionModeMulti(request);
        }

        public void EnableSSL(uint[] deviceIDs)
        {
            var request = new EnableSSLMultiRequest { };
            request.DeviceIDs.AddRange(deviceIDs);

            connectMasterClient.EnableSSLMulti(request);
        }

        public void DisableSSL(uint[] deviceIDs)
        {
            var request = new DisableSSLMultiRequest { };
            request.DeviceIDs.AddRange(deviceIDs);

            connectMasterClient.DisableSSLMulti(request);
        }

        public IAsyncStreamReader<Gsdk.Connect.StatusChange> Subscribe(int queueSize)
        {
            var request = new SubscribeStatusRequest { QueueSize = queueSize };
            var streamCall = connectMasterClient.SubscribeStatus(request);

            return streamCall.ResponseStream;
        }
    }
}