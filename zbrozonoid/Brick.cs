using System;
using SFML.Graphics;

namespace zbrozonoid
{
        public class Brick
        {
            public bool IsVisible { get; set; } = true;
            public RectangleShape Rect { get; set; }

            public Brick(RectangleShape rect)
            {
                Rect = rect;
            }
        }
}
