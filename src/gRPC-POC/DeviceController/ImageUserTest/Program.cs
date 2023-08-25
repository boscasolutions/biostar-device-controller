using Google.Protobuf;
using Grpc.Core;
using Gsdk.Connect;
using System;
using System.Threading.Tasks;

namespace example
{
    class FaceUserTest
    {
        private const string GATEWAY_CA_FILE = "../../../../cert/ca.crt";
        private const string GATEWAY_ADDR = "192.168.8.98";
        private const int GATEWAY_PORT = 4000;

        private const string DEVICE_ADDR = "192.168.8.227";
        private const int DEVICE_PORT = 51211;
        private const bool USE_SSL = false;

        private const string CODE_MAP_FILE = "../../event/event_code.json";

        private const string FACE_UNWARPED_IMAGE = "./unwarped.jpg";
        private const string USER_PROFILE_IMAGE = "./profile.jpg";

        private GatewayClient gatewayClient;
        private ConnectSvc connectSvc;
        private DeviceSvc deviceSvc;
        private FaceSvc faceSvc;
        private UserSvc userSvc;

        public FaceUserTest(GatewayClient client)
        {
            gatewayClient = client;

            connectSvc = new ConnectSvc(gatewayClient.GetChannel());
            deviceSvc = new DeviceSvc(gatewayClient.GetChannel());
            faceSvc = new FaceSvc(gatewayClient.GetChannel());
            userSvc = new UserSvc(gatewayClient.GetChannel());
        }

        public static async Task Main(string[] args)
        {
            GatewayClient gatewayClient = null;
            FaceUserTest faceUserTest = null;
            uint devID = 0;

            try
            {
                gatewayClient = new GatewayClient();
                gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

                faceUserTest = new FaceUserTest(gatewayClient);

                var connectInfo = new ConnectInfo { IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
                devID = await faceUserTest.connectSvc.ConnectAsync(connectInfo);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot connect to the device: {0}", e);
                gatewayClient.Close();
                Environment.Exit(1);
            }

            uint[] devIDs = { devID };

            try
            {
                TestImageUser testUser = new TestImageUser(faceUserTest.faceSvc, faceUserTest.userSvc);

                ByteString warpedImageData = await testUser.NormalizeImageAsync(devID, FACE_UNWARPED_IMAGE);

                warpedImageData = await testUser.EnrollFaceUserAsync(devID, warpedImageData, USER_PROFILE_IMAGE);
                string[] userIDs = await testUser.GetFaceUserListAsync(devID);
                userIDs = await testUser.GetFaceUsersAsync(devID, userIDs);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot complete the user image test for device {0}: {1}", devID, e);
            }
            finally
            {
                await faceUserTest.connectSvc.Disconnect(devIDs);
                gatewayClient.Close();
            }
        }
    }
}
