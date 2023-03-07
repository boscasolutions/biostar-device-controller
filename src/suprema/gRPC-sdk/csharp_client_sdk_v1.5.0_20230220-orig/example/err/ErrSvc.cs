using System;
using Gsdk.Err;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

namespace example
{
  class ErrSvc
  {
    public static MultiErrorResponse GetMultiError(RpcException e) {
      Console.WriteLine("Exception: {0} {1}", e, e.Trailers);

      var enumerator = e.Trailers.GetEnumerator();

      while(enumerator.MoveNext()) {
        var status = Google.Rpc.Status.Parser.ParseFrom(enumerator.Current.ValueBytes);

        foreach(Any any in status.Details) {
          if(any.TryUnpack(out MultiErrorResponse errResponse)) {
            return errResponse;
          }
        }
      }

      return null; 
    }
  }
}