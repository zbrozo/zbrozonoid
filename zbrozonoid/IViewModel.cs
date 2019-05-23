using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace zbrozonoid
{
    public interface IViewModel
    {
        List<Brick> Bricks { get; }
        Dictionary<int, Color> Colors { get; }

        void PrepareBricksToDraw();
    }
}
