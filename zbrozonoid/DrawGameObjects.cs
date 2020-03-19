/*
Copyright(C) 2018 Tomasz Zbrożek

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using zbrozonoid.Menu;
using zbrozonoid.Views;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{

    public class DrawGameObjects : IDrawGameObjects
    {
        public RenderWindow Render { get; private set; }
        public IPrepareTextLine PrepareTextLine { get; private set; }
        public IGame game { get; private set; }

        private IViewModel viewModel;
        private IMenuViewModel menuViewModel;

        public DrawGameObjects(RenderWindow renderWindow, IPrepareTextLine prepareTextLine, IViewModel viewModel, IMenuViewModel menuViewModel, IGame game)
        {
            this.Render = renderWindow;
            this.game = game;
            this.viewModel = viewModel;
            this.PrepareTextLine = prepareTextLine;
            this.menuViewModel = menuViewModel;
        }
        /*
        public void DrawBackground(Sprite background)
        {
            Render.Draw(background);
        }
        */
        public void DrawBorders()
        {
            var borderManager = game.BorderManager;

            foreach (IBorder border in borderManager)
            {
                RectangleShape rectangle = new RectangleShape();
                rectangle.Position = new Vector2f(border.Boundary.Min.X, border.Boundary.Min.Y);
                rectangle.Size = new Vector2f(border.Boundary.Size.X, border.Boundary.Size.Y);
                rectangle.FillColor = Color.White;

                Render.Draw(rectangle);
            }
        }

        /*
        public void DrawBricks(List<Brick> bricksToDraw)
        {
            foreach (var brick in bricksToDraw.Where(x => x.IsVisible))
            {
                Render.Draw(brick.Rect);
            }
        }
        */
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
                Render.Draw(rect);
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

                Render.Draw(circle);
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
                        Render.Draw(circle);

                        opacity = opacity - 60;
                    }
                }
            }
        }

        public void DrawGameOver()
        {
            Render.Draw(viewModel.GameOverMessage);
        }

        public void DrawPressPlayToPlay()
        {
            Render.Draw(viewModel.PressButtonToPlayMessage);
        }

        public void DrawStopPlayMessage()
        {
            Render.Draw(viewModel.StopPlayMessage);
        }

    }
}
