using System;
using System.Linq;
using CartesianRope;
using System.Collections.Generic;

namespace Test
{
    class Program
    {
        static Random rnd = new Random();

        static double GetAdvantage(int arrSize, int minLength, int maxLength)
        {
            var nodes = Enumerable.Range(0, arrSize).Select(dummy => new TreapNode<int>(rnd.Next(), rnd.Next(minLength, maxLength)));
            var commonNode = nodes.Aggregate(TreapNode<int>.Merge);
            var optimalNode = TreapNode<int>.ConstructOptimal(commonNode.Flatten());

            return commonNode.AverageAccess / optimalNode.AverageAccess;
        }

        static void Main(string[] args)
        {
            const int testCount = 100;
            const int arrSize = 50;
            const int minLength = 1, maxLength = 25;

            var ratios = Enumerable.Range(0, testCount).Select(x => GetAdvantage(arrSize, minLength, maxLength)).ToArray();

            Array.Sort(ratios);
            
            Console.WriteLine($"Optimal is better in {ratios.Min()} - {ratios.Max()} times.");
            Console.WriteLine($"Average is {ratios.Average()}, median is {ratios[ratios.Length / 2]}");
        }
    }
}
