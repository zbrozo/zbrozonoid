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
                IElement borderElement = border as IElement;
                IElement padElement = pad as IElement;

                if (border.Type == Edge.Left)
                {
                    padElement.PosX = borderElement.PosX + borderElement.Width;
                    return true;
                }

                if (border.Type == Edge.Right)
                {
                    padElement.PosX = borderElement.PosX - padElement.Width;
                    return true;
                }
            }

            return false;
        }

        public bool DetectAndVerify(IBall ball)
        {
            if (collisionManager.Detect(border, ball))
            {
                collisionManager.Bounce(ball);
                ball.SavePosition();
                return true;
            }
            return false;
        }
    }
}
