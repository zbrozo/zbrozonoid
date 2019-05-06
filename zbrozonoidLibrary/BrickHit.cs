using System;
using zbrozonoidLibrary.Interfaces;

namespace zbrozonoidLibrary
{
    public class BrickHit
    {
        public BrickHit(int number, IBrick brick)
        {
            Number = number;
            Brick = brick;
        }

        public int Number { get; set; }
        public IBrick Brick { get; set; }
    }
}
