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
using SFML.Graphics;
using SFML.System;
using zbrozonoidLibrary;
using zbrozonoidLibrary.Interfaces;

namespace zbrozonoid
{

    public class DrawGameObjects : IDrawGameObjects
    {
        private RenderWindow renderWindow;
        private IGame game;
        private IViewModel viewModel;

        public DrawGameObjects(RenderWindow renderWindow, IViewModel viewModel, IGame game)
        {
            this.renderWindow = renderWindow;
            this.game = game;
            this.viewModel = viewModel;
        }

        public void DrawBackground(Sprite background)
        {
            renderWindow.Draw(background);
        }

        public void DrawBorders()
        {
            var borderManager = game.BorderManager;

            foreach (IBorder border in borderManager)
            {
                var element = border as IElement;
                if (element == null)
                {
                    continue;
                }

                RectangleShape rectangle = new RectangleShape();
                rectangle.Position = new Vector2f(element.PosX, element.PosY);
                rectangle.Size = new Vector2f(element.Width, element.Height);
                rectangle.FillColor = Color.White;

                renderWindow.Draw(rectangle);
            }
        }

        public void DrawBricks(List<Brick> bricksToDraw)
        {
            foreach (Brick brick in bricksToDraw)
            {
                if (brick.IsVisible)
                {
                    renderWindow.Draw(brick.Rect);
                }
            }
        }

        public void DrawPad()
        {
            foreach (IPad pad in game.PadManager)
            {
                game.GetPadPosition(pad, out int posX, out int posY);

                game.GetPadSize(pad, out int width, out int height);

                VertexArray rect = new VertexArray(PrimitiveType.Quads, 4);
                rect.Append(new Vertex(new Vector2f(posX, posY), Color.White));
                rect.Append(new Vertex(new Vector2f(posX + width, posY), Color.White));
                rect.Append(new Vertex(new Vector2f(posX + width, posY + height), Color.Blue));
                rect.Append(new Vertex(new Vector2f(posX, posY + height), Color.Blue));
                renderWindow.Draw(rect);
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

                renderWindow.Draw(circle);
            }
        }

        private void DrawTail(IBall ball)
        {
            ITail tail = game.TailManager.Find(ball);
            if (tail != null)
            {
                int i = 0;
                int opacity = 150;
                foreach (Position position in tail)
                {
                    ++i;
                    if (i % 14 == 0)
                    {
                        if (opacity < 0)
                        {
                            break;
                        }

                        ball.GetSize(out int width, out int height);

                        Color color = Color.Cyan;
                        color.A = (byte)opacity;

                        CircleShape circle = new CircleShape();
                        circle.Position = new Vector2f(position.X, position.Y);
                        circle.Radius = (float)width / 2;
                        circle.FillColor = color;
                        renderWindow.Draw(circle);

                        opacity = opacity - 60;
                    }
                }
            }
        }

        public void DrawTitle()
        {
            renderWindow.Draw(viewModel.Title);
        }

        public void DrawLivesAndScoresInfo()
        {
            renderWindow.Draw(viewModel.LiveAndScoresMessage);
        }

        public void DrawGameOver()
        {
            renderWindow.Draw(viewModel.GameOverMessage);
        }


        public void DrawPressPlayToPlay()
        {
            renderWindow.Draw(viewModel.PressButtonToPlayMessage);
        }

    }
}
