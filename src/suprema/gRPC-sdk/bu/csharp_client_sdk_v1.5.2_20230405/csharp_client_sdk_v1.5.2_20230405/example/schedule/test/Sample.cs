using System;
using Gsdk.Schedule;
using Google.Protobuf;

namespace example
{
	class SampleTest
	{
    private const uint SAMPLE_HOLIDAY_GROUP_ID = 1;
    private const uint WEEKLY_SCHEDULE_ID = 2; // 0 and 1 are reserved
    private const uint DAILY_SCHEDULE_ID = WEEKLY_SCHEDULE_ID + 1;
    private const int NUM_OF_DAY = 30;

		private ScheduleSvc scheduleSvc;

		public SampleTest(ScheduleSvc svc) {
			scheduleSvc = svc;
		}

		public void Test(uint deviceID) {    
      // Backup the original schedules
      var origSchedules = scheduleSvc.GetList(deviceID);
      var origHolidayGroups = scheduleSvc.GetHolidayList(deviceID);

      Console.WriteLine(Environment.NewLine + "Original Schedules: {0}" + Environment.NewLine, origSchedules);    
			Console.WriteLine(Environment.NewLine + "Original Holiday Groups: {0}" + Environment.NewLine, origHolidayGroups); 

      // Test sample schedules     
      Console.WriteLine(Environment.NewLine + "===== Test Sample Schedules =====" + Environment.NewLine);    
      scheduleSvc.DeleteAll(deviceID);
      scheduleSvc.DeleteAllHoliday(deviceID);

      HolidaySchedule holidaySchedule = MakeHolidayGroup(deviceID);
      MakeWeeklySchedule(deviceID, holidaySchedule);
      MakeDailySchedule(deviceID);

      var newSchedules = scheduleSvc.GetList(deviceID);
      Console.WriteLine(">>> Sample Schedules: {0}" + Environment.NewLine, newSchedules);    

      var newGroups = scheduleSvc.GetHolidayList(deviceID);
      Console.WriteLine(">>> Sample Holiday Groups: {0}" + Environment.NewLine, newGroups);    

      // Restore the original schedules
      scheduleSvc.DeleteAll(deviceID);
      scheduleSvc.DeleteAllHoliday(deviceID);

      var scheduleArray = new ScheduleInfo[origSchedules.Count];
      origSchedules.CopyTo(scheduleArray, 0);
      scheduleSvc.Add(deviceID, scheduleArray); 

      var groupArray = new HolidayGroup[origHolidayGroups.Count];
      origHolidayGroups.CopyTo(groupArray, 0);
      scheduleSvc.AddHoliday(deviceID, groupArray); 
    }

    HolidaySchedule MakeHolidayGroup(uint deviceID) {
      var holidayGroup = new HolidayGroup{ ID = SAMPLE_HOLIDAY_GROUP_ID, Name = "Sample Holiday Group" };
      holidayGroup.Holidays.Add(new Holiday{ Date = 0, Recurrence = HolidayRecurrence.RecurYearly }); // Jan .1
      holidayGroup.Holidays.Add(new Holiday{ Date = 358, Recurrence = HolidayRecurrence.RecurYearly }); //  Dec. 25 in non leap year, 359 in leap year

      scheduleSvc.AddHoliday(deviceID, new HolidayGroup[]{ holidayGroup }); 

      var daySchedule = new DaySchedule();
      daySchedule.Periods.Add(new TimePeriod{ StartTime = 600, EndTime = 720 }); // 10 am ~ 12 pm

      var holidaySchedule = new HolidaySchedule{ GroupID = SAMPLE_HOLIDAY_GROUP_ID, DaySchedule = daySchedule };

      return holidaySchedule;           
    }

    void MakeWeeklySchedule(uint deviceID, HolidaySchedule holidaySchedule) {
      var weekdaySchedule = new DaySchedule();
      weekdaySchedule.Periods.Add(new TimePeriod{ StartTime = 540, EndTime = 720 }); // 9 am ~ 12 pm
      weekdaySchedule.Periods.Add(new TimePeriod{ StartTime = 780, EndTime = 1080 }); // 1 pm ~ 6 pm

      var weekendSchedule = new DaySchedule();
      weekendSchedule.Periods.Add(new TimePeriod{ StartTime = 570, EndTime = 770 }); // 9:30 am ~ 12:30 pm
      
      var weeklySchedule = new WeeklySchedule();
      weeklySchedule.DaySchedules.Insert(0, weekendSchedule); // Sunday
      weeklySchedule.DaySchedules.Insert(1, weekdaySchedule); // Monday
      weeklySchedule.DaySchedules.Insert(2, weekdaySchedule); // Tuesday
      weeklySchedule.DaySchedules.Insert(3, weekdaySchedule); // Wednesday
      weeklySchedule.DaySchedules.Insert(4, weekdaySchedule); // Thursday
      weeklySchedule.DaySchedules.Insert(5, weekdaySchedule); // Friday
      weeklySchedule.DaySchedules.Insert(6, weekendSchedule); // Saturday

      var scheduleInfo = new ScheduleInfo{ ID = WEEKLY_SCHEDULE_ID, Name = "Sample Weekly Schedule", Weekly = weeklySchedule };
      scheduleInfo.Holidays.Add(holidaySchedule);

      scheduleSvc.Add(deviceID, new ScheduleInfo[]{ scheduleInfo });
    }

    void MakeDailySchedule(uint deviceID) {
      var daySchedule = new DaySchedule();
      daySchedule.Periods.Add(new TimePeriod{ StartTime = 540, EndTime = 720 }); // 9 am ~ 12 pm
      daySchedule.Periods.Add(new TimePeriod{ StartTime = 780, EndTime = 1080 }); // 1 pm ~ 6 pm
      
      var dailySchedule = new DailySchedule{ StartDate = (uint)(DateTime.Now.DayOfYear - 1) }; // 30 days starting from today
      for(int i = 0; i < NUM_OF_DAY; i++) {
        dailySchedule.DaySchedules.Insert(i, daySchedule);
      }

      var scheduleInfo = new ScheduleInfo{ ID = DAILY_SCHEDULE_ID, Name = "Sample Daily Schedule", Daily = dailySchedule };

      scheduleSvc.Add(deviceID, new ScheduleInfo[]{ scheduleInfo });
    }
	}
}


