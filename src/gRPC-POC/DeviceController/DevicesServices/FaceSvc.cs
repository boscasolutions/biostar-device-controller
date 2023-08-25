using Google.Protobuf;
using Grpc.Core;
using Gsdk.Face;

namespace example
{
    public class FaceSvc
    {
        private Face.FaceClient faceClient;

        public FaceSvc(Channel channel)
        {
            faceClient = new Face.FaceClient(channel);
        }

        public async Task<FaceData> ScanAsync(uint deviceID, FaceEnrollThreshold enrollThreshold)
        {
            var request = new ScanRequest { DeviceID = deviceID, EnrollThreshold = enrollThreshold };
            var response = await faceClient.ScanAsync(request);

            return response.FaceData;
        }

        public async Task<ByteString> NormalizeAsync(uint deviceID, ByteString unwrappedImageData)
        {
            var request = new NormalizeRequest { DeviceID = deviceID, UnwrappedImageData = unwrappedImageData };
            var response = await faceClient.NormalizeAsync(request);

            return response.WrappedImageData;
        }

        public async Task<FaceConfig> GetConfigAsync(uint deviceID)
        {
            var request = new GetConfigRequest { DeviceID = deviceID };
            var response = await faceClient.GetConfigAsync(request);

            return response.Config;
        }

        public async Task SetConfigAsync(uint deviceID, FaceConfig config)
        {
            var request = new SetConfigRequest { DeviceID = deviceID, Config = config };
            _ = await faceClient.SetConfigAsync(request);
        }
    }
}