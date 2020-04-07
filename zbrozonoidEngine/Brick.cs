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
    using zbrozonoidEngine.Interfaces;

    public class Brick : IBrick
    {
        public Rectangle Boundary { get; set; } = new Rectangle();

        public int ColorNumber { get; set; }
        public BrickType Type { get; set; }

        public bool IsHit { get; set; }

        public bool IsBeatable
        {
            get
            {
                if (Type != BrickType.Solid && Type != BrickType.None)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsVisible 
        {
            get
            {
                if (Type != BrickType.None)
                {
                    return true;
                }
                return false;
            }
        }

        public Brick(BrickType type, int x, int y, int width = 50, int height = 25)
        {
            Boundary.Min = new Vector2(x, y);
            Boundary.Size = new Vector2(width, height);
            Type = type;
            IsHit = false;
        }

    }
}
