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

    public class BorderManager : IBorderManager
    {
        public int Count => borders.Count;

        public bool IsReadOnly { get; } = false;

        private readonly List<IBorder> borders = new List<IBorder>();

        public void Create(IScreen screen)
        {
            Border border1 = new Border(screen, Edge.Bottom);
            borders.Add(border1);
            Border border2 = new Border(screen, Edge.Left);
            borders.Add(border2);
            Border border3 = new Border(screen, Edge.Right);
            borders.Add(border3);
        }

        public void Add(IBorder border)
        {
            borders.Add(border);
        }

        public void Clear()
        {
            borders.Clear();
        }

        public bool Contains(IBorder border)
        {
            return borders.Contains(border);
        }

        public void CopyTo(IBorder[] ballsArray, int arrayIndex)
        {
            borders.CopyTo(ballsArray, arrayIndex);
        }

        public bool Remove(IBorder item)
        {
            return borders.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IBorder> GetEnumerator()
        {
            return new BorderEnum(borders);
        }
    }
}
