#define TESTING

using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace CartesianRope
{
    [DebuggerDisplay("{GetSize(Left)} - [{Length}] - {GetSize(Right)}")]
    public partial class TreapNode<T>
    {
        private int priority { get; }

        public TreapNode<T> Left { get; }
        public TreapNode<T> Right { get; }

        public int Length { get; }
        public int Size { get; }
        public int Cost { get; }

        public double AverageAccess => (double)Cost / Size;

        public TreapNode(int prior, int len, TreapNode<T> left = null, TreapNode<T> right = null)
        {
            this.priority = prior;
            Left = left;
            Right = right;
            Length = len;
            Size = GetSize(Left) + Length + GetSize(Right);
            Cost = GetCost(Left) + Size + GetCost(Right);

        } 

        private static int GetSize(TreapNode<T> n) => n == null ? 0 : n.Size;
        private static int GetCost(TreapNode<T> n) => n == null ? 0 : n.Cost;

        public void Split(int x, out TreapNode<T> L, out TreapNode<T> R)
        {
            TreapNode<T> newTree = null;
            int curIndex = GetSize(Left) + 1;

            if (curIndex <= x)
            {
                if (Right == null)
                    R = null;
                else
                    Right.Split(x - curIndex, out newTree, out R);
                L = new TreapNode<T>(priority, Length, Left, newTree);
            }
            else
            {
                if (Left == null)
                    L = null;
                else
                    Left.Split(x, out L, out newTree);
                R = new TreapNode<T>(priority, Length, newTree, Right);
            }
        }

        public static TreapNode<T> Merge(TreapNode<T> L, TreapNode<T> R)
        {
            if (L == null) return R;
            if (R == null) return L;

            if (L.priority > R.priority)
            {
                var newR = Merge(L.Right, R);
                return new TreapNode<T>(L.priority, L.Length, L.Left, newR);
            }
            else
            {
                var newL = Merge(L, R.Left);
                return new TreapNode<T>(R.priority, R.Length, newL, R.Right);
            }
        }

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
                        TreapNode<T> tmp = new TreapNode<T>(arr[split].priority, arr[split].Length, construct(from, split - 1), construct(split + 1, to));
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
                result.Add(new TreapNode<T>(node.priority, node.Length));
                flatten(node.Right);
            };
            flatten(this);
            return result.ToArray();
        }
    }
}