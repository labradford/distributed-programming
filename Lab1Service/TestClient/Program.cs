using MathServiceRef;
using System;

namespace TestClient
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            MathServiceClient proxy = new MathServiceClient();
            double result = await proxy.AddAsync(12.5, 2.3);
            Console.WriteLine(result);

            double subtractResult = await proxy.SubtractAsync(44.26, 22.13);
            Console.WriteLine(subtractResult);

            double multiplyResult = await proxy.MultiplyAsync(12.21, 21.12);
            Console.WriteLine(multiplyResult);

            double divideResult = await proxy.DivideAsync(144, 12);
            Console.WriteLine(divideResult);

            double circleAreaResult = await proxy.CircleAreaAsync(2.34);
            Console.WriteLine(circleAreaResult);

            Console.Write("Press <ENTER> quit...");
            Console.ReadLine();
        }
    }
}