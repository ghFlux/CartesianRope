using System;
using System.Collections.Generic;
using System.Linq;
using CartesianRope;

namespace Test
{
    class Program
    {
        static Random rnd = new Random();

        static double GetAdvantage(int arrSize, int minLength, int maxLength)
        {
            var nodes = Enumerable.Range(0, arrSize).Select(dummy => new Rope<int>(new int[rnd.Next(minLength, maxLength)]));
            var commonNode = nodes.Aggregate((a, b) => a + b);
            var optimalNode = commonNode.Optimized();

            var s2 = commonNode.Range(rnd.Next(commonNode.Length));

            return commonNode.AverageAccess / optimalNode.AverageAccess;
        }

        static int[] RopeChunkSizes<T>(Rope<T> rope)
        {
            List<int> result = new List<int>();
            Rope<T>.Traverse(rope.Root, node => result.Add(node.Length));
            return result.ToArray();
        }

        static void Main(string[] args)
        {
            const int testCount = 100;
            const int arrSize = 50;
            const int minLength = 20, maxLength = 25;
            
            var ratios = Enumerable.Range(0, testCount).Select(x => GetAdvantage(arrSize, minLength, maxLength)).ToArray();
            Array.Sort(ratios);

            Console.WriteLine($"Optimal is better in {ratios.Min()} - {ratios.Max()} times.");
            Console.WriteLine($"Average is {ratios.Average()}, median is {ratios[ratios.Length / 2]}");


            var sampleArray = new int[] { 1, 2, 3};
            Rope<int>.DirectCopyThreshold = 6;
            Rope<int> rope = Enumerable.Range(0, 5).Select(__ => new Rope<int>(sampleArray)).Aggregate((acc, r) => acc + r);

            var chunkSizes = RopeChunkSizes(rope);
            Console.WriteLine($"Chunk sizes of rope: {string.Join(" ", chunkSizes)}");
            Console.WriteLine($"RopeContent: [{string.Join(" ", rope)}]");
        }
    }
}
