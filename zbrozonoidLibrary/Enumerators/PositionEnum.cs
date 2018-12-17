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

    public class PositionEnum : IEnumerator<Position>
    {
        private readonly List<Position> positions;

        int position = -1;

        public PositionEnum(List<Position> positions)
        {
            this.positions = positions;
        }

        public bool MoveNext()
        {
            position++;
            return position < positions.Count;
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

        public Position Current
        {
            get
            {
                try
                {
                    return positions[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
