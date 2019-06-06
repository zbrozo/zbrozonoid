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
namespace zbrozonoidLibrary.Enumerators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using zbrozonoidLibrary.Interfaces;

    public class BallEnum : IEnumerator<IBall>
    {
        public List<IBall> Balls;

        int position = -1;

        public BallEnum(List<IBall> balls)
        {
            Balls = balls;
        }
        
        public bool MoveNext()
        {
            position++;
            return (position < Balls.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        void IDisposable.Dispose() { }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public IBall Current
        {
            get
            {
                try
                {
                    return Balls[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
