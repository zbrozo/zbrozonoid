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
    using NLog;
    using zbrozonoidEngine.Interfaces;

    public class Pad : IPad
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Rectangle Boundary { get; set; } = new Rectangle();
        private int OffsetY { get; set; }

        public Pad()
        {
            OffsetY = 20;
            Boundary.Min = new Vector2(0, OffsetY);
        }

        public void SetSize(int width, int height)
        {
            Boundary.Size = new Vector2(width, height);
        }

        public void GetSize(out int width, out int height)
        {
            width = Boundary.Size.X;
            height = Boundary.Size.Y;
        }

        public void LogData()
        {
            Logger.Info(
                string.Format(
                    "Pad: {0}, {1}, {2}, {3}",
                    Boundary.Min.X,
                    Boundary.Min.Y,
                    Boundary.Size.X,
                    Boundary.Size.Y));
        }
    }
}
