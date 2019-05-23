using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using zbrozonoidLibrary;
using zbrozonoidLibrary.Interfaces;

namespace zbrozonoid
{
    public class ViewModel : IViewModel
    {
        private List<Brick> bricksToDraw = new List<Brick>();

        private IGame game;

        private Dictionary<int, Color> colors = new Dictionary<int, Color>
                                            {
                                                { 0, Color.Black },
                                                { 1, Color.White },
                                                { 2, Color.Red },
                                                { 3, Color.Cyan },
                                                { 4, new Color(204, 68, 204) },
                                                { 5, Color.Green },
                                                { 6, Color.Blue },
                                                { 7, Color.Yellow },
                                                { 8, new Color(221, 136, 85) },
                                                { 9, new Color(255, 119, 199) },
                                                { 10, new Color(255, 119, 119) }, // LightRed
                                                { 11, new Color(51, 51, 51) },
                                                { 12, new Color(119, 119, 119) },
                                                { 13, new Color(170, 255, 102) },
                                                { 14, new Color(0, 136, 255) },
                                                { 15, new Color(187, 187, 187) }
                                            };

        public List<Brick> Bricks => bricksToDraw;
        public Dictionary<int, Color> Colors => colors;

        public ViewModel(IGame game)
        {
            this.game = game;
        }

        public void PrepareBricksToDraw()
        {
            bricksToDraw.Clear();

            List<IBrick> bricks = game.Bricks;
            foreach (IBrick brick in bricks)
            {
                if (/*!brick.Hit &&*/ brick.Type > 0)
                {
                    if (colors.TryGetValue((int)brick.ColorNumber, out Color color))
                    {
                        RectangleShape rectangle = new RectangleShape();
                        rectangle.Position = new Vector2f(brick.PosX, brick.PosY);
                        rectangle.Size = new Vector2f(brick.Width, brick.Height);
                        rectangle.FillColor = color;

                        Brick brickToDraw = new Brick(rectangle);
                        bricksToDraw.Add(brickToDraw);
                    }
                }
            }
        }

    }
}
