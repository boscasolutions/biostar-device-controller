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

        public FaceData Scan(uint deviceID, FaceEnrollThreshold enrollThreshold)
        {
            var request = new ScanRequest { DeviceID = deviceID, EnrollThreshold = enrollThreshold };
            var response = faceClient.Scan(request);

            return response.FaceData;
        }

        public ByteString Normalize(uint deviceID, ByteString unwrappedImageData)
        {
            var request = new NormalizeRequest { DeviceID = deviceID, UnwrappedImageData = unwrappedImageData };
            var response = faceClient.Normalize(request);

            return response.WrappedImageData;
        }

        public FaceConfig GetConfig(uint deviceID)
        {
            var request = new GetConfigRequest { DeviceID = deviceID };
            var response = faceClient.GetConfig(request);

            return response.Config;
        }

        public void SetConfig(uint deviceID, FaceConfig config)
        {
            var request = new SetConfigRequest { DeviceID = deviceID, Config = config };
            var response = faceClient.SetConfig(request);
        }
    }
}