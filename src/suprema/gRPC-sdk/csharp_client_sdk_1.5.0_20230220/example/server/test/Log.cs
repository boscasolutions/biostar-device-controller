using System;
using Gsdk.Event;

namespace example
{
	class LogTest
	{
		private EventSvc eventSvc;

		public LogTest(EventSvc svc) {
			eventSvc = svc;
		}

    public void EventCallback(EventLog eventLog) {
      Console.WriteLine("{0}: Device {1}, User {2}, {3}", DateTimeOffset.FromUnixTimeSeconds(eventLog.Timestamp).UtcDateTime, eventLog.DeviceID, eventLog.UserID, eventSvc.GetEventString(eventLog.EventCode, eventLog.SubCode));        
    }
	}
}

