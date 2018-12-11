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

    public class Tail : ITail
    {
        private readonly List<Position> positions = new List<Position>();

        private readonly IContainer<Position> tail;

        private int max = 200;

        public Tail()
        {
            tail = new Container<Position>(positions);
        }

        public void Add(Position position)
        {
            positions.Insert(0, position);
            if (positions.Count > max)
            {
                positions.RemoveRange(max, positions.Count - max);
            }
        }

        public IContainer<Position> Get()
        {
            return tail;
        }
    }
}
