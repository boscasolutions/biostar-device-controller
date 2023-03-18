using Gsdk.Event;
using Gsdk.Tna;
using System;

namespace example
{
    class LogTest
    {
        private TNASvc tnaSvc;
        private EventSvc eventSvc;
        private uint firstEventID;

        public LogTest(TNASvc tnaSvc, EventSvc eventSvc)
        {
            this.tnaSvc = tnaSvc;
            this.eventSvc = eventSvc;
            firstEventID = 0;
        }

        public void Test(uint deviceID)
        {
            Console.WriteLine(Environment.NewLine + "===== T&A Log Events =====" + Environment.NewLine);

            var events = tnaSvc.GetTNALog(deviceID, firstEventID, 0);

            var config = tnaSvc.GetConfig(deviceID);

            for (int i = 0; i < events.Count; i++)
            {
                PrintEvent(events[i], config);
            }
        }

        private void PrintEvent(TNALog logEvent, TNAConfig config)
        {
            Console.WriteLine("{0}: Device {1}, User {2}, {3}, {4}", DateTimeOffset.FromUnixTimeSeconds(logEvent.Timestamp).UtcDateTime, logEvent.DeviceID, logEvent.UserID, eventSvc.GetEventString(logEvent.EventCode, logEvent.SubCode), GetTNALabel(logEvent.TNAKey, config));
        }

        private string GetTNALabel(Key key, TNAConfig config)
        {
            if (config.Labels.Count > (int)key - 1)
            {
                return string.Format("{0}(KEY{1})", config.Labels[(int)key - 1], key);
            }
            else
            {
                return string.Format("KEY{0}", key);
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

