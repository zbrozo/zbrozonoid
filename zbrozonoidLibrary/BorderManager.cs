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

    public class BorderManager
    {
        private List<IBorder> Borders = new List<IBorder>();

        private int Index = 0;

        public void Create(IScreen screen)
        {
            Border border1 = new Border(screen, Edge.Bottom);
            Borders.Add(border1);
            Border border2 = new Border(screen, Edge.Left);
            Borders.Add(border2);
            Border border3 = new Border(screen, Edge.Right);
            Borders.Add(border3);
        }

        public IBorder First()
        {
            Index = 0;
            return GetCurrent();
        }

        public IBorder Next()
        {
            if (Index >= Borders.Count)
            {
                return null;
            }

            ++Index;

            return GetCurrent();
        }

        public bool IsLast()
        {
            if (Index >= Borders.Count)
            {
                return true;
            }
            return false;
        }

        public IBorder GetCurrent()
        {
            if (Borders.Count == 0 || Index >= Borders.Count)
            {
                return null;
            }

            return Borders[Index];
        }

    }
}
