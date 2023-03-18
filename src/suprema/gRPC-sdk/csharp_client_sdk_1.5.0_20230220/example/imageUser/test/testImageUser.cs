using System;
using System.IO;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Gsdk.Device;
using Gsdk.Operator;
using Gsdk.Face;
using Gsdk.User;

namespace example
{
	class TestImageUser
	{
		private FaceSvc faceSvc;
    private UserSvc userSvc;

    private const string FACE_WARPED_IMAGE = "warped.jpg";

		public TestImageUser(FaceSvc fSvc, UserSvc uSvc) {
			faceSvc = fSvc;
      userSvc = uSvc;
		}

    public void setImageData(string fileName, ref byte[] imagedata)
    {
      File.WriteAllBytes(fileName, imagedata);
    }

    public void getImageData(string fileName, out byte[] imageData)
    {
      imageData = File.ReadAllBytes(fileName);
    }

    public ByteString NormalizeImage(uint deviceID, string rawImageFileName)
    {
      Console.WriteLine("Normalize from {0}, with {1}", deviceID, rawImageFileName);

      byte[] imageData;
      getImageData(rawImageFileName, out imageData);

      int imageSize = Buffer.ByteLength(imageData);
      ByteString rawImageData = ByteString.CopyFrom(imageData);

      ByteString warpedImageData = faceSvc.Normalize(deviceID, rawImageData);

      Console.WriteLine("Normalize completed");

      const bool isWriteImage = true;
      if (isWriteImage)
      {
        byte[] data = warpedImageData.ToByteArray();
        string warpedFileName = FACE_WARPED_IMAGE;
        setImageData(warpedFileName, ref data);
      };

      return warpedImageData;
    }

    public void EnrollFaceUser(uint deviceID, ref ByteString warpedImageData, string profileImageFileName)
    {
      Console.WriteLine("Enroll user");

      byte[] profileBytes;
      getImageData(profileImageFileName, out profileBytes);
      ByteString profileImage = ByteString.CopyFrom(profileBytes);

      string userID10 = "10";
      UserInfo userInfo10 = new UserInfo{Hdr = new UserHdr{ID = userID10}};
      userInfo10.Name = "testProfileUser";
      userInfo10.Setting = new UserSetting{StartTime = 978307200, EndTime = 1924991999};
      userInfo10.Photo = profileImage;

      FaceData faceData = new FaceData();
      faceData.Flag = (uint)FaceFlag.Bs2FaceFlagEx | (uint)FaceFlag.Bs2FaceFlagWarped;
      faceData.ImageData = warpedImageData;
      userInfo10.Faces.Add(faceData);

      string userID11 = "11";
      UserInfo userInfo11 = new UserInfo{Hdr = new UserHdr{ID = userID11}};
      userInfo11.Name = "testNonProfileUser";
      userInfo11.Setting = new UserSetting{StartTime = 978307200, EndTime = 1924991999};
      // userInfo11.Photo = profileImage;

      FaceData faceData11 = new FaceData();
      faceData11.Flag = (uint)FaceFlag.Bs2FaceFlagEx | (uint)FaceFlag.Bs2FaceFlagWarped;
      faceData11.ImageData = warpedImageData;
      userInfo11.Faces.Add(faceData11);

      userSvc.Enroll(deviceID, new UserInfo[]{userInfo10, userInfo11});

      Console.WriteLine("Enroll user finished");
    }

    public string[] GetFaceUserList(uint deviceID)
    {
      Console.WriteLine("Get user list");

      RepeatedField<UserHdr> userList = userSvc.GetList(deviceID);

      string[] userIDs = new string[userList.Count];
      for (int idx = 0; idx < userList.Count; idx++)
      {
        Console.WriteLine("User:{0}", userList[idx].ID);
        userIDs[idx] = userList[idx].ID;
      }

      return userIDs;
    }

    public void GetFaceUsers(uint deviceID, ref string[] userIDs)
    {
      Console.WriteLine("Get users");

      RepeatedField<UserInfo> userInfo = userSvc.GetUser(deviceID, userIDs);
      foreach (UserInfo info in userInfo)
      {
        Console.WriteLine("ID : {0}", info.Hdr.ID);
        Console.WriteLine("Name : {0}", info.Name);
        Console.WriteLine("NumOfFace : {0}", info.Hdr.NumOfFace);
        Console.WriteLine("Photo : {0}", info.Photo);
        if (0 < info.Photo.Length)
        {
          byte[] data = info.Photo.ToByteArray();
          string userDeviceImage = string.Format("user_image{0}_fromDevice.jpg", info.Hdr.ID);
          setImageData(userDeviceImage, ref data);
        }
      }
    }
	}
}

