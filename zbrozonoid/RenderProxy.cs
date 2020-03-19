using System;
using SFML.Graphics;
using zbrozonoid.Views;

namespace zbrozonoid
{
    public class RenderProxy : IRenderProxy
    {
        private readonly RenderWindow render;
        private readonly IPrepareTextLine prepareTextLine;

        public RenderProxy(RenderWindow renderWindow, IPrepareTextLine prepareTextLine)
        {
            this.render = renderWindow;
            this.prepareTextLine = prepareTextLine;
        }

        public void Draw(Drawable o)
        {
            render.Draw(o);
        }

        public Text PrepareTextLine(string text, int lineNumber)
        {
            return prepareTextLine.Prepare(text, lineNumber);
        }

        public Text PrepareTextLine(string text, int lineNumber, bool horizCenter = true, bool vertReverse = false, int offsetX = 0, int offsetY = 0, uint fontSize = 50)
        {
            return prepareTextLine.Prepare(
                            text,
                            lineNumber,
                            horizCenter,
                            vertReverse,
                            offsetX,
                            offsetY,
                            fontSize);
        }
    }
}
