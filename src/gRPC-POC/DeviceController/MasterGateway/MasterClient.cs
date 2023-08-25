using Grpc.Core;
using Gsdk.Login;
using Gsdk.Tenant;

namespace example
{
    public class MasterClient : GrpcClient
    {
        private const string ADMIN_TENANT_ID = "administrator";

        public void ConnectAdmin(string caFile, string adminCertFile, string adminKeyFile, string masterAddr, int masterPort)
        {
            var channelCredentials = new SslCredentials(File.ReadAllText(caFile), new KeyCertificatePair(File.ReadAllText(adminCertFile), File.ReadAllText(adminKeyFile)));
            var callCredentials = CallCredentials.FromInterceptor(JwtCredential.JwtAuthInterceptor);

            _channel = new Channel(masterAddr, masterPort, ChannelCredentials.Create(channelCredentials, callCredentials));

            var loginClient = new Login.LoginClient(_channel);

            var request = new LoginAdminRequest { AdminTenantCert = File.ReadAllText(adminCertFile), TenantID = ADMIN_TENANT_ID };
            var response = loginClient.LoginAdmin(request);

            JwtCredential.SetToken(response.JwtToken);
        }

        public void InitTenant(string tenantID, string gatewayID)
        {
            var tenantClient = new Tenant.TenantClient(_channel);

            var getRequest = new GetRequest { };
            getRequest.TenantIDs.Add(tenantID);

            try
            {
                var getResponse = tenantClient.Get(getRequest);

                if (getResponse.TenantInfos.Count == 1)
                {
                    Console.WriteLine("{0} is already registered", tenantID);
                    return;
                }
            }
            catch
            {
            }

            Console.WriteLine("{0} is not found. Trying to add the tenant...", tenantID);

            var tenantInfo = new TenantInfo { TenantID = tenantID };
            tenantInfo.GatewayIDs.Add(gatewayID);

            var addRequest = new AddRequest { };
            addRequest.TenantInfos.Add(tenantInfo);

            var addResponse = tenantClient.Add(addRequest);

            Console.WriteLine("{0} is registered successfully", tenantID);
        }

        public void ConnectTenant(string caFile, string tenantCertFile, string tenantKeyFile, string masterAddr, int masterPort)
        {
            var channelCredentials = new SslCredentials(File.ReadAllText(caFile), new KeyCertificatePair(File.ReadAllText(tenantCertFile), File.ReadAllText(tenantKeyFile)));
            var callCredentials = CallCredentials.FromInterceptor(JwtCredential.JwtAuthInterceptor);

            _channel = new Channel(masterAddr, masterPort, ChannelCredentials.Create(channelCredentials, callCredentials));

            var loginClient = new Login.LoginClient(_channel);

            var request = new LoginRequest { TenantCert = File.ReadAllText(tenantCertFile) };
            var response = loginClient.Login(request);

            JwtCredential.SetToken(response.JwtToken);
        }
    }
}