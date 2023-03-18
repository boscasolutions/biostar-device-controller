using Google.Protobuf.Collections;
using Grpc.Core;
using Gsdk.Operator;

namespace example
{
    public class OperatorSvc
    {
        private Operator.OperatorClient operatorClient;

        public OperatorSvc(Channel channel)
        {
            operatorClient = new Operator.OperatorClient(channel);
        }

        public RepeatedField<Gsdk.Auth.Operator> GetList(uint deviceID)
        {
            var request = new GetListRequest { DeviceID = deviceID };
            var response = operatorClient.GetList(request);

            return response.Operators;
        }

        public void Add(uint deviceID, Gsdk.Auth.Operator[] operators)
        {
            var request = new AddRequest { DeviceID = deviceID };
            request.Operators.AddRange(operators);
            operatorClient.Add(request);
        }

        public void DeleteAll(uint deviceID)
        {
            var request = new DeleteAllRequest { DeviceID = deviceID };
            operatorClient.DeleteAll(request);
        }
    }
}