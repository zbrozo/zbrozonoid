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
    using System.Collections;
    using System.Collections.Generic;

    using zbrozonoidLibrary.Enumerators;
    using zbrozonoidLibrary.Interfaces;

    public class Tail : ITail
    {
        public int Count => positions.Count;

        public bool IsReadOnly { get; } = false;

        private readonly List<Position> positions = new List<Position>();

        private int max = 200;

        public void Add(Position position)
        {
            positions.Insert(0, position);
            if (positions.Count > max)
            {
                positions.RemoveRange(max, positions.Count - max);
            }
        }

        public void Clear()
        {
            positions.Clear();
        }

        public bool Remove(Position position)
        {
            return positions.Remove(position);
        }

        public bool Contains(Position position)
        {
            return positions.Contains(position);
        }

        public void CopyTo(Position[] positions, int arrayIndex)
        {
            this.positions.CopyTo(positions, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public IEnumerator<Position> GetEnumerator()
        {
            return new TailEnum(positions);
        }
    }
}
