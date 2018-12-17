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

    public class Pad : IPad, IElement
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private int OffsetY { get; set; }
        

        public Pad()
        {
            OffsetY = 20;
            PosY = OffsetY;

        }

        public void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void GetSize(out int width, out int height)
        {
            width = Width;
            height = Height;
        }

        public void LogData()
        {
            Logger.Instance.Write(
                string.Format(
                    "Pad: {0}, {1}, {2}, {3}",
                    PosX,
                    PosY,
                    Width,
                    Height));
        }
    }
}
