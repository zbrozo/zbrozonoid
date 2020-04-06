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
    using System.Collections;
    using System.Collections.Generic;
    using zbrozonoidEngine.Enumerators;
    using zbrozonoidEngine.Interfaces;

    public class BallManager : IBallManager
    {
        public int Count => balls.Count;

        public bool IsReadOnly { get; } = false;

        private readonly Dictionary<IBall, IPad> balls = new Dictionary<IBall, IPad>();

        public void Add(IBall ball, IPad pad)
        {
            balls.Add(ball, pad);
        }

        public void Clear()
        {
            balls.Clear();
        }
        /*
        public bool Contains(KeyValuePair<IBall, IPad> ball)
        {
            //return balls.Contains(ball);
            return true;
        }

        public void CopyTo(KeyValuePair<IBall, IPad>[] ballsArray, int arrayIndex)
        {
            //balls.CopyTo(ballsArray, arrayIndex);
        }

        public bool Remove(KeyValuePair<IBall, IPad> item)
        {
            //return balls.Remove(item);
            return false;
        }

        public IBall GetFirst()
        {
            if (Count > 0)
            {
                return balls[0];
            }

            return null;
        }
        */

        public void Remove(IBall ball)
        {
            balls.Remove(ball);
        }

        public IPad GetPadAssignedToBall(IBall ball)
        {
            return balls[ball];
        }

        /*
        public void LeaveOnlyOne()
        {
            if (balls.Count > 1)
            {
                Dictionary<IBall, IPad>.KeyCollection b = balls.Keys;
                var i = b.GetEnumerator();
                i.MoveNext();
                IBall ball = i.Current;
                IPad pad = balls[ball];

                balls.Clear();
                balls.Add(ball, pad);

                //balls.RemoveRange(1, balls.Count - 1);
            }
        }
        */

        public IEnumerator<IBall> GetEnumerator()
        {
            return balls.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return balls.Keys.GetEnumerator();
        }
    }
}
