#define TESTING

using System.Diagnostics;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CartesianRope
{
    [DebuggerDisplay("{GetSize(Left)} - [{Length}] - {GetSize(Right)}")]
    public sealed partial class Rope<T>
    {
        internal TreapNode Root { get; set; }
        private static Random RandomGenerator { get; set; } = new Random();
        public static int DirectCopyThreshold { get; set; } = 32;

        public int Length { get; private set; }
        public double AverageAccess => Root == null ? 0 : Root.AverageAccess;

        private Rope(TreapNode root)
        {
            Root = root;
            Length = (Root == null) ? 0 : Root.Length;
        }

        public Rope(IEnumerable<T> sequence) : this(new TreapNode(sequence.ToArray()))
        {
        }

        internal static TreapNode Merge(TreapNode L, TreapNode R)
        {
            if (L == null) return R;
            if (R == null) return L;

            if (L.Priority > R.Priority)
            {
                var newR = Merge(L.rChild, R);
                return new TreapNode(null, L.Priority, L.Length, L.lChild, newR);
            }
            else
            {
                var newL = Merge(L, R.lChild);
                return new TreapNode(null, R.Priority, R.Length, newL, R.rChild);
            }
        }

        internal static void Traverse(TreapNode node, Action<TreapNode> action)
        {
            if (node == null) return;
            Traverse(node.lChild, action);
            action(node);
            Traverse(node.rChild, action);
        }

        /// <summary>
        /// Splits node content into two trees by index.
        /// </summary>
        /// <param name="node">Node to split</param>
        /// <param name="index">Splitting index</param>
        /// <param name="L">Left resulting tree</param>
        /// <param name="R">Right resulting tree</param>
        internal void Split(TreapNode node, int index, out TreapNode L, out TreapNode R)
        {
            TreapNode newTree = null;
            int lBound = TreapNode.GetSize(node.lChild);
            int rBound = lBound + node.Size;

            if (index >= rBound) // going right
            {
                if (node.rChild == null)
                    R = null;
                else
                    Split(node.rChild, index - rBound, out newTree, out R);

                L = new TreapNode(node.Data, node.Priority, node.Length, node.lChild, newTree);
            }
            else if (index < lBound) // going left
            {
                if (node.lChild == null)
                    L = null;
                else
                    Split(node.lChild, index, out L, out newTree);

                R = new TreapNode(node.Data, node.Priority, node.Length, newTree, node.rChild);
            }
            else // split node in middle
            {
                index -= lBound;
                L = new TreapNode(data: node.Data, priority: RandomGenerator.Next(), len: index, left: node.lChild, right: null);
                R = new TreapNode(data: node.Data, priority: RandomGenerator.Next(), len: node.Length - index, left: null, right: node.rChild);
            }
        }

        /// <summary>
        /// Splits node content into two trees by index. Uses stack instead of recursion.
        /// </summary>
        /// <param name="node">Node to split</param>
        /// <param name="index">Splitting index</param>
        /// <param name="L">Left resulting tree</param>
        /// <param name="R">Right resulting tree</param>
        private void SplitStack(TreapNode node, int index, out TreapNode L, out TreapNode R)
        {
            Stack<TreapNode> path = new Stack<TreapNode>();
            L = R = null;

            while (true)
            {
                path.Push(node);

                int leftBound = TreapNode.GetSize(node.lChild);
                int rightBound = leftBound + node.Size;

                if (index < leftBound) // going left
                {
                    node = node.lChild;
                }
                else if (rightBound <= index) // going right
                {
                    node = node.rChild;
                }
                else // split node
                {
                    L = new TreapNode(
                            data: node.Data,
                            priority: RandomGenerator.Next(),
                            len: index,
                            left: node.lChild,
                            right: null
                        );
                    R = new TreapNode(
                            data: node.Data,
                            priority: RandomGenerator.Next(),
                            len: node.Length - index,
                            left: null,
                            right: node.rChild
                        );
                }
                while (path.Count > 0)
                {
                    var currentNode = path.Pop();

                }

                Console.WriteLine(string.Join(" - ", path.Select(p => p.Size)));
            }
        }

        private static TreapNode[] Flatten(TreapNode root)
        {
            List<TreapNode> result = new List<TreapNode>();
            Traverse(root, node => result.Add(new TreapNode(node.Data, node.Priority, node.Length)));
            return result.ToArray();
        }

        private static TreapNode ConstructOptimal(TreapNode[] arr)
        {
            int n = arr.Length;
            TreapNode[,] cache = new TreapNode[n, n];
            Func<int, int, TreapNode> construct = null;
            construct = (from, to) =>
            {
                if (from > to) return null;
                if (from == to) return arr[from];
                if (cache[from, to] != null) return cache[from, to];

                TreapNode res = null;
                {
                    int bestCost = int.MaxValue;
                    for (int split = from; split <= to; split++)
                    {
                        TreapNode tmp = new TreapNode(null, arr[split].Priority, arr[split].Length, construct(from, split - 1), construct(split + 1, to));
                        if (tmp.Cost < bestCost) bestCost = (res = tmp).Cost;
                    }
                }
                return cache[from, to] = res;
            };
            return construct(0, n - 1);
        }

        public Rope<T> Optimized()
        {
            return new Rope<T>(ConstructOptimal(Flatten(Root)));
        }

        public bool Equals(IEnumerable<T> other)
        {
            List<T> buffer = new List<T>();
            Traverse(Root, node => buffer.AddRange(node.Data.Take(node.Length)));
            return buffer.SequenceEqual(other);
        }

        public static Rope<T> operator +(Rope<T> lhs, Rope<T> rhs)
        {
            return new Rope<T>(Merge(lhs.Root, rhs.Root));
        }
    }
}