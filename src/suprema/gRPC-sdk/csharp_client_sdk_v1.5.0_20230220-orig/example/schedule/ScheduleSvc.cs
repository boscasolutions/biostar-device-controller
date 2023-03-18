using Gsdk.Schedule;
using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System;

namespace example
{
  class ScheduleSvc
  {
    private Schedule.ScheduleClient scheduleClient;

    public ScheduleSvc(Channel channel) {
      scheduleClient = new Schedule.ScheduleClient(channel);
    }

    public RepeatedField<ScheduleInfo> GetList(uint deviceID) {
      var request = new GetListRequest{ DeviceID = deviceID };
      var response = scheduleClient.GetList(request);

      return response.Schedules;
    }

    public void Add(uint deviceID, ScheduleInfo[] schedules) {
      var request = new AddRequest{ DeviceID = deviceID };
      request.Schedules.AddRange(schedules);
      scheduleClient.Add(request);
    }

    public void DeleteAll(uint deviceID) {
      var request = new DeleteAllRequest{ DeviceID = deviceID };
      scheduleClient.DeleteAll(request);
    }

    public RepeatedField<HolidayGroup> GetHolidayList(uint deviceID) {
      var request = new GetHolidayListRequest{ DeviceID = deviceID };
      var response = scheduleClient.GetHolidayList(request);

      return response.Groups;
    }

    public void AddHoliday(uint deviceID, HolidayGroup[] groups) {
      var request = new AddHolidayRequest{ DeviceID = deviceID };
      request.Groups.AddRange(groups);
      scheduleClient.AddHoliday(request);
    }

    public void DeleteAllHoliday(uint deviceID) {
      var request = new DeleteAllHolidayRequest{ DeviceID = deviceID };
      scheduleClient.DeleteAllHoliday(request);
    }
  }
}