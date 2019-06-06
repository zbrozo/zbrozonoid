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
namespace zbrozonoidEngine
{
    using System.Collections;
    using System.Collections.Generic;
    using zbrozonoidEngine.Interfaces;

    public class Tail : ITail
    {
        public int Count => positions.Count;

        public bool IsReadOnly { get; } = false;

        private readonly List<Vector2> positions = new List<Vector2>();

        private int max = 200;

        public void Add(Vector2 position)
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

        public bool Remove(Vector2 position)
        {
            return positions.Remove(position);
        }

        public bool Contains(Vector2 position)
        {
            return positions.Contains(position);
        }

        public void CopyTo(Vector2[] positions, int arrayIndex)
        {
            this.positions.CopyTo(positions, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Vector2> GetEnumerator()
        {
            return positions.GetEnumerator();
        }
    }
}
