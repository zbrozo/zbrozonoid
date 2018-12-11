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

    public class Container<T> : IContainer<T> where T : struct
    {
        private int Index { get; set; }

        private readonly List<T> list;

        public Container(List<T> list)
        {
            this.list = list;
        }

        public T? First()
        {
            Index = 0;
            return GetCurrent();
        }

        public T? Next() 
        {
            if (Index >= list.Count)
            {
                return null;
            }

            ++Index;

            return GetCurrent();
        }

        public bool IsLast()
        {
            if (Index >= list.Count)
            {
                return true;
            }
            return false;
        }

        public T? GetCurrent()
        {
            if (list.Count == 0 || Index >= list.Count)
            {
                return null;
            }

            return list[Index];
        }

    }
}
