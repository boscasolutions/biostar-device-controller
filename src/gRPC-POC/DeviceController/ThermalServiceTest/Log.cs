using Gsdk.Event;
using Gsdk.Thermal;
using System;

namespace example
{
    class LogTest
    {
        private ThermalSvc thermalSvc;
        private EventSvc eventSvc;
        private uint firstEventID;

        public LogTest(ThermalSvc thermalSvc, EventSvc eventSvc)
        {
            this.thermalSvc = thermalSvc;
            this.eventSvc = eventSvc;
            firstEventID = 0;
        }

        public void Test(uint deviceID)
        {
            Console.WriteLine(Environment.NewLine + "===== Log Events with Temperature =====" + Environment.NewLine);

            var events = thermalSvc.GetTemperatureLog(deviceID, firstEventID, 0);

            for (int i = 0; i < events.Count; i++)
            {
                PrintEvent(events[i]);
            }
        }

        private void PrintEvent(TemperatureLog logEvent)
        {
            try
            {
                var userID = UInt32.Parse(logEvent.UserID);

                if (userID == 0xffffffff)
                { // no user iD
                    Console.WriteLine("{0}: Device {1}, {2}, Temperature {3}", DateTimeOffset.FromUnixTimeSeconds(logEvent.Timestamp).UtcDateTime, logEvent.DeviceID, eventSvc.GetEventString(logEvent.EventCode, logEvent.SubCode), logEvent.Temperature);
                }
                else
                {
                    Console.WriteLine("{0}: Device {1}, User {2}, {3}, Temperature {4}", DateTimeOffset.FromUnixTimeSeconds(logEvent.Timestamp).UtcDateTime, logEvent.DeviceID, logEvent.UserID, eventSvc.GetEventString(logEvent.EventCode, logEvent.SubCode), logEvent.Temperature);
                }
            }
            catch
            { // invalid user ID
                Console.WriteLine("{0}: Device {1}, {2}, Temperature {3}", DateTimeOffset.FromUnixTimeSeconds(logEvent.Timestamp).UtcDateTime, logEvent.DeviceID, eventSvc.GetEventString(logEvent.EventCode, logEvent.SubCode), logEvent.Temperature);
            }

        }

        public void EventCallback(EventLog logEvent)
        {
            if (firstEventID == 0)
            {
                firstEventID = logEvent.ID;
            }

            Console.WriteLine(Environment.NewLine + "Realtime Event: {0}", logEvent);
        }
    }
}

