namespace zbrozonoidEngine
{
    public struct Vector2
    {
        public int X { get; }
        public int Y { get; }

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            Vector2 result = new Vector2(a.X + b.X, a.Y + b.Y);
            return result;
        }

        public override string ToString()
        {
            return $"X: { X}; Y: { Y}";
        }
    }
}
