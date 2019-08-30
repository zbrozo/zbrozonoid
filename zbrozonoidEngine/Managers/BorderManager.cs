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

        using zbrozonoidEngine.Interfaces;

    public class BorderManager : IBorderManager
    {
        public int Count => borders.Count;

        public bool IsReadOnly { get; } = false;

        private readonly List<IBorder> borders = new List<IBorder>();

        public void Create(IScreen screen, IGameConfig config)
        {
            borders.Clear();

            Border borderLeft = new Border(screen, Edge.Left);
            borders.Add(borderLeft);
            Border borderRight = new Border(screen, Edge.Right);
            borders.Add(borderRight);

            if (config.Players <= 1)
            {
                Border border = new Border(screen, Edge.Top);
                borders.Add(border);
            }

            if (config.Players <= 0)
            {
                Border border = new Border(screen, Edge.Bottom);
                borders.Add(border);
            }
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
            return borders.GetEnumerator(); 
        }
    }
}
