using System;
using SFML.Graphics;
using SFML.System;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.Views
{
    public class GamePlayView : IView
    {
        private IRenderProxy render;
        private IView playfieldView;
        private IGame game;

        public GamePlayView(IRenderProxy render,
                            IGamePlayfieldView playfieldView,
                            IGame game)
        {
            this.render = render;
            this.playfieldView = playfieldView;
            this.game = game;
        }

        public void Display()
        {
            playfieldView.Display();

            DrawPad();
            DrawBall();
        }

        public void Dispose()
        {
        }


        public void DrawPad()
        {
            foreach (var value in game.PadManager)
            {
                IPad pad = value.Item3;

                game.GetPadPosition(pad, out int posX, out int posY);

                game.GetPadSize(pad, out int width, out int height);

                VertexArray rect = new VertexArray(PrimitiveType.Quads, 4);
                rect.Append(new Vertex(new Vector2f(posX, posY), Color.White));
                rect.Append(new Vertex(new Vector2f(posX + width, posY), Color.White));
                rect.Append(new Vertex(new Vector2f(posX + width, posY + height), Color.Blue));
                rect.Append(new Vertex(new Vector2f(posX, posY + height), Color.Blue));
                render.Draw(rect);
            }
        }

        public void DrawBall()
        {
            var ballManager = game.BallManager;
            foreach (IBall ball in ballManager)
            {
                ball.GetPosition(out int posX, out int posY);
                ball.GetSize(out int width, out int height);

                DrawTail(ball);

                CircleShape circle = new CircleShape();
                circle.Position = new Vector2f(posX, posY);
                circle.Radius = (float)width / 2;
                circle.FillColor = Color.Cyan;

                render.Draw(circle);
            }
        }

        private void DrawTail(IBall ball)
        {
            ITail tail = game.TailManager.Find(ball);
            if (tail != null)
            {
                int i = 0;
                int opacity = 150;

                ball.GetSize(out int width, out int height);

                foreach (Vector2 position in tail)
                {
                    ++i;
                    if (i % 14 == 0)
                    {
                        if (opacity < 0)
                        {
                            break;
                        }

                        Color color = Color.Cyan;
                        color.A = (byte)opacity;

                        CircleShape circle = new CircleShape();
                        circle.Position = new Vector2f(position.X, position.Y);
                        circle.Radius = (float)width / 2;
                        circle.FillColor = color;
                        render.Draw(circle);

                        opacity = opacity - 60;
                    }
                }
            }
        }

    }
}
