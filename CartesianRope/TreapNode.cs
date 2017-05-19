using System;
using System.Diagnostics;

namespace CartesianRope
{
    /// <typeparam name="T"> Item type in container </typeparam>
    public sealed partial class Rope<T>
    {
        /// <summary>Node of cartesian tree.</summary>
        [DebuggerDisplay("{GetSize(LChild)} - [{Length}] - {GetSize(RChild)}")]
        internal class TreapNode : ICloneable, IEquatable<TreapNode>
        {
            public int Priority { get; }
            public TreapNode LChild { get; }
            public TreapNode RChild { get; }

            /// <summary>Array with actual data.</summary>
            public T[] Data { get; }

            /// <summary>Offset of stored data block.</summary>
            public int Offset { get; }

            /// <summary>Length of data block stored in this particular node.</summary>
            public int Length { get; }

            /// <summary>Total length of data chunks in this node's subtree.</summary>
            public int Size { get; }

            /// <summary>
            /// Total cost of accessing each item in this subtree.
            /// Calculated as sum of block lengths multiplied by their depth in tree.
            /// </summary>
            public int Cost { get; }

            /// <summary>Average count of indirections needed for accessing element at random index in this subtree.</summary>
            public double AverageAccess => (double)Cost / Size;

            public TreapNode(T[] data, int offset, int length, int priority, TreapNode left = null, TreapNode right = null)
            {
                Data = data;
                Offset = offset;
                Priority = priority;
                LChild = left;
                RChild = right;
                Length = length;
                Size = GetSize(LChild) + Length + GetSize(RChild);
                Cost = GetCost(LChild) + Size + GetCost(RChild);
            }

            public TreapNode(T[] data) : this(data, 0, data.Length, RandomGenerator.Next())
            {
                // No body needed.
            }

            public static int GetSize(TreapNode node) => node == null ? 0 : node.Size;
            public static int GetCost(TreapNode node) => node == null ? 0 : node.Cost;

            public override string ToString()
            {
                // TODO: remove this after active dev phase.
                return $"Node : [size : {Size}, l:{(LChild == null ? "NULL" : $"node({LChild.Size})")}, r:{(RChild == null ? "NULL" : $"node({RChild.Size})")}]";
            }

            /// <summary> Produces deep copy of current object. </summary>
            public object Clone()
            {
                return new TreapNode(
                        data: Data,
                        offset: Offset,
                        length: Length,
                        priority: Priority,
                        left: LChild != null ? LChild.Clone() as TreapNode : null,
                        right: RChild != null ? RChild.Clone() as TreapNode : null
                    );
            }

            static bool EqualsOrNull<U>(U obj1, U obj2) where U : IEquatable<U>
            {
                if (obj1 == null)
                    return (obj2 == null) ? true : obj2.Equals(obj1);
                else
                    return obj1.Equals(obj2);
            }

            public bool Equals(TreapNode other)
            {
                if (other == null) return false;

                return
                    Offset == other.Offset &&
                    Data == other.Data &&
                    Length == other.Length &&
                    Priority == other.Priority &&
                    
                    EqualsOrNull(LChild, other.LChild) &&
                    EqualsOrNull(RChild, other.RChild);
            }
        }
    }
}
