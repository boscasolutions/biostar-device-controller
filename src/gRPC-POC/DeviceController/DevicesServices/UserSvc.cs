using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.User;

namespace example
{
    public class UserSvc
    {
        private User.UserClient userClient;

        public UserSvc(Channel channel)
        {
            userClient = new User.UserClient(channel);
        }

        public RepeatedField<UserHdr> GetList(uint deviceID)
        {
            var request = new GetListRequest { DeviceID = deviceID };

            try
            {
                var response = userClient.GetList(request);

                return response.Hdrs;
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot get the user list {0}: {1}", deviceID, e);
                throw;
            }
        }

        public async Task<RepeatedField<UserInfo>> GetUserAsync(uint deviceID, string[] userIDs)
        {
            var request = new GetRequest { DeviceID = deviceID };
            request.UserIDs.AddRange(userIDs);

            try
            {
                var response = await userClient.GetAsync(request);

                return response.Users;
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot get the user information {0}: {1}", deviceID, e);
                throw;
            }
        }

        public async Task EnrollAsync(uint deviceID, UserInfo[] users)
        {
            var request = new EnrollRequest { DeviceID = deviceID };
            request.Users.AddRange(users);
            request.Overwrite = true;

            try
            {
                await userClient.EnrollAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot enroll users {0}: {1}", deviceID, e);
                throw;
            }
        }

        public async Task EnrollMultiAsync(uint[] deviceIDs, UserInfo[] users)
        {
            var request = new EnrollMultiRequest { };
            request.DeviceIDs.AddRange(deviceIDs);
            request.Users.AddRange(users);

            try
            {
                await userClient.EnrollMultiAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Enroll multi error: {0}", e);
                throw;
            }
        }

        public async Task DeleteAsync(uint deviceID, string[] userIDs)
        {
            var request = new DeleteRequest { DeviceID = deviceID };
            request.UserIDs.AddRange(userIDs);

            try
            {
                await userClient.DeleteAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot delete users {0}: {1}", deviceID, e);
                throw;
            }
        }

        public async Task DeleteMultiAsync(uint[] deviceIDs, string[] userIDs)
        {
            var request = new DeleteMultiRequest { };
            request.DeviceIDs.AddRange(deviceIDs);
            request.UserIDs.AddRange(userIDs);

            try
            {
                await userClient.DeleteMultiAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Delete multi error: {0}", e);
                throw;
            }
        }

        public async Task SetCardAsync(uint deviceID, UserCard[] userCards)
        {
            var request = new SetCardRequest { DeviceID = deviceID };
            request.UserCards.AddRange(userCards);

            try
            {
                await userClient.SetCardAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot set cards {0}: {1}", deviceID, e);
                throw;
            }
        }

        public async Task SetFingerAsync(uint deviceID, UserFinger[] userFingers)
        {
            var request = new SetFingerRequest { DeviceID = deviceID };
            request.UserFingers.AddRange(userFingers);

            try
            {
                await userClient.SetFingerAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot set fingers {0}: {1}", deviceID, e);
                throw;
            }
        }

        public async Task SetFaceAsync(uint deviceID, UserFace[] userFaces)
        {
            var request = new SetFaceRequest { DeviceID = deviceID };
            request.UserFaces.AddRange(userFaces);

            try
            {
                await userClient.SetFaceAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot set faces {0}: {1}", deviceID, e);
                throw;
            }
        }

        public async Task SetAccessGroupAsync(uint deviceID, UserAccessGroup[] groups)
        {
            var request = new SetAccessGroupRequest { DeviceID = deviceID };
            request.UserAccessGroups.AddRange(groups);

            try
            {
                await userClient.SetAccessGroupAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot set access groups {0}: {1}", deviceID, e);
                throw;
            }
        }

        public async Task<RepeatedField<UserAccessGroup>> GetAccessGroupAsync(uint deviceID, string[] userIDs)
        {
            var request = new GetAccessGroupRequest { DeviceID = deviceID };
            request.UserIDs.AddRange(userIDs);

            try
            {
                var response = await userClient.GetAccessGroupAsync(request);
                return response.UserAccessGroups;
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot get access groups {0}: {1}", deviceID, e);
                throw;
            }
        }
    }
}