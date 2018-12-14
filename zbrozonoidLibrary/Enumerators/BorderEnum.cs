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
    using System.Collections.Generic;
    using System.Collections;

    using zbrozonoidLibrary.Interfaces;

    public class BorderEnum : IEnumerator<IBorder>
    {
        public List<IBorder> Borders;

        int position = -1;

        public BorderEnum(List<IBorder> borders)
        {
            Borders = borders;
        }

        public bool MoveNext()
        {
            position++;
            return (position < Borders.Count);
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

        public IBorder Current
        {
            get
            {
                try
                {
                    return Borders[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
