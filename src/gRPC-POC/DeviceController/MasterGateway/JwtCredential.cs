using Grpc.Core;
using Grpc.Core.Utils;

namespace example
{
    class JwtCredential
    {
        private const string JWT_TOKEN_KEY = "token";
        private static string jwtToken = "";

        public static void SetToken(string token)
        {
            jwtToken = token;
        }

        public static Task JwtAuthInterceptor(AuthInterceptorContext context, Metadata metadata)
        {
            metadata.Add(JWT_TOKEN_KEY, jwtToken);
            return TaskUtils.CompletedTask;
        }
    }
}