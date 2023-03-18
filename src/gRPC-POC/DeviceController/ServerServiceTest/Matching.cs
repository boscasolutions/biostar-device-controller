using Grpc.Core;
using Gsdk.Card;
using Gsdk.Finger;
using Gsdk.Server;
using Gsdk.User;
using System;
using System.Threading;

namespace example
{
    class MatchingTest
    {
        private const int QUEUE_SIZE = 16;
        private const string TEST_USER_ID = "1234";

        private ServerSvc serverSvc;
        private AuthSvc authSvc;
        private bool returnError;

        public MatchingTest(ServerSvc serverSvc, AuthSvc authSvc)
        {
            this.serverSvc = serverSvc;
            this.authSvc = authSvc;

            returnError = false;
        }

        public void Test(uint deviceID)
        {
            // Backup the original configuration
            var origAuthConfig = authSvc.GetConfig(deviceID);
            Console.WriteLine("Original Auth Config: {0}" + Environment.NewLine, origAuthConfig);

            // Enable server matching for the test
            var testConfig = origAuthConfig.Clone();
            testConfig.UseServerMatching = true;

            authSvc.SetConfig(deviceID, testConfig);

            var newConfig = authSvc.GetConfig(deviceID);
            Console.WriteLine("Test Auth Config: {0}" + Environment.NewLine, newConfig);

            TestVerify();
            TestIdentify();

            // Restore the original configuration
            authSvc.SetConfig(deviceID, origAuthConfig);
        }

        public void TestVerify()
        {
            try
            {
                var reqStream = serverSvc.Subscribe(QUEUE_SIZE);

                CancellationTokenSource cancelToken = new CancellationTokenSource();
                returnError = true;
                HandleVerify(reqStream, cancelToken.Token);

                Console.WriteLine(Environment.NewLine + "===== Server Matching: Verify Test =====" + Environment.NewLine);
                Console.WriteLine(">> Try to authenticate a card. It should fail since the device gateway will return an error code to the request.");
                KeyInput.PressEnter(">> Press ENTER for the next test." + Environment.NewLine);

                returnError = false;
                Console.WriteLine(">> Try to authenticate a card. The gateway will return SUCCESS with user information this time. The result will vary according to the authentication modes of the devices.");
                KeyInput.PressEnter(">> Press ENTER for the next test." + Environment.NewLine);

                cancelToken.Cancel();
                serverSvc.Unsubscribe();
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot subscribe to the server requests: {0}", e);
                throw;
            }
        }

        async void HandleVerify(IAsyncStreamReader<ServerRequest> reqStream, CancellationToken token)
        {
            Console.WriteLine("Start handling server requests...");

            try
            {
                while (await reqStream.MoveNext(token))
                {
                    var serverReq = reqStream.Current;

                    if (serverReq.ReqType != RequestType.VerifyRequest)
                    {
                        Console.WriteLine("!! Request type is not VERIFY_REQUEST. Just ignore it.");
                        continue;
                    }

                    if (returnError)
                    {
                        Console.WriteLine("## Gateway returns VERIFY_FAIL.");
                        serverSvc.HandleVerify(serverReq, ServerErrorCode.VerifyFail, null);
                    }
                    else
                    {
                        Console.WriteLine("## Gateway returns SUCCESS with user information.");
                        var userInfo = new UserInfo { Hdr = new UserHdr { ID = TEST_USER_ID, NumOfCard = 1 } };
                        userInfo.Cards.Add(new CSNCardData { Data = serverReq.VerifyReq.CardData });
                        serverSvc.HandleVerify(serverReq, ServerErrorCode.Success, userInfo);
                    }
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                {
                    Console.WriteLine("Subscription is cancelled");
                }
                else
                {
                    Console.WriteLine("Subscription error: {0}", e);
                }
            }
            finally
            {
                Console.WriteLine("Stop handling server requests");
            }
        }

        public void TestIdentify()
        {
            try
            {
                var reqStream = serverSvc.Subscribe(QUEUE_SIZE);

                CancellationTokenSource cancelToken = new CancellationTokenSource();
                returnError = true;
                HandleIdentify(reqStream, cancelToken.Token);

                Console.WriteLine(Environment.NewLine + "===== Server Matching: Identify Test =====" + Environment.NewLine);
                Console.WriteLine(">> Try to authenticate a fingerprint. It should fail since the device gateway will return an error code to the request.");
                KeyInput.PressEnter(">> Press ENTER for the next test." + Environment.NewLine);

                returnError = false;
                Console.WriteLine(">> Try to authenticate a fingerprint. The gateway will return SUCCESS with user information this time. The result will vary according to the authentication modes of the devices.");
                KeyInput.PressEnter(">> Press ENTER for the next test." + Environment.NewLine);

                cancelToken.Cancel();
                serverSvc.Unsubscribe();
            }
            catch (RpcException e)
            {
                Console.WriteLine("Cannot subscribe to the server requests: {0}", e);
                throw;
            }
        }

        async void HandleIdentify(IAsyncStreamReader<ServerRequest> reqStream, CancellationToken token)
        {
            Console.WriteLine("Start handling server requests...");

            try
            {
                while (await reqStream.MoveNext(token))
                {
                    var serverReq = reqStream.Current;

                    if (serverReq.ReqType != RequestType.IdentifyRequest)
                    {
                        Console.WriteLine("!! Request type is not IDENTIFY_REQUEST. Just ignore it.");
                        continue;
                    }

                    if (returnError)
                    {
                        Console.WriteLine("## Gateway returns IDENTIFY_FAIL.");
                        serverSvc.HandleIdentify(serverReq, ServerErrorCode.IdentifyFail, null);
                    }
                    else
                    {
                        Console.WriteLine("## Gateway returns SUCCESS with user information.");
                        var fingerData = new FingerData();
                        fingerData.Templates.Add(serverReq.IdentifyReq.TemplateData);
                        fingerData.Templates.Add(serverReq.IdentifyReq.TemplateData);

                        var userInfo = new UserInfo { Hdr = new UserHdr { ID = TEST_USER_ID, NumOfFinger = 1 } };
                        userInfo.Fingers.Add(fingerData);
                        serverSvc.HandleIdentify(serverReq, ServerErrorCode.Success, userInfo);
                    }
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                {
                    Console.WriteLine("Subscription is cancelled");
                }
                else
                {
                    Console.WriteLine("Subscription error: {0}", e);
                }
            }
            finally
            {
                Console.WriteLine("Stop handling server requests");
            }
        }

    }
}

