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
    using System.Collections;
    using System.Collections.Generic;

    using zbrozonoidLibrary.Enumerators;
    using zbrozonoidLibrary.Interfaces;

    public class BallManager : IBallManager
    {
        public int Count => balls.Count;

        public bool IsReadOnly { get; } = false;

        private readonly List<IBall> balls = new List<IBall>();

        public void Add(IBall ball)
        {
            balls.Add(ball);
        }

        public void Clear()
        {
            balls.Clear();
        }

        public bool Contains(IBall ball)
        {
            return balls.Contains(ball);
        }

        public void CopyTo(IBall[] ballsArray, int arrayIndex)
        {
            balls.CopyTo(ballsArray, arrayIndex);
        }

        public bool Remove(IBall item)
        {
            return balls.Remove(item);
        }

        public IBall GetFirst()
        {
            if (Count > 0)
            {
                return balls[0];
            }

            return null;
        }

        public void LeaveOnlyOne()
        {
            if (balls.Count > 1)
            {
                balls.RemoveRange(1, balls.Count - 1);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public IEnumerator<IBall> GetEnumerator()
        {
            return new BallEnum(balls);
        }
    }
}
