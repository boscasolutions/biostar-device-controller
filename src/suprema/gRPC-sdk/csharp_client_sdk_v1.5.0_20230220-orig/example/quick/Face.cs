using System;
using System.IO;
using Gsdk.Face;

namespace example
{
	class FaceTest
	{
		private const string FACE_IMAGE_FILE = "./face.bmp";
		private FaceSvc faceSvc;

		public FaceTest(FaceSvc svc) {
			faceSvc = svc;
		}

		public void Test(uint deviceID) {
			var faceConfig = faceSvc.GetConfig(deviceID);

			Console.WriteLine("Face config: {0}" + Environment.NewLine, faceConfig);

			Console.WriteLine(">>> Scan a face...");

			var faceData = faceSvc.Scan(deviceID, faceConfig.EnrollThreshold);

			for (int i = 0; i < faceData.Templates.Count; i++) {
				Console.WriteLine("Face template[{0}]: {1}" + Environment.NewLine, i, BitConverter.ToString(faceData.Templates[i].ToByteArray()));
			}
	
			File.WriteAllBytes(FACE_IMAGE_FILE, faceData.ImageData.ToByteArray());
		}
	}
}

