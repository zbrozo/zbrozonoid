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
namespace zbrozonoidLibrary.Managers
{
    using zbrozonoidLibrary.Interfaces;

    public class ScreenCollisionManager : IScreenCollisionManager
    {
        private readonly IScreen screen;

        public ScreenCollisionManager(IScreen screen)
        {
            this.screen = screen;
        }

        public bool DetectAndVerify(IPad pad)
        {
            if (pad.Boundary.Min.X <= 0)
            {
                pad.Boundary.Min = new Vector2(0, pad.Boundary.Min.Y);
            }

            if (pad.Boundary.Min.X > screen.Width - pad.Boundary.Size.X)
            {
                pad.Boundary.Min = new Vector2(screen.Width - pad.Boundary.Size.X, pad.Boundary.Min.Y);
            }

            return false;
        }

        public bool DetectAndVerify(IBall ball)
        {
            if (ball.Boundary.Min.X < 0)
            {
                ball.Boundary.Min = new Vector2(0, ball.Boundary.Min.Y);
                return ball.Bounce(Edge.Right);
            }

            if (ball.Boundary.Min.X > screen.Width - ball.Boundary.Size.X)
            {
                ball.Boundary.Min = new Vector2(screen.Width - ball.Boundary.Size.X, ball.Boundary.Min.Y);
                return ball.Bounce(Edge.Left);
            }

            if (ball.Boundary.Min.Y < 0)
            {
                ball.Boundary.Min = new Vector2(ball.Boundary.Min.X, 0);
                return ball.Bounce(Edge.Bottom);
            }

            if (ball.Boundary.Min.Y > screen.Height - ball.Boundary.Size.Y)
            {
                ball.Boundary.Min = new Vector2(ball.Boundary.Min.X, screen.Height - ball.Boundary.Size.Y);
                return ball.Bounce(Edge.Top);
            }

            return false;
        }

        public bool Detect(IBall ball)
        {
            ball.GetSize(out int width, out int height);

            if (ball.Boundary.Min.X < -width)
            {
                return true;
            }

            if (ball.Boundary.Min.X > screen.Width)
            {
                return true;
            }

            if (ball.Boundary.Min.Y < -height)
            {
                return true;
            }

            if (ball.Boundary.Min.Y > screen.Height)
            {
                return true;
            }

            return false;
        }


    }
}
