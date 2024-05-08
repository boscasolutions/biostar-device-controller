using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using Gsdk.Event;
using Grpc.Core;
using Google.Protobuf.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace example
{
  public delegate void EventCallback(EventLog logEvent);

  class EventSvc
  {
    private const int MONITORING_QUEUE_SIZE = 8;

    private Event.EventClient eventClient;
    private CancellationTokenSource cancellationTokenSource;
    
    private EventCallback callback;

    private EventCodeMap codeMap;

    public EventSvc(Channel channel) {
      eventClient = new Event.EventClient(channel);
    }

    public void SetCallback(EventCallback eventCallback) {
      callback = eventCallback;
    }

    public RepeatedField<EventLog> GetLog(uint deviceID, uint startEventID, uint maxNumOfLog) {
      var request = new GetLogRequest{ DeviceID = deviceID, StartEventID = startEventID, MaxNumOfLog = maxNumOfLog };
      var response = eventClient.GetLog(request);

      return response.Events;
    }

    public RepeatedField<EventLog> GetLogWithFilter(uint deviceID, uint startEventID, uint maxNumOfLog, EventFilter filter) {
      var request = new GetLogWithFilterRequest{ DeviceID = deviceID, StartEventID = startEventID, MaxNumOfLog = maxNumOfLog };
      request.Filters.Add(filter);
      var response = eventClient.GetLogWithFilter(request);

      return response.Events;
    }    

    public RepeatedField<ImageLog> GetImageLog(uint deviceID, uint startEventID, uint maxNumOfLog) {
      var request = new GetImageLogRequest{ DeviceID = deviceID, StartEventID = startEventID, MaxNumOfLog = maxNumOfLog };
      var response = eventClient.GetImageLog(request);

      return response.ImageEvents;    
    }    

    public void StartMonitoring(uint deviceID) {
      try {
        var enableRequest = new EnableMonitoringRequest{ DeviceID = deviceID };
        eventClient.EnableMonitoring(enableRequest);

        var subscribeRequest = new SubscribeRealtimeLogRequest{ DeviceIDs = {deviceID}, QueueSize = MONITORING_QUEUE_SIZE };
        var call = eventClient.SubscribeRealtimeLog(subscribeRequest);

        cancellationTokenSource = new CancellationTokenSource();

        ReceiveEvents(this, call.ResponseStream, cancellationTokenSource.Token);
      } catch (RpcException e) {
        Console.WriteLine("Cannot start monitoring {0}: {1}", deviceID, e);
        throw;
      }
    }

    public void EnableMonitoring(uint deviceID) {
      try {
        var enableRequest = new EnableMonitoringRequest{ DeviceID = deviceID };
        eventClient.EnableMonitoring(enableRequest);
      } catch (RpcException e) {
        Console.WriteLine("Cannot enable monitoring {0}: {1}", deviceID, e);
        throw;
      }
    }    

    public void StartMonitoringMulti(uint[] deviceIDs) {
      try {
        var enableRequest = new EnableMonitoringMultiRequest{};
        enableRequest.DeviceIDs.AddRange(deviceIDs);
        eventClient.EnableMonitoringMulti(enableRequest);

        var subscribeRequest = new SubscribeRealtimeLogRequest{DeviceIDs = {deviceIDs}, QueueSize = MONITORING_QUEUE_SIZE};
        var call = eventClient.SubscribeRealtimeLog(subscribeRequest);

        cancellationTokenSource = new CancellationTokenSource();

        ReceiveEvents(this, call.ResponseStream, cancellationTokenSource.Token);
      } catch (RpcException e) {
        Console.WriteLine("Cannot enable monitoring {0}: {1}", deviceIDs, e);
        throw;
      }
    }

    public void StartMonitoring() {
      try {
        var subscribeRequest = new SubscribeRealtimeLogRequest{ QueueSize = MONITORING_QUEUE_SIZE };
        var call = eventClient.SubscribeRealtimeLog(subscribeRequest);

        cancellationTokenSource = new CancellationTokenSource();

        ReceiveEvents(this, call.ResponseStream, cancellationTokenSource.Token);
      } catch (RpcException e) {
        Console.WriteLine("Cannot start monitoring: {0}", e);
        throw;
      }
    }    

    static async void ReceiveEvents(EventSvc svc, IAsyncStreamReader<EventLog> stream, CancellationToken token) {
      Console.WriteLine("Start receiving real-time events");

      try {
        while(await stream.MoveNext(token)) {
          var eventLog = stream.Current;

          if(svc.callback != null) {
            svc.callback(eventLog);
          } else {
            Console.WriteLine("Event: {0}", eventLog);        
          }
        }
      } catch (RpcException e) {
        if(e.StatusCode == StatusCode.Cancelled) {
          Console.WriteLine("Monitoring is cancelled");
        } else {
          Console.WriteLine("Monitoring error: {0}", e);
        }
      } finally {
        Console.WriteLine("Stop receiving real-time events");
      }
    }

    public void StopMonitoring(uint deviceID) {
      var disableRequest = new DisableMonitoringRequest{ DeviceID = deviceID };
      eventClient.DisableMonitoring(disableRequest);

      if(cancellationTokenSource != null) {
        cancellationTokenSource.Cancel();
      }
    }

    public void StopMonitoringMulti(uint[] deviceIDs) {
      var disableRequest = new DisableMonitoringMultiRequest{};
      disableRequest.DeviceIDs.AddRange(deviceIDs);
      eventClient.DisableMonitoringMulti(disableRequest);

      if (cancellationTokenSource != null) {
        cancellationTokenSource.Cancel();
      }
    }

    public void DisableMonitoring(uint deviceID) {
      var disableRequest = new DisableMonitoringRequest{ DeviceID = deviceID };
      eventClient.DisableMonitoring(disableRequest);
    }

    public void StopMonitoring() {
      if(cancellationTokenSource != null) {
        cancellationTokenSource.Cancel();
      }
    }

    public void InitCodeMap(string filename) {
      var jsonData = File.ReadAllText(filename);
      codeMap = JsonSerializer.Deserialize<EventCodeMap>(jsonData);
    }

    public string GetEventString(uint eventCode, uint subCode) {
      if(codeMap == null) {
        return string.Format("No code map(0x{0:X})", eventCode | subCode);
      }

      for(int i = 0; i < codeMap.entries.Count; i++) {
        if(eventCode == codeMap.entries[i].event_code && subCode == codeMap.entries[i].sub_code) {
          return codeMap.entries[i].desc;
        }
      }

      return string.Format("Unknown event(0x{0:X})", eventCode | subCode);
    }
  }
}