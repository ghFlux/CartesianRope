namespace CartesianRope
{
    public class ImplicitTreap
    {
        private int y;

        public ImplicitTreap Left;
        public ImplicitTreap Right;

        public int Length;
        public int Size;
        public int Cost;

        public double AverageAccess => (double)Cost / Size; 


        public ImplicitTreap(int y, int len, ImplicitTreap left = null, ImplicitTreap right = null)
        {
            this.y = y;
            Left = left;
            Right = right;
            Length = len;
            Size = GetSize(Left) + Length + GetSize(Right);
            Cost = GetCost(Left) + Size + GetCost(Right);
        }

        private static int GetSize(ImplicitTreap n) => n == null ? 0 : n.Size;
        private static int GetCost(ImplicitTreap n) => n == null ? 0 : n.Cost;

        public void Split(int x, out ImplicitTreap L, out ImplicitTreap R)
        {
            ImplicitTreap newTree = null;
            int curIndex = GetSize(Left) + 1;

            if (curIndex <= x)
            {
                if (Right == null)
                    R = null;
                else
                    Right.Split(x - curIndex, out newTree, out R);
                L = new ImplicitTreap(y, Length, Left, newTree);
            }
            else
            {
                if (Left == null)
                    L = null;
                else
                    Left.Split(x, out L, out newTree);
                R = new ImplicitTreap(y, Length, newTree, Right);
            }
        }

        public static ImplicitTreap Merge(ImplicitTreap L, ImplicitTreap R)
        {
            if (L == null) return R;
            if (R == null) return L;

            if (L.y > R.y)
            {
                var newR = Merge(L.Right, R);
                return new ImplicitTreap(L.y, L.Length, L.Left, newR);
            }
            else
            {
                var newL = Merge(L, R.Left);
                return new ImplicitTreap(R.y, R.Length, newL, R.Right);
            }
        }

    }
}