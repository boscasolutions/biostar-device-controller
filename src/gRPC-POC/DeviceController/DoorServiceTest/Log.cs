using Gsdk.Event;
using System;

namespace example
{
    class LogTest
    {
        private const int FIRST_DOOR_EVENT = 0x5000; // BS2_EVENT_DOOR_UNLOCKED
        private const int LAST_DOOR_EVENT = 0x5E00; // BS2_EVENT_DOOR_UNLOCK

        private const int MAX_NUM_OF_LOG = 16;

        private EventSvc eventSvc;
        private uint firstEventID;

        public LogTest(EventSvc svc)
        {
            eventSvc = svc;
            firstEventID = 0;
        }

        public void EventCallback(EventLog eventLog)
        {
            if (firstEventID == 0)
            {
                firstEventID = eventLog.ID;
            }

            if (eventLog.EventCode >= FIRST_DOOR_EVENT && eventLog.EventCode <= LAST_DOOR_EVENT)
            {
                Console.WriteLine("{0}: Door {1}, {2}", DateTimeOffset.FromUnixTimeSeconds(eventLog.Timestamp).UtcDateTime, eventLog.EntityID, eventSvc.GetEventString(eventLog.EventCode, eventLog.SubCode));
            }
            else
            {
                Console.WriteLine("{0}: Device {1}, User {2}, {3}", DateTimeOffset.FromUnixTimeSeconds(eventLog.Timestamp).UtcDateTime, eventLog.DeviceID, eventLog.UserID, eventSvc.GetEventString(eventLog.EventCode, eventLog.SubCode));
            }
        }

        public string GetUserID(uint deviceID)
        {
            var events = eventSvc.GetLog(deviceID, firstEventID, MAX_NUM_OF_LOG);

            foreach (EventLog eventLog in events)
            {
                if (eventLog.EventCode == 0x1900)
                { // BS2_EVENT_ACCESS_DENIED
                    return eventLog.UserID;
                }
            }

            return null;
        }
    }
}

