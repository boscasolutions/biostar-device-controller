using System;
using Gsdk.Event;

namespace example
{
	class LogTest
	{
    private const int FIRST_APB_EVENT = 0x6000; // BS2_EVENT_ZONE_APB_VIOLATION
    private const int LAST_APB_EVENT = 0x6200; // BS2_EVENT_ZONE_APB_ALARM_CLEAR

		private EventSvc eventSvc;

		public LogTest(EventSvc svc) {
			eventSvc = svc;
		}

    public void EventCallback(EventLog eventLog) {
      if(eventLog.EventCode >= FIRST_APB_EVENT && eventLog.EventCode <= LAST_APB_EVENT) {
        Console.WriteLine("{0}: APB Zone {1}, User {2}, {3}", DateTimeOffset.FromUnixTimeSeconds(eventLog.Timestamp).UtcDateTime, eventLog.EntityID,  eventLog.UserID, eventSvc.GetEventString(eventLog.EventCode, eventLog.SubCode));        
      } else {
        Console.WriteLine("{0}: Device {1}, User {2}, {3}", DateTimeOffset.FromUnixTimeSeconds(eventLog.Timestamp).UtcDateTime, eventLog.DeviceID, eventLog.UserID, eventSvc.GetEventString(eventLog.EventCode, eventLog.SubCode));        
      }
    }
	}
}

