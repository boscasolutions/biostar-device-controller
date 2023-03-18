using Gsdk.Auth;
using System;

namespace example
{
    class OperatorTest
    {
        private OperatorSvc operatorSvc;

        public OperatorTest(OperatorSvc svc)
        {
            operatorSvc = svc;
        }

        public void AddOperator(uint deviceID)
        {
            var operators = new Operator
            {
                UserID = "10",
                Level = OperatorLevel.Admin
            };

            operatorSvc.Add(deviceID, new Operator[] { operators });
            Console.WriteLine("Operator Added");
        }

        public void GetOperator(uint deviceID)
        {
            var operators = operatorSvc.GetList(deviceID);
            Console.WriteLine("Test Operators: {0}", operators);
        }

        public void RemoveOperator(uint deviceID)
        {
            operatorSvc.DeleteAll(deviceID);
            Console.WriteLine("Operator removed");
        }
    }
}

