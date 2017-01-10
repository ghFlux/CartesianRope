using System;
using System.Linq;
using CartesianRope;

namespace Test
{
    class Program
    {
        static Random rnd = new Random(1);

        static double GetAdvantage(int arrSize, int minLength, int maxLength)
        {
            int[] arr = new int[arrSize];
            for (int i = 0; i < arrSize; i++)
                arr[i] = minLength + rnd.Next() % (maxLength - minLength);

            var pureNode = NodeHelper.ConstructOptimal(arr);
            var implNode = arr.Select(x => new ImplicitTreap(rnd.Next(), x)).Aggregate(ImplicitTreap.Merge);
            // do testing
            return implNode.AverageAccess / pureNode.AverageAccess;
        }



        static void Main(string[] args)
        {
            const int testCount = 100;
            const int arrSize = 500;

            double min = double.MaxValue, max = double.MinValue;
            for (int i = 1; i <= testCount; i++)
            {
                double testResult = GetAdvantage(arrSize, 1, 5000);
                min = Math.Min(min, testResult);
                max = Math.Max(max, testResult);
                //Console.WriteLine("{0}\t/\t{1}", i, testCount);
            }
            Console.WriteLine("Optimal is better in {0} - {1} times.", min, max);
        }
    }
}