using Gsdk.Event;
using System;

namespace example
{
    class LogTest
    {
        private EventSvc eventSvc;
        private uint firstEventID;

        public LogTest(EventSvc svc)
        {
            eventSvc = svc;
            firstEventID = 0;
        }

        public void PrintUserLog(uint deviceID, string userID)
        {
            Console.WriteLine(Environment.NewLine + "===== Log Events for User {0} =====" + Environment.NewLine, userID);

            var filter = new EventFilter { UserID = userID };
            var events = eventSvc.GetLogWithFilter(deviceID, firstEventID, 0, filter);

            for (int i = 0; i < events.Count; i++)
            {
                PrintEvent(events[i]);
            }

            Console.WriteLine(Environment.NewLine + "===== Verify Success Events of User {0} =====" + Environment.NewLine, userID);

            filter.EventCode = 0x1000; // BS2_EVENT_VERIFY_SUCCESS

            events = eventSvc.GetLogWithFilter(deviceID, firstEventID, 0, filter);

            for (int i = 0; i < events.Count; i++)
            {
                PrintEvent(events[i]);
            }
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

