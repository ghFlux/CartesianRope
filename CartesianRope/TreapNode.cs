using System.Diagnostics;

namespace CartesianRope
{
    /// <typeparam name="T">Item type in container</typeparam>
    public sealed partial class Rope<T>
    {
        /// <summary>
        /// Node of cartesian tree.
        /// </summary>
        [DebuggerDisplay("{GetSize(lChild)} - [{Length}] - {GetSize(rChild)}")]
        internal class TreapNode
        {
            public int Priority { get; }
            public TreapNode lChild { get; private set; }
            public TreapNode rChild { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public T[] Data { get; }
            public int Offset { get; }
            /// <summary>
            /// Length of data block stored in this particular node.
            /// </summary>
            public int Length { get; }

            /// <summary>
            /// Total length of data chunks in this node's subtree.
            /// </summary>
            public int Size { get; }

            /// <summary>
            /// Total cost of accessing each item in this subtree.
            /// Calculated as sum of block lengths multiplied by their depth in tree.
            /// </summary>
            public int Cost { get; }

            /// <summary>
            /// Average count of indirections needed for accessing element at random index in this subtree.
            /// </summary>
            public double AverageAccess => (double)Cost / Size;

            public TreapNode(T[] data, int offset, int length, int priority, TreapNode left = null, TreapNode right = null)
            {
                Data = data;
                Offset = offset;
                Priority = priority;
                lChild = left;
                rChild = right;
                Length = length;
                Size = GetSize(lChild) + Length + GetSize(rChild);
                Cost = GetCost(lChild) + Size + GetCost(rChild);
            }

            public TreapNode(T[] data) : this(data, 0, data.Length, RandomGenerator.Next())
            {
            }

            public static int GetSize(TreapNode node) => node == null ? 0 : node.Size;
            public static int GetCost(TreapNode node) => node == null ? 0 : node.Cost;

            public override string ToString()
            {
                // TODO: remove this after active dev phase.
                return $"Node : [size : {Size}, l:{(lChild == null ? "NULL" : $"node({lChild.Size})")}, r:{(rChild == null ? "NULL" : $"node({rChild.Size})")}]";
            }
        }
    }
}
