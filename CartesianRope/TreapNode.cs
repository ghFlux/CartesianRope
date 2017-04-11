using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace CartesianRope
{
    /// <summary>
    /// Node of cartesian tree.
    /// </summary>
    /// <typeparam name="T">Item type in container</typeparam>
    [DebuggerDisplay("{GetSize(Left)} - [{Length}] - {GetSize(Right)}")]
    public partial class TreapNode<T>
    {
        public int Priority { get; }

        public TreapNode<T> lChild { get; internal set; }
        public TreapNode<T> rChild { get; internal set; }

        public int Length { get; }
        public int Size { get; }
        public int Cost { get; }
        public T[] Data { get; }

        public double AverageAccess => (double)Cost / Size;

        /// <summary>
        /// Constructs new instance of node class.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="priority"></param>
        /// <param name="len"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public TreapNode(T[] data, int priority, int len, TreapNode<T> left = null, TreapNode<T> right = null)
        {
            Data = data;
            Priority = priority;
            lChild = left;
            rChild = right;
            Length = len;
            Size = GetSize(lChild) + Length + GetSize(rChild);
            Cost = GetCost(lChild) + Size + GetCost(rChild);
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
            Rope<T>.Traverse(this, node => result.Add(new TreapNode<T>(node.Data, node.Priority, node.Length)));
            return result.ToArray();
        }
    }
}
