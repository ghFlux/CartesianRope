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
        internal TreapNode<T> Root { get; set; }
        private static Random RandomGenerator { get; set; } = new Random();

        public static int DirectCopyThreshold { get; set; } = 32;

        public Rope(IEnumerable<T> sequence)
        {
            foreach (T item in sequence)
            {
                Root = Merge(Root, new TreapNode<T>(null, RandomGenerator.Next(), 1));
            }
        }

        public static TreapNode<T> Merge(TreapNode<T> L, TreapNode<T> R)
        {
            if (L == null) return R;
            if (R == null) return L;

            if (L.Priority > R.Priority)
            {
                var newR = Merge(L.rChild, R);
                return new TreapNode<T>(null, L.Priority, L.Length, L.lChild, newR);
            }
            else
            {
                var newL = Merge(L, R.lChild);
                return new TreapNode<T>(null, R.Priority, R.Length, newL, R.rChild);
            }
        }

        public static void Traverse(TreapNode<T> node, Action<TreapNode<T>> action)
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
        public void Split(TreapNode<T> node, int index, out TreapNode<T> L, out TreapNode<T> R)
        {
            TreapNode<T> newTree = null;
            int lBound = TreapNode<T>.GetSize(node.lChild);
            int rBound = lBound + node.Size;

            if (index >= rBound) // going right
            {
                if (node.rChild == null)
                    R = null;
                else
                    Split(node.rChild, index - rBound, out newTree, out R);

                L = new TreapNode<T>(node.Data, node.Priority, node.Length, node.lChild, newTree);
            }
            else if (index < lBound) // going left
            {
                if (node.lChild == null)
                    L = null;
                else
                    Split(node.lChild, index, out L, out newTree);

                R = new TreapNode<T>(node.Data, node.Priority, node.Length, newTree, node.rChild);
            }
            else // split node in middle
            {
                index -= lBound;
                L = new TreapNode<T>(data: node.Data, priority: RandomGenerator.Next(), len: index, left: node.lChild, right: null);
                R = new TreapNode<T>(data: node.Data, priority: RandomGenerator.Next(), len: node.Length - index, left: null, right: node.rChild);
            }
        }

        /// <summary>
        /// Splits node content into two trees by index. Uses stack instead of recursion.
        /// </summary>
        /// <param name="node">Node to split</param>
        /// <param name="index">Splitting index</param>
        /// <param name="L">Left resulting tree</param>
        /// <param name="R">Right resulting tree</param>
        private void SplitStack(TreapNode<T> node, int index, out TreapNode<T> L, out TreapNode<T> R)
        {
            Stack<TreapNode<T>> path = new Stack<TreapNode<T>>();
            L = R = null;

            while (true)
            {
                path.Push(node);

                int leftBound = TreapNode<T>.GetSize(node.lChild);
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
                    L = new TreapNode<T>(
                            data: node.Data,
                            priority: RandomGenerator.Next(),
                            len: index,
                            left: node.lChild,
                            right: null
                        );
                    R = new TreapNode<T>(
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

        public bool Equals(IEnumerable<T> other)
        {
            List<T> buffer = new List<T>();
            Traverse(Root, node => buffer.AddRange(node.Data.Take(node.Length)));
            return buffer.SequenceEqual(other);
        }
    }
}