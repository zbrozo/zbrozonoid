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
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.Managers
{

    public class BorderCollisionManager : IBorderCollisionManager
    {
        private ICollisionManager collisionManager;

        public BorderCollisionManager(ICollisionManager collisionManager)
        {
            this.collisionManager = collisionManager;
        }

        private bool DetectAndVerify(IBorder border, IPad pad)
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

        public bool DetectAndVerify(IEnumerable<IBorder> borders, IPad pad)
        {
            foreach (IBorder border in borders)
            {
                if (DetectAndVerify(border, pad))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Detect(IBorder border, IBall ball)
        {
            return collisionManager.Detect(border, ball);
        }

        public void Bounce(IReadOnlyCollection<IBrick> bricksHitList, IBorder border, IBall ball)
        {
            // TODO
            //collisionManager.Bounce(bricksHitList, border, ball);
            ball.SavePosition();
        }
    }
}
