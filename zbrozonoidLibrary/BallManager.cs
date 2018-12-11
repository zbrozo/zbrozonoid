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
namespace zbrozonoidLibrary
{
    using System.Collections.Generic;

    using zbrozonoidLibrary.Interfaces;

    public class BallManager : IBallManager
    {
        private readonly List<IBall> balls = new List<IBall>();
        private int Index { get; set; }

        public void Add(IBall ball)
        {
            balls.Add(ball);
        }

        public IBall First()
        {
            Index = 0;
            return GetCurrent();
        }

        public IBall Next()
        {
            if (Index >= balls.Count)
            {
                return null;
            }

            ++Index;

            return GetCurrent();
        }

        public bool IsLast()
        {
            if (Index >= balls.Count)
            {
                return true;
            }
            return false;
        }

        public IBall GetCurrent()
        {
            if (balls.Count == 0 || Index >= balls.Count)
            {
                return null;
            }

            return balls[Index];
        }

        public void LeaveOnlyOne()
        {
            if (balls.Count > 1)
            {
                balls.RemoveRange(1, balls.Count - 1);
            }
        }
    }
}
