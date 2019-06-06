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

    public class Border : IBorder
    {
        public Rectangle Boundary { get; set; } = new Rectangle();

        public Edge Type { get; set; }

        private int borderSize = 12;

        public Border(IScreen screen, Edge edge)
        {
            Type = edge;

            switch (edge)
            {
                case Edge.Bottom:
                    {
                        Boundary.Min = new Vector2(0, screen.Height - borderSize);
                        Boundary.Size = new Vector2(screen.Width, borderSize);
                        break;
                    }
                case Edge.Left:
                    {
                        Boundary.Min = new Vector2(0, 0);
                        Boundary.Size = new Vector2(borderSize, screen.Height);
                        break;
                    }
                case Edge.Right:
                    {
                        Boundary.Min = new Vector2(screen.Width - borderSize, 0);
                        Boundary.Size = new Vector2(borderSize, screen.Height);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
