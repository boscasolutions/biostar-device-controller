using Google.Protobuf.Collections;
using Gsdk.Event;
using System;
using System.Threading.Tasks;

namespace example
{
    class EventMgr
    {
        private EventSvc eventSvc;
        private TestConfig testConfig;

        public const int MAX_NUM_OF_LOG = 16384;

        public EventMgr(EventSvc svc, TestConfig config)
        {
            eventSvc = svc;
            testConfig = config;
        }

        public async Task HandleEventAsync(EventCallback callback)
        {
            await eventSvc.SetCallback(callback);
            await eventSvc.StartMonitoringAsync();
        }

        public void StopHandleEvent()
        {
            eventSvc.StopMonitoring();
        }

        public void PrintEvent(EventLog eventLog)
        {
            Console.WriteLine("[EVENT] {0}: Device {1}, User {2}, {3}", DateTimeOffset.FromUnixTimeSeconds(eventLog.Timestamp).UtcDateTime, eventLog.DeviceID, eventLog.UserID, eventSvc.GetEventString(eventLog.EventCode, eventLog.SubCode));
        }

        public RepeatedField<EventLog> ReadNewLog(TestDeviceInfo devInfo, uint maxNumOfLog)
        {
            var eventLogs = eventSvc.GetLog(devInfo.device_id, devInfo.last_event_id + 1, maxNumOfLog);

            if (eventLogs.Count > 0)
            {
                testConfig.UpdateLastEventID(devInfo.device_id, eventLogs[eventLogs.Count - 1].ID);
            }

            return eventLogs;
        }

        public void HandleConnection(uint deviceID)
        {
            Console.WriteLine("***** Device {0} is connected", deviceID);

            var dev = testConfig.GetDeviceInfo(deviceID);

            if (dev == null)
            {
                Console.WriteLine("!!! Device {0} is not in the configuration file", deviceID);
                return;
            }

            var eventLogs = new RepeatedField<EventLog>();

            // read new logs
            while (true)
            {
                Console.WriteLine("[{0}] Reading log records starting from {1}...", deviceID, dev.last_event_id);

                var newLogs = ReadNewLog(dev, MAX_NUM_OF_LOG);

                Console.WriteLine("[{0}] Read {1} events", deviceID, newLogs.Count);

                eventLogs.Add(newLogs);

                if (newLogs.Count < MAX_NUM_OF_LOG)
                { // read the last log
                    break;
                }
            }

            // do something with the event logs
            // ...
            Console.WriteLine("[{0}] The total number of new events: {1}", deviceID, eventLogs.Count);

            // enable real-time monitoring
            eventSvc.EnableMonitoring(deviceID);
        }
    }
}