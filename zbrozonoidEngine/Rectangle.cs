namespace zbrozonoidEngine
{
    public class Rectangle
    {
        private Vector2 min;
        private Vector2 max;
        private Vector2 size;

        public Vector2 Min
        {
            get
            {
                return min;
            }

            set
            {
                min = value;
                max = value + size;
            }
        }

        public Vector2 Max => max;

        public Vector2 Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
                max = min + value;
            }
        }

        public Rectangle()
        {
        }

        public Rectangle(int x, int y, int width, int height)
        {
            min = new Vector2(x, y);
            size = new Vector2(width, height);
            max = min + size;
        }
    }
}
