using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.Event;
using System.Text.Json;

namespace example
{
    public delegate void EventCallback(EventLog logEvent);

    public class EventService
    {
        private const int MONITORING_QUEUE_SIZE = 8;

        private Event.EventClient _eventClient;
        private CancellationTokenSource _cancellationTokenSource;
        private EventCallback _callback;
        private EventCodeMap _codeMap;

        public EventService(Channel channel)
        {
            _eventClient = new Event.EventClient(channel);
        }

        public async Task SetCallback(EventCallback eventCallback)
        {
            _callback = eventCallback;

            await Task.CompletedTask;
        }

        public async Task<RepeatedField<EventLog>> GetLogAsync(uint deviceID, uint startEventID, uint maxNumOfLog)
        {
            var request = new GetLogRequest { DeviceID = deviceID, StartEventID = startEventID, MaxNumOfLog = maxNumOfLog };

            var response = await _eventClient.GetLogAsync(request);

            return response.Events;
        }

        public async Task<RepeatedField<EventLog>> GetLogWithFilterAsync(uint deviceID, uint startEventID, uint maxNumOfLog, EventFilter filter)
        {
            var request = new GetLogWithFilterRequest { DeviceID = deviceID, StartEventID = startEventID, MaxNumOfLog = maxNumOfLog };
            
            request.Filters.Add(filter);

            var response = await _eventClient.GetLogWithFilterAsync(request);

            return response.Events;
        }

        public RepeatedField<ImageLog> GetImageLog(uint deviceID, uint startEventID, uint maxNumOfLog)
        {
            var request = new GetImageLogRequest { DeviceID = deviceID, StartEventID = startEventID, MaxNumOfLog = maxNumOfLog };

            var response = _eventClient.GetImageLog(request);

            return response.ImageEvents;
        }

        public async Task StartMonitoringAsync(uint deviceID)
        {
            try
            {
                var enableRequest = new EnableMonitoringRequest { DeviceID = deviceID };
             
                _eventClient.EnableMonitoring(enableRequest);

                var subscribeRequest = new SubscribeRealtimeLogRequest { DeviceIDs = { deviceID }, QueueSize = MONITORING_QUEUE_SIZE };
                
                var call = _eventClient.SubscribeRealtimeLog(subscribeRequest);

                _cancellationTokenSource = new CancellationTokenSource();

                await ReceiveEvents(this, call.ResponseStream, _cancellationTokenSource.Token);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot start monitoring {0}: {1}", deviceID, e);
                
                throw;
            }
        }

        public async Task EnableMonitoringAsync(uint deviceID)
        {
            try
            {
                var enableRequest = new EnableMonitoringRequest { DeviceID = deviceID };
                
                await _eventClient.EnableMonitoringAsync(enableRequest);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot enable monitoring {0}: {1}", deviceID, e);
                throw;
            }
        }

        public async Task StartMonitoringMultiAsync(uint[] deviceIDs)
        {
            try
            {
                var enableRequest = new EnableMonitoringMultiRequest { };
         
                enableRequest.DeviceIDs.AddRange(deviceIDs);
                
                await _eventClient.EnableMonitoringMultiAsync(enableRequest);

                var subscribeRequest = new SubscribeRealtimeLogRequest { DeviceIDs = { deviceIDs }, QueueSize = MONITORING_QUEUE_SIZE };
               
                var call = _eventClient.SubscribeRealtimeLog(subscribeRequest);

                _cancellationTokenSource = new CancellationTokenSource();

                await ReceiveEvents(this, call.ResponseStream, _cancellationTokenSource.Token);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot enable monitoring {0}: {1}", deviceIDs, e);
                throw;
            }
        }

        public async Task StartMonitoringAsync()
        {
            try
            {
                var subscribeRequest = new SubscribeRealtimeLogRequest { QueueSize = MONITORING_QUEUE_SIZE };
                
                var call = _eventClient.SubscribeRealtimeLog(subscribeRequest);

                _cancellationTokenSource = new CancellationTokenSource();

                await ReceiveEvents(this, call.ResponseStream, _cancellationTokenSource.Token);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot start monitoring: {0}", e);
                
                throw;
            }
        }

        static async Task ReceiveEvents(EventService svc, IAsyncStreamReader<EventLog> stream, CancellationToken token)
        {
            Console.WriteLine("Start receiving real-time events");

            try
            {
                while (await stream.MoveNext(token))
                {
                    var eventLog = stream.Current;

                    if (svc._callback != null)
                    {
                        svc._callback(eventLog);
                    }
                    else
                    {
                        Console.WriteLine("Event: {0}", eventLog);
                    }
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                {
                    Console.WriteLine("Monitoring is cancelled");
                }
                else
                {
                    Console.WriteLine("Monitoring error: {0}", e);
                }
            }
            finally
            {
                Console.WriteLine("Stop receiving real-time events");
            }
        }

        public async Task StopMonitoringAsync(uint deviceID)
        {
            var disableRequest = new DisableMonitoringRequest { DeviceID = deviceID };
            
            await _eventClient.DisableMonitoringAsync(disableRequest);

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public async Task StopMonitoringMultiAsync(uint[] deviceIDs)
        {
            var disableRequest = new DisableMonitoringMultiRequest { };
            
            disableRequest.DeviceIDs.AddRange(deviceIDs);
            
            await _eventClient.DisableMonitoringMultiAsync(disableRequest);

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public async Task DisableMonitoringAsync(uint deviceID)
        {
            var disableRequest = new DisableMonitoringRequest { DeviceID = deviceID };
            
            await _eventClient.DisableMonitoringAsync(disableRequest);
        }

        public void StopMonitoring()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public void InitCodeMap(string filename)
        {
            var jsonData = File.ReadAllText(filename);
            
            if (jsonData == null)
            {
                Console.WriteLine($"Cannot read code map file: {filename}");
            
                throw new Exception($"Cannot read code map file: {filename}");
            }

            _codeMap = JsonSerializer.Deserialize<EventCodeMap>(jsonData);
        }

        public string GetEventString(uint eventCode, uint subCode)
        {
            if (_codeMap == null)
            {
                return string.Format("No code map(0x{0:X})", eventCode | subCode);
            }

            for (int i = 0; i < _codeMap.entries.Count; i++)
            {
                if (eventCode == _codeMap.entries[i].event_code && subCode == _codeMap.entries[i].sub_code)
                {
                    return _codeMap.entries[i].desc;
                }
            }

            return string.Format("Unknown event(0x{0:X})", eventCode | subCode);
        }
    }
}