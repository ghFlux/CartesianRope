using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CartesianRope.Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestInitialize]
        public void TestSetup()
        {
            int seed = 42;
            Rope<int>.RandomGenerator = new Random(seed);
        }

        [TestMethod]
        public void TestRopeConstructor()
        {
            Rope<int> r1 = new Rope<int>(new int[] { 1, 3, 4 });
        }

        [TestMethod]
        public void TestRopeImmutability()
        {
            Rope<int> rope = new Rope<int>(new int[] { 1, 3, 4 });
            var rootCopy = rope.Root.Clone() as Rope<int>.TreapNode;
            rope.Range(1);
            var newRoot = rope.Root;
            Assert.IsTrue(newRoot.Equals(rootCopy));
        }

        [TestMethod]
        public void TestRopeIndexing()
        {
            Random rand = new Random();
            int chunkCount = 1000;
            int chunkMaxSize = 100;
            int probeCount = 1000000;

            Func<int, int[]> makeChunk = size => Enumerable.Range(0, size).Select(dummy => rand.Next()).ToArray();

            List<int> realContent = new List<int>();
            Rope<int> rope = new Rope<int>(new int[] { });

            for (int i = 0; i < chunkCount; i++)
            {
                int[] nextChunk = makeChunk(rand.Next(0, chunkMaxSize));
                realContent.AddRange(nextChunk);
                rope = rope + new Rope<int>(nextChunk);
            }

            for (int i = 0; i < probeCount; i++)
            {
                int index = rand.Next(rope.Length);
                if (rope[index] != realContent[index]) throw new Exception("Shit happened");
            }
        }

    }
}
