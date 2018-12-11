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

    public class Border : IBorder, IElement
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Edge Type { get; set; }

        private int borderSize = 12;

        public Border(IScreen screen, Edge edge)
        {
            Type = edge;

            switch (edge)
            {
                case Edge.Bottom:
                    {
                        PosX = 0;
                        PosY = screen.Height - borderSize;
                        Width = screen.Width;
                        Height = borderSize;
                        break;
                    }
                case Edge.Left:
                    {
                        PosX = 0;
                        PosY = 0;
                        Width = borderSize;
                        Height = screen.Height;
                        break;
                    }
                case Edge.Right:
                    {
                        PosX = screen.Width - borderSize;
                        PosY = 0;
                        Width = borderSize;
                        Height = screen.Height;
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
