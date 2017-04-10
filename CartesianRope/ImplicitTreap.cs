#define TESTING

using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace CartesianRope
{
    [DebuggerDisplay("{GetSize(Left)} - [{Length}] - {GetSize(Right)}")]
    public partial class TreapNode<T>
    {
        public int Priority { get; }

        public TreapNode<T> Left { get; internal set; }
        public TreapNode<T> Right { get; internal set; }

        public int Length { get; }
        public int Size { get; }
        public int Cost { get; }
        public T[] Data { get; }

        public double AverageAccess => (double)Cost / Size;

        public TreapNode(T[] data, int priority, int len, TreapNode<T> left = null, TreapNode<T> right = null)
        {
            Data = data;
            Priority = priority;
            Left = left;
            Right = right;
            Length = len;
            Size = GetSize(Left) + Length + GetSize(Right);
            Cost = GetCost(Left) + Size + GetCost(Right);
        } 

        public static int GetSize(TreapNode<T> node) => node == null ? 0 : node.Size;
        public static int GetCost(TreapNode<T> node) => node == null ? 0 : node.Cost;

        public static TreapNode<T> ConstructOptimal(TreapNode<T>[] arr)
        {
            int n = arr.Length;
            TreapNode<T>[,] cache = new TreapNode<T>[n, n];
            Func<int, int, TreapNode<T>> construct = null;
            construct = (from, to) =>
            {
                if (from > to) return null;
                if (from == to) return arr[from];
                if (cache[from, to] != null) return cache[from, to];

                TreapNode<T> res = null;
                {
                    int bestCost = int.MaxValue;
                    for (int split = from; split <= to; split++)
                    {
                        TreapNode<T> tmp = new TreapNode<T>(null, arr[split].Priority, arr[split].Length, construct(from, split - 1), construct(split + 1, to));
                        if (tmp.Cost < bestCost) bestCost = (res = tmp).Cost;
                    }
                }
                return cache[from, to] = res;
            };
            return construct(0, n - 1);
        }
        
        public TreapNode<T>[] Flatten()
        {
            List<TreapNode<T>> result = new List<TreapNode<T>>();
            Action<TreapNode<T>> flatten = null;
            flatten = (node) =>
            {
                if (node == null) return;
                flatten(node.Left);
                result.Add(new TreapNode<T>(null, node.Priority, node.Length));
                flatten(node.Right);
            };
            flatten(this);
            return result.ToArray();
        }
    }

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
                var newR = Merge(L.Right, R);
                return new TreapNode<T>(null, L.Priority, L.Length, L.Left, newR);
            }
            else
            {
                var newL = Merge(L, R.Left);
                return new TreapNode<T>(null, R.Priority, R.Length, newL, R.Right);
            }
        }

        public void Split(TreapNode<T> node, int index, out TreapNode<T> L, out TreapNode<T> R)
        {
            TreapNode<T> newTree = null;
            int leftSize = TreapNode<T>.GetSize(node.Left);

            if (leftSize < index) // going right
            {
                if (node.Right == null)
                    R = null;
                else
                    Split(node.Right, index - leftSize, out newTree, out R);

                L = new TreapNode<T>(node.Data, node.Priority, node.Length, node.Left, newTree);
            }
            else // going left
            {
                if (node.Left == null)
                    L = null;
                else
                    Split(node.Left, index, out L, out newTree);

                R = new TreapNode<T>(node.Data, node.Priority, node.Length, newTree, node.Right);
            }
        }

        private void SplitNode(TreapNode<T> node, int index, out TreapNode<T> L, out TreapNode<T> R)
        {
            L = new TreapNode<T>(data: node.Data, priority: RandomGenerator.Next(), len: index);
            R = new TreapNode<T>(data: node.Data, priority: RandomGenerator.Next(), len: node.Length - index);
        }

        private void Split2(TreapNode<T> node, int index, out TreapNode<T> L, out TreapNode<T> R)
        {
            Stack<TreapNode<T>> path = new Stack<TreapNode<T>>();

            while (true)
            {
                path.Push(node);

                int leftBound = TreapNode<T>.GetSize(node.Left);
                int rightBound = leftBound + node.Size;

                if (index < leftBound)
                {
                    
                }
                else if (rightBound <= index)
                {
                }
                else
                {
                }

            }
        }
    }
}