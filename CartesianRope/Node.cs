using System;
using System.Diagnostics;

namespace CartesianRope
{
    [DebuggerDisplay("{GetSize(lChild)} - [{Length}] - {GetSize(rChild)}")]
    public class Node
    {
        public int Length { get; private set; }
        public int Size { get; private set; }
        public int Cost { get; private set; }

        public double AverageAccess => (double)Cost / Size;

        private Node lChild, rChild;

        private static int GetSize(Node n) => n == null ? 0 : n.Size; 
        private static int GetCost(Node n) => n == null ? 0 : n.Cost; 

        public Node(int len, Node lChild = null, Node rChild = null)
        {
            this.lChild = lChild;
            this.rChild = rChild;
            Length = len;
            Size = len + GetSize(lChild) + GetSize(rChild);
            Cost = Size + GetCost(lChild) + GetCost(rChild);
        }
    }

    public static class NodeHelper
    {
        public static Node ConstructOptimal(int[] arr)
        {
            int n = arr.Length;
            Node[,] cache = new Node[n, n];
            Func<int, int, Node> rec = null;
            rec = (from, to) =>
            {
                if (from > to) return null;
                if (cache[from, to] != null) return cache[from, to];
                Node res = null;
                if (from == to) res = new Node(arr[from]);
                else
                {
                    int bestCost = int.MaxValue;
                    for (int split = from; split <= to; split++)
                    {
                        Node tmp = new Node(arr[split], rec(from, split - 1), rec(split + 1, to));
                        if (tmp.Cost < bestCost) bestCost = (res = tmp).Cost;
                    }
                }
                return cache[from, to] = res;
            };
            return rec(0, n - 1);
        }
    }
}