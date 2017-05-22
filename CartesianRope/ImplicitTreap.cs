using System.Diagnostics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace CartesianRope
{
    [DebuggerDisplay("Rope<{typeof(T).Name}>[{Length}]")]
    public sealed partial class Rope<T>
    {
        internal TreapNode Root { get; set; }
        internal static Random RandomGenerator { get; set; } = new Random();
        public static int DirectCopyThreshold { get; set; } = 32;

        public int Length { get; private set; }
        public double AverageAccess => Root == null ? 0 : Root.AverageAccess;
        
        [Pure]
        private Rope(TreapNode root)
        {
            Root = root;
            Length = (Root == null) ? 0 : Root.Size;
        }

        [Pure]
        public Rope(IEnumerable<T> sequence)
        {
            Root = sequence.Select(item => new TreapNode(new T[] { item })).Aggregate(null as TreapNode, Merge);
            Length = (Root == null) ? 0 : Root.Size;
        }

        [Pure]
        internal static TreapNode Merge(TreapNode L, TreapNode R)
        {
            if (L == null) return R;
            if (R == null) return L;

            if (L.Size + R.Size <= DirectCopyThreshold)
            {
                Debug.Assert(L.LChild == null && L.RChild == null);
                Debug.Assert(R.LChild == null && R.RChild == null);
                T[] dataArray = new T[L.Size + R.Size];
                Array.Copy(L.Data, L.Offset, dataArray, 0, L.Length);
                Array.Copy(R.Data, R.Offset, dataArray, L.Length, R.Length);
                return new TreapNode(dataArray);
            }

            if (L.Priority > R.Priority)
            {
                var newR = Merge(L.RChild, R);
                return new TreapNode(L.Data, L.Offset, L.Length, L.Priority, L.LChild, newR);
            }
            else
            {
                var newL = Merge(L, R.LChild);
                return new TreapNode(R.Data, R.Offset, R.Length, R.Priority, newL, R.RChild);
            }
        }

        [Pure]
        internal static void Traverse(TreapNode node, Action<TreapNode> action)
        {
            if (node == null) return;
            Traverse(node.LChild, action);
            action(node);
            Traverse(node.RChild, action);
        }

        /// <summary>Splits node content into two trees by specified <see cref="index"/>.</summary>
        /// <param name="node">Node to split</param>
        /// <param name="index">Splitting index</param>
        /// <param name="L">Left resulting tree</param>
        /// <param name="R">Right resulting tree</param>
        [Pure]
        internal void Split(TreapNode node, int index, out TreapNode L, out TreapNode R)
        {
            TreapNode newTree = null;
            int lBound = TreapNode.GetSize(node.LChild);
            int rBound = lBound + node.Size;

            if (index >= rBound) // going right
            {
                if (node.RChild == null)
                    R = null;
                else
                    Split(node.RChild, index - rBound, out newTree, out R);

                L = new TreapNode(node.Data, node.Offset, node.Length, node.Priority, node.LChild, newTree);
            }
            else if (index < lBound) // going left
            {
                if (node.LChild == null)
                    L = null;
                else
                    Split(node.LChild, index, out L, out newTree);

                R = new TreapNode(node.Data, node.Offset, node.Length, node.Priority, newTree, node.RChild);
            }
            else // split node in middle
            {
                index -= lBound;
                L = new TreapNode(
                        data: node.Data,
                        offset: node.Offset,
                        length: index,
                        priority: RandomGenerator.Next(),
                        left: node.LChild,
                        right: null
                    );
                R = new TreapNode(
                        data: node.Data,
                        offset: node.Offset + index,
                        length: node.Length - index,
                        priority: RandomGenerator.Next(),
                        left: null,
                        right: node.RChild
                    );
            }
        }

        /// <summary>
        /// Splits node content into two trees by index. Uses stack instead of recursion.
        /// </summary>
        /// <param name="node">Node to split</param>
        /// <param name="index">Splitting index</param>
        /// <param name="L">Left resulting tree</param>
        /// <param name="R">Right resulting tree</param>
        [Pure]
        private void SplitStack(TreapNode node, int index, out TreapNode L, out TreapNode R)
        {
            Stack<TreapNode> path = new Stack<TreapNode>();

            while (true)
            {
                path.Push(node);

                int leftBound = TreapNode.GetSize(node.LChild);
                int rightBound = leftBound + node.Size;

                if (index < leftBound) // going left
                {
                    node = node.LChild;
                }
                else if (rightBound <= index) // going right
                {
                    node = node.RChild;
                }
                else // split node
                {
                    L = new TreapNode(
                        data: node.Data,
                        offset: node.Offset,
                        length: index,
                        priority: RandomGenerator.Next(),
                        left: node.LChild,
                        right: null
                        );
                    R = new TreapNode(
                        data: node.Data,
                        offset: node.Offset + index,
                        length: node.Length - index,
                        priority: RandomGenerator.Next(),
                        left: null,
                        right: node.RChild
                        );
                    break;
                }
            }

            Console.Error.WriteLine(string.Join("\n", path.Reverse()));

            while (path.Count > 0)
            {
                var currentNode = path.Pop();

            }
        }

        [Pure]
        private static TreapNode[] Flatten(TreapNode root)
        {
            List<TreapNode> result = new List<TreapNode>();
            Traverse(root, node => result.Add(new TreapNode(node.Data, node.Offset, node.Length, node.Priority)));
            return result.ToArray();
        }

        [Pure]
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
                        TreapNode tmp = new TreapNode(arr[split].Data, arr[split].Offset, arr[split].Length, arr[split].Priority, construct(from, split - 1), construct(split + 1, to));
                        if (tmp.Cost < bestCost) bestCost = (res = tmp).Cost;
                    }
                }
                return cache[from, to] = res;
            };
            return construct(0, n - 1);
        }

        [Pure]
        public Rope<T> Optimized()
        {
            return new Rope<T>(ConstructOptimal(Flatten(Root)));
        }

        [Pure]
        public bool Equals(IEnumerable<T> other)
        {
            List<T> buffer = new List<T>();
            Traverse(Root, node => buffer.AddRange(node.Data.Take(node.Length)));
            return buffer.SequenceEqual(other);
        }

        [Pure]
        public static Rope<T> operator +(Rope<T> lhs, Rope<T> rhs)
        {
            return new Rope<T>(Merge(lhs.Root, rhs.Root));
        }

        [Pure]
        public Rope<T> Range(int index)
        {
            Split(Root, index, out TreapNode L, out TreapNode R);
            Rope<T> res = new Rope<T>(R);
            if (res.Length + index != this.Length) throw new Exception(":(");
            return res;
        }

        internal T Index(int index)
        {
            if (index < 0 || index >= Length) throw new IndexOutOfRangeException(nameof(index));
            TreapNode current = this.Root;
            int chunkBeginning = 0;
            while (true)
            {
                int leftSubtreeSize = TreapNode.GetSize(current.LChild);
                if (index < leftSubtreeSize)
                {
                    current = current.LChild;
                }
                else
                {
                    chunkBeginning += leftSubtreeSize;
                    index -= leftSubtreeSize;

                    if (index < current.Length)
                    {
                        return current.Data[current.Offset + index];
                    }
                    else
                    {
                        chunkBeginning += current.Length;
                        index -= current.Length;
                        current = current.RChild;
                    }
                }
            }
        }

        public T this[int index]
        {
            get { return Index(index); }
        }

        
    }
}