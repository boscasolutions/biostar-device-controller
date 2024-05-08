using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core.Logging;
using Gsdk.Face;
using Gsdk.User;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace example
{
    class ApplyUserImageTest
    {
        private FaceSvc faceSvc;
        private UserSvc userSvc;

        public ApplyUserImageTest(FaceSvc fSvc, UserSvc uSvc)
        {
            faceSvc = fSvc;
            userSvc = uSvc;
        }

        public byte[] setImageData(string fileName, byte[] imageData)
        {
            File.WriteAllBytes(fileName, imageData);
            
            return imageData;
        }

        public byte[] getImageData(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        public async Task<ByteString> NormalizeImageAsync(uint deviceID, byte[] imageData, string profileImageFileName, bool createProfileImage)
        {
            Console.WriteLine("Normalize from device: {0}", deviceID);

            ByteString rawImageData = ByteString.CopyFrom(imageData);

            ByteString normalizedImageData = await faceSvc.NormalizeAsync(deviceID, rawImageData);

            Console.WriteLine("Normalize completed");

            // System.Drawing.Image

            if (createProfileImage)
            {
                byte[] data = normalizedImageData.ToByteArray();
                string warpedFileName = profileImageFileName;
                setImageData(warpedFileName, data);
            };

            return normalizedImageData;
        }

        public async Task<ByteString> EnrollFaceUserAsync(uint deviceID, ByteString normalizedImageData)
        {
            Console.WriteLine("Enroll user");
            
            string userID10 = "10";
            UserInfo userInfo10 = new UserInfo { Hdr = new UserHdr { ID = userID10 } };
            userInfo10.Name = "testProfileUser";
            userInfo10.Setting = new UserSetting { StartTime = ToDeviceDateTime(DateTime.UtcNow), EndTime = ToDeviceDateTime(DateTime.UtcNow.AddYears(3))};

            // The maximum size is 16KB
            if (normalizedImageData != null)
            {
                if (normalizedImageData.Length > (16 * 1024))
                {
                    Console.WriteLine($"Image is bigger then the maximum size of 16KB allowd for a avatar image size: {normalizedImageData.Length}");

                    userInfo10.Photo = ByteString.CopyFrom(resizeImageData(normalizedImageData.ToByteArray()));

                    Console.WriteLine($"resizedData size: {userInfo10.Photo.Length}");
                }
                else
                {
                    Console.WriteLine($"Image size: {normalizedImageData.Length}");

                    userInfo10.Photo = normalizedImageData;
                }
            }

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

        public async Task<string[]> ShowFaceUsersListAsync(uint deviceID, string[] userIDs)
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

        public UInt32 ToDeviceDateTime(DateTime dateTime)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan currTime = dateTime - startTime;

            return Convert.ToUInt32(Math.Abs(currTime.TotalSeconds));
        }

        private byte[] resizeImageData(byte[] imageByteData) 
        {
            byte[] result;
            // calculate percentage reduction:
            int requierdByteSize = (16 * 1024);
            float imageSize = imageByteData.Length;
            float percentage = (((float)requierdByteSize / (float)imageSize));

            using (Image image = Image.Load(imageByteData))
            {
                // int width = (int)(image.Width * percentage);
                // int height = (int)(image.Height * percentage);

                image.Mutate(x => x.Resize(image.Width, image.Height));
                
                MemoryStream stream = new MemoryStream();
                image.Save(stream, new JpegEncoder { Quality = 100});
                result = stream.ToArray();
            }
            return result;
        }

        //private static System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, Size size)
        //{
        //    // Get the image current width
        //    int sourceWidth = imgToResize.Width;
        //    // Get the image current height
        //    int sourceHeight = imgToResize.Height;
        //    float nPercent = 0;
        //    float nPercentW = 0;
        //    float nPercentH = 0;
        //    // Calculate width and height with new desired size
        //    nPercentW = ((float)size.Width / (float)sourceWidth);
        //    nPercentH = ((float)size.Height / (float)sourceHeight);
        //    nPercent = Math.Min(nPercentW, nPercentH);
        //    // New Width and Height
        //    int destWidth = (int)(sourceWidth * nPercent);
        //    int destHeight = (int)(sourceHeight * nPercent);
        //    Bitmap b = new Bitmap(destWidth, destHeight);
        //    Graphics g = Graphics.FromImage((System.Drawing.Image)b);
        //    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    // Draw image with new width and height
        //    g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
        //    g.Dispose();
        //    return (System.Drawing.Image)b;
        //}
    }
}