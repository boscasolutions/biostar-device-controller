using Grpc.Core;
using Gsdk.Connect;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace example
{
    public delegate void ConnectionCallback(uint deviceID);

    class DeviceMgr
    {
        private ConnectSvc connectSvc;
        private TestConfig testConfig;
        private List<uint> connectedIDs;
        private ConnectionCallback connCallback;
        private CancellationTokenSource cancelToken;

        private const int STATUS_QUEUE_SIZE = 16;

        public DeviceMgr(ConnectSvc svc, TestConfig config)
        {
            connectSvc = svc;
            testConfig = config;

            connectedIDs = new List<uint>();
        }

        public async Task ConnectToDevicesAsync()
        {
            var connInfos = testConfig.GetAsyncConnectInfo();

            await connectSvc.AddAsyncConnectionAsync(connInfos);
        }

        public async Task DeleteConnectionAsync()
        {
            if (connectedIDs.Count > 0)
            {
                connectSvc.DeleteAsyncConnection(connectedIDs.ToArray());
            }

            if (cancelToken != null)
            {
                cancelToken.Cancel();
            }
        }

        public async Task<uint[]> GetConnectedDevicesAsync(bool refreshList)
        {
            if (refreshList)
            {
                var devInfos = await connectSvc.GetDeviceListAsync();
                connectedIDs.Clear();

                foreach (DeviceInfo dev in devInfos)
                {
                    if (dev.Status == Gsdk.Connect.Status.TcpConnected || dev.Status == Gsdk.Connect.Status.TlsConnected)
                    {
                        connectedIDs.Add(dev.DeviceID);
                    }
                }
            }

            return connectedIDs.ToArray();
        }

        public async Task HandleConnectionAsync(ConnectionCallback callback)
        {
            connCallback = callback;

            var devStatusStream = await connectSvc.SubscribeAsync(STATUS_QUEUE_SIZE);

            cancelToken = new CancellationTokenSource();

            ReceiveStatus(this, devStatusStream, cancelToken.Token);
        }

        static async void ReceiveStatus(DeviceMgr mgr, IAsyncStreamReader<StatusChange> stream, CancellationToken token)
        {
            try
            {
                while (await stream.MoveNext(token))
                {
                    var statusChange = stream.Current;
                    if (statusChange.Status == Gsdk.Connect.Status.TcpConnected || statusChange.Status == Gsdk.Connect.Status.TlsConnected)
                    {
                        mgr.connectedIDs.Add(statusChange.DeviceID);

                        if (mgr.connCallback != null)
                        {
                            mgr.connCallback(statusChange.DeviceID);
                        }
                    }
                    else if (statusChange.Status == Gsdk.Connect.Status.Disconnected)
                    {
                        mgr.connectedIDs.Remove(statusChange.DeviceID);
                    }

                    if (statusChange.Status != Gsdk.Connect.Status.TlsNotAllowed && statusChange.Status != Gsdk.Connect.Status.TcpNotAllowed)
                    {
                        Console.WriteLine("\n[Status] Device status change: {0}\n", statusChange);
                    }
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                {
                    Console.WriteLine("Subscription is cancelled");
                }
                else
                {
                    Console.WriteLine("Monitoring error: {0}", e);
                }
            }
        }
    }
}