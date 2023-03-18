using System;
using Gsdk.User;
using Gsdk.Connect;
using Gsdk.Event;
using Gsdk.Device;
using Gsdk.Auth;
using Gsdk.Card;
using Gsdk.Finger;
using Gsdk.Face;
using Grpc.Core;

namespace example
{
  class UserTest
  {
    private const string GATEWAY_CA_FILE = "../../../../cert/gateway/ca.crt";
    private const string GATEWAY_ADDR = "192.168.8.98";
    private const int GATEWAY_PORT = 4000;

    private const string DEVICE_ADDR = "192.168.8.227";
    private const int DEVICE_PORT = 51211;
    private const bool USE_SSL = false;        

    private const string CODE_MAP_FILE = "../../event/event_code.json";

    private GatewayClient gatewayClient;
    private ConnectSvc connectSvc;
    private UserSvc userSvc;
    private EventSvc eventSvc;
    private DeviceSvc deviceSvc;
    private AuthSvc authSvc;
    private CardSvc cardSvc;
    private FingerSvc fingerSvc;
    private FaceSvc faceSvc;

    public UserTest(GatewayClient client) {
      gatewayClient = client;

      connectSvc = new ConnectSvc(gatewayClient.GetChannel());
      userSvc = new UserSvc(gatewayClient.GetChannel());
      eventSvc = new EventSvc(gatewayClient.GetChannel());
      deviceSvc = new DeviceSvc(gatewayClient.GetChannel());
      authSvc = new AuthSvc(gatewayClient.GetChannel());
      cardSvc = new CardSvc(gatewayClient.GetChannel());
      fingerSvc = new FingerSvc(gatewayClient.GetChannel());
      faceSvc = new FaceSvc(gatewayClient.GetChannel());
    }

    public static void Main(string[] args)
    {
      GatewayClient gatewayClient = null;
      UserTest userTest = null;
      uint devID = 0;

      try {
        gatewayClient = new GatewayClient();
        gatewayClient.Connect(GATEWAY_CA_FILE, GATEWAY_ADDR, GATEWAY_PORT);

        userTest = new UserTest(gatewayClient);

        var connectInfo = new ConnectInfo{ IPAddr = DEVICE_ADDR, Port = DEVICE_PORT, UseSSL = USE_SSL };
        devID = userTest.connectSvc.Connect(connectInfo);
      } catch (RpcException e) {
				Console.WriteLine("Cannot connect to the device: {0}", e);
        gatewayClient.Close();
        Environment.Exit(1);
			}

      uint[] devIDs = { devID };
      DeviceCapability capability = null;

      try {
        capability = userTest.deviceSvc.GetCapability(devID);

        Console.WriteLine("Device Capability : {0}" + Environment.NewLine, capability);

      } catch (RpcException e) {
				Console.WriteLine("Cannot get the device info: {0}", e);
        userTest.connectSvc.Disconnect(devIDs);
        gatewayClient.Close();
        Environment.Exit(1);
      }

      try {
        AuthTest authTest = new AuthTest(userTest.authSvc);
        AuthConfig origAuthConfig = authTest.PrepareAuthConfig(devID);

        LogTest logTest = new LogTest(userTest.eventSvc);

        userTest.eventSvc.InitCodeMap(CODE_MAP_FILE);
        userTest.eventSvc.StartMonitoring(devID);
        userTest.eventSvc.SetCallback(logTest.EventCallback);

        var newUserID = userTest.EnrollUser(devID, capability.ExtendedAuthSupported);

        if (capability.CardInputSupported) {
          new CardTest(userTest.cardSvc, userTest.userSvc).Test(devID, newUserID);
        } else {
          Console.WriteLine("!! The device {0} does not support cards. Skip the card test.", devID);
        } 

        if (capability.FingerprintInputSupported) {
          new FingerTest(userTest.fingerSvc, userTest.userSvc).Test(devID, newUserID);
        } else {
          Console.WriteLine("!! The device {0} does not support fingerprints. Skip the fingerprint test.", devID);
        }       

        if (capability.FaceInputSupported) {
          new FaceTest(userTest.faceSvc, userTest.userSvc).Test(devID, newUserID);
        } else {
          Console.WriteLine("!! The device {0} does not support faces. Skip the face test.", devID);
        }           

        authTest.Test(devID, capability.ExtendedAuthSupported);

        userTest.eventSvc.StopMonitoring(devID);

        logTest.PrintUserLog(devID, newUserID);

        userTest.userSvc.Delete(devID, new string[]{ newUserID });

        userTest.authSvc.SetConfig(devID, origAuthConfig);
      } catch (RpcException e) {
				Console.WriteLine("Cannot complete the user test for device {0}: {1}", devID, e);
      } finally {
        userTest.connectSvc.Disconnect(devIDs);
        gatewayClient.Close();
      }
    }

    public string EnrollUser(uint deviceID, bool extendedAuthSupported) {
      var userList = userSvc.GetList(deviceID);

      Console.WriteLine(Environment.NewLine + "Existing User list: {0}" + Environment.NewLine, userList);

      string newUserID = string.Format("{0}", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
      UserInfo newUser = new UserInfo{ Hdr = new UserHdr{ ID = newUserID } };

      if (extendedAuthSupported) {
        newUser.Setting = new UserSetting{ CardAuthExtMode = (uint)AuthMode.AuthExtModeCardOnly, FingerAuthExtMode = (uint)AuthMode.AuthExtModeFingerprintOnly, FaceAuthExtMode = (uint)AuthMode.AuthExtModeFaceOnly };
      } else {
        newUser.Setting = new UserSetting{ CardAuthMode = (uint)AuthMode.CardOnly, BiometricAuthMode = (uint)AuthMode.BiometricOnly };
      }

      userSvc.Enroll(deviceID, new UserInfo[]{ newUser });

      var newUsers = userSvc.GetUser(deviceID, new string[]{ newUserID });
      Console.WriteLine(Environment.NewLine + "Test User: {0}" + Environment.NewLine, newUsers[0]);

      return newUserID;
    }
  }
}
