using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.ServiceModel;

namespace SharedLib
{
    [ServiceContract]
    public interface IMathService
    {
        [OperationContract]
        bool IsPrime(BigInteger number);

        [OperationContract]
        BigInteger Sqrt(BigInteger number);
    }
}
