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
    using zbrozonoidLibrary.Interfaces;

    public class Brick : IBrick, IElement
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ColorNumber { get; set; }
        public BrickType Type { get; set; }
        public bool Hit { get; set; }

        public Brick(BrickType type, int x, int y, int width = 50, int height = 25)
        {
            PosX = x;
            PosY = y;
            Width = width;
            Height = height;
            Type = type;
            Hit = false;
        }

        public bool IsBeatable()
        {
            if (Type != BrickType.Solid)
            {
                return true;
            }
            return false;
        }

        public bool IsVisible()
        {
            if (Type != BrickType.None)
            {
                return true;
            }

            return false;
        }
    }
}
