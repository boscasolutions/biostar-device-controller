using Gsdk.Door;
using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System;

namespace example
{
  class DoorSvc
  {
    private Door.DoorClient doorClient;

    public DoorSvc(Channel channel) {
      doorClient = new Door.DoorClient(channel);
    }

    public RepeatedField<DoorInfo> GetList(uint deviceID) {
      var request = new GetListRequest{ DeviceID = deviceID };
      var response = doorClient.GetList(request);

      return response.Doors;
    }

    public RepeatedField<Gsdk.Door.Status> GetStatus(uint deviceID) {
      var request = new GetStatusRequest{ DeviceID = deviceID };
      var response = doorClient.GetStatus(request);

      return response.Status;
    }


    public void Add(uint deviceID, DoorInfo[] doors) {
      var request = new AddRequest{ DeviceID = deviceID };
      request.Doors.AddRange(doors);
      doorClient.Add(request);
    }

    public void DeleteAll(uint deviceID) {
      var request = new DeleteAllRequest{ DeviceID = deviceID };
      doorClient.DeleteAll(request);
    }

    public void Lock(uint deviceID, uint[] doorIDs, DoorFlag doorFlag) {
      var request = new LockRequest{ DeviceID = deviceID, DoorFlag = (uint)doorFlag };
      request.DoorIDs.AddRange(doorIDs);
      doorClient.Lock(request);
    }

    public void Unlock(uint deviceID, uint[] doorIDs, DoorFlag doorFlag) {
      var request = new UnlockRequest{ DeviceID = deviceID, DoorFlag = (uint)doorFlag };
      request.DoorIDs.AddRange(doorIDs);
      doorClient.Unlock(request);
    }

    public void Release(uint deviceID, uint[] doorIDs, DoorFlag doorFlag) {
      var request = new ReleaseRequest{ DeviceID = deviceID, DoorFlag = (uint)doorFlag };
      request.DoorIDs.AddRange(doorIDs);
      doorClient.Release(request);
    }
  }
}