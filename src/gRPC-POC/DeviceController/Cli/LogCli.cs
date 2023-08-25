using example;
using Gsdk.Event;
using System;

namespace example
{
    public class LogCli
    {
        private const int MAX_NUM_EVENT = 32;
        private const string CODE_MAP_FILE = "./event_code.json";

        private EventSvc eventSvc;
        private uint firstEventID;

        public LogCli(EventSvc svc)
        {
            eventSvc = svc;
            firstEventID = 0;
        }

        public async Task EventsLogggerAsync(uint deviceID)
        {
            eventSvc.InitCodeMap(CODE_MAP_FILE);
            await eventSvc.StartMonitoringAsync(deviceID);
            await eventSvc.SetCallback(EventCallback);

            Console.WriteLine(Environment.NewLine + "===== Event Stream =====" + Environment.NewLine);

            KeyInput.PressEnter(">> Try to authenticate credentials to check real-time monitoring. And, press ENTER to read the generated event logs." + Environment.NewLine);

            if (firstEventID == 0)
            {
                Console.WriteLine(Environment.NewLine + ">> There is no new event. Just read {0} event logs from the start.", MAX_NUM_EVENT);
            }
            else
            {
                Console.WriteLine(Environment.NewLine + ">> Read new events starting from {0}", firstEventID);
            }

            var events = eventSvc.GetLog(deviceID, firstEventID, MAX_NUM_EVENT);

            for (int i = 0; i < events.Count; i++)
            {
                PrintEvent(events[i]);
            }

            if (events.Count > 0 && firstEventID != 0)
            {
                var filter = new EventFilter { EventCode = events[0].EventCode };
                Console.WriteLine(Environment.NewLine + ">> Filter with event code {0}", filter.EventCode);

                events = eventSvc.GetLogWithFilter(deviceID, firstEventID, MAX_NUM_EVENT, filter);
                for (int i = 0; i < events.Count; i++)
                {
                    PrintEvent(events[i]);
                }
            }

            await eventSvc.StopMonitoringAsync(deviceID);
        }

        private void PrintEvent(EventLog logEvent)
        {
            Console.WriteLine("{0}: Device {1}, User {2}, {3}", DateTimeOffset.FromUnixTimeSeconds(logEvent.Timestamp).UtcDateTime, logEvent.DeviceID, logEvent.UserID, eventSvc.GetEventString(logEvent.EventCode, logEvent.SubCode));
        }

        public void EventCallback(EventLog logEvent)
        {
            if (firstEventID == 0)
            {
                firstEventID = logEvent.ID;
            }

            Console.WriteLine(Environment.NewLine + " Realtime Event: {0}", logEvent);
        }
    }
}

