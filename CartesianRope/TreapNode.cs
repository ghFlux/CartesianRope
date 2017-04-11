using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace CartesianRope
{
    /// <typeparam name="T">Item type in container</typeparam>
    public sealed partial class Rope<T>
    {
        /// <summary>
        /// Node of cartesian tree.
        /// </summary>
        [DebuggerDisplay("{GetSize(Left)} - [{Length}] - {GetSize(Right)}")]
        internal class TreapNode
        {
            public int Priority { get; }

            public TreapNode lChild { get; internal set; }
            public TreapNode rChild { get; internal set; }

            public int Length { get; }
            public int Size { get; }
            public int Cost { get; }
            public T[] Data { get; }

            public double AverageAccess => (double)Cost / Size;

            public TreapNode(T[] data, int priority, int len, TreapNode left = null, TreapNode right = null)
            {
                Data = data;
                Priority = priority;
                lChild = left;
                rChild = right;
                Length = len;
                Size = GetSize(lChild) + Length + GetSize(rChild);
                Cost = GetCost(lChild) + Size + GetCost(rChild);
            }

            public TreapNode(T[] data) : this(data, RandomGenerator.Next(), data.Length)
            {
            }

            public static int GetSize(TreapNode node) => node == null ? 0 : node.Size;
            public static int GetCost(TreapNode node) => node == null ? 0 : node.Cost;
        }
    }
}