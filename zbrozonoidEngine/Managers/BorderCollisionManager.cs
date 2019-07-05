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
namespace zbrozonoidEngine.Managers
{
    using zbrozonoidEngine.Interfaces;

    public class BorderCollisionManager : IBorderCollisionManager
    {
        private IBorder border;

        private ICollisionManager collisionManager;

        public BorderCollisionManager(IBorder border, ICollisionManager collisionManager)
        {
            this.border = border;
            this.collisionManager = collisionManager;
        }

        public bool DetectAndVerify(IPad pad)
        {
            if (collisionManager.Detect(border, pad))
            {
                if (border.Type == Edge.Left)
                {
                    pad.Boundary.Min = new Vector2(border.Boundary.Max.X, pad.Boundary.Min.Y);
                    return true;
                }

                if (border.Type == Edge.Right)
                {
                    pad.Boundary.Min = new Vector2(border.Boundary.Min.X - pad.Boundary.Size.X, pad.Boundary.Min.Y);
                    return true;
                }
            }

            return false;
        }

        public bool DetectAndVerify(IBall ball)
        {
            if (collisionManager.Detect(border, ball))
            {
                collisionManager.Bounce(border,ball);
                ball.SavePosition();
                return true;
            }
            return false;
        }
    }
}
