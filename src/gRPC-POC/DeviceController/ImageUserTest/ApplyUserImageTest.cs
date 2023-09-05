using Google.Protobuf;
using Google.Protobuf.Collections;
using Gsdk.Face;
using Gsdk.User;
using System;
using System.IO;
using System.Threading.Tasks;

namespace example
{
    class ApplyUserImageTest
    {
        private FaceSvc faceSvc;
        private UserSvc userSvc;

        private const string FACE_WARPED_IMAGE = "warped.jpg";
        private const string USER_PROFILE_IMAGE = "./profile.jpg";

        public ApplyUserImageTest(FaceSvc fSvc, UserSvc uSvc)
        {
            faceSvc = fSvc;
            userSvc = uSvc;
        }

        public byte[] setImageData(string fileName, byte[] imageData)
        {
            // if(!File.Exists(fileName)) File.Create(fileName);

            File.WriteAllBytes(fileName, imageData);
            
            return imageData;
        }

        public byte[] getImageData(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        public async Task<ByteString> NormalizeImageAsync(uint deviceID, byte[] imageData)
        {
            Console.WriteLine("Normalize from device: {0}", deviceID);

            // int imageSize = Buffer.ByteLength(imageData);
            ByteString rawImageData = ByteString.CopyFrom(imageData);

            ByteString normalizedImageData = await faceSvc.NormalizeAsync(deviceID, rawImageData);

            Console.WriteLine("Normalize completed");

            const bool isWriteImage = true;
            if (isWriteImage)
            {
                byte[] data = normalizedImageData.ToByteArray();
                string warpedFileName = USER_PROFILE_IMAGE;
                setImageData(warpedFileName, data);
            };

            return normalizedImageData;
        }

        public async Task<ByteString> EnrollFaceUserAsync(uint deviceID, ByteString normalizedImageData, byte[] profileImageBytes)
        {
            Console.WriteLine("Enroll user");
          
            ByteString profileImage = ByteString.CopyFrom(profileImageBytes);

            string userID10 = "10";
            UserInfo userInfo10 = new UserInfo { Hdr = new UserHdr { ID = userID10 } };
            userInfo10.Name = "testProfileUser";
            userInfo10.Setting = new UserSetting { StartTime = 978307200, EndTime = 1924991999 };
            userInfo10.Photo = profileImage;

            FaceData faceData10 = new FaceData();
            faceData10.Flag = (uint)FaceFlag.Bs2FaceFlagEx | (uint)FaceFlag.Bs2FaceFlagWarped;
            faceData10.ImageData = normalizedImageData;
            userInfo10.Faces.Add(faceData10);

            await userSvc.EnrollAsync(deviceID, new UserInfo[] { userInfo10 });

            Console.WriteLine("Enroll user finished");

            return normalizedImageData;
        }

        public async Task<string[]> GetFaceUserListAsync(uint deviceID)
        {
            Console.WriteLine("Get user list");

            RepeatedField<UserHdr> userList = await userSvc.GetListAsync(deviceID);

            string[] userIDs = new string[userList.Count];
            for (int idx = 0; idx < userList.Count; idx++)
            {
                Console.WriteLine("User:{0}", userList[idx].ID);
                userIDs[idx] = userList[idx].ID;
            }

            return userIDs;
        }

        public async Task<string[]> GetFaceUsersAsync(uint deviceID, string[] userIDs)
        {
            Console.WriteLine("Get users");

            RepeatedField<UserInfo> userInfo = await userSvc.GetUserAsync(deviceID, userIDs);

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
                    setImageData(userDeviceImage, data);
                }
            }

            return userIDs;
        }
    }
}