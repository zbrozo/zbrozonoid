using System;
using SFML.Graphics;

namespace zbrozonoid.Views
{
    public interface IPrepareTextLine
    {
        RenderWindow Render { get; }

        Text PrepareMenuItem(string name, int number, bool isCurrent);
        //Text Prepare(string text, int lineNumber, uint fontSize = 50);
        Text Prepare(string text, int lineNumber,
                            bool horizCenter = true,
                            bool vertReverse = false,
                            int offsetX = 0,
                            int offsetY = 0,
                            uint fontSize = 50);

    }
}
