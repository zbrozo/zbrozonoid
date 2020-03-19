using System;
using SFML.Graphics;

namespace zbrozonoid
{
    public interface IRenderProxy
    {
        void Draw(Drawable o);

        Text PrepareTextLine(string text, int lineNumber,
                            bool horizCenter = true,
                            bool vertReverse = false,
                            int offsetX = 0,
                            int offsetY = 0,
                            uint fontSize = 50);

    }
}
