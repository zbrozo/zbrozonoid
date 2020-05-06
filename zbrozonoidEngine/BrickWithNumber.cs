using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class BrickWithNumber : Brick
    {
        public BrickWithNumber(int number, IBrick brick) : base(brick)
        {
            Number = number;
        }

        public int Number { get; }
    }
}
