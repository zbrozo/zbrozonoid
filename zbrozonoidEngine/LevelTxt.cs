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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using zbrozonoidEngine.Interfaces;

    public class LevelTxt : ILevel
    {

        public List<IBrick> Bricks { get; set; } = new List<IBrick>();

        public string BackgroundPath { get; set; }

        public int BeatableBricksNumber { get; set; }

        private const int Margin = 12;
        private const int BlockWidth = 50;
        private const int BlockHeight = 25;
        private const int MaxLineLength = 40;
        private const int BlocksAmountInLine = MaxLineLength/2;
        private const int BlocksAmountInColumn = 30;

        public List<IBrick> CreateNewBricks(List<IBrick> bricks, int offsetY)
        {
            List<IBrick> temporaryBricks = new List<IBrick>();

            foreach (var brick in bricks)
            {
                var tmpBrick = new Brick(0, 0, 0);

                Rectangle boundary = new Rectangle(
                    brick.Boundary.Min.X,
                    brick.Boundary.Min.Y + offsetY,
                    brick.Boundary.Size.X,
                    brick.Boundary.Size.Y);

                tmpBrick.Type = brick.Type;
                tmpBrick.IsHit = brick.IsHit;
                tmpBrick.ColorNumber = brick.ColorNumber;
                tmpBrick.Boundary = boundary;

                temporaryBricks.Add(tmpBrick);
            }

            return temporaryBricks;
        }

        public IBrick CreateBrick(string data, int x, int y)
        {
            var brick = new Brick(0, 0, 0);
            int ColorNumber = Convert.ToInt32(data.Substring(x * 2, 1), 16);
            int Type = Convert.ToInt32(data.Substring(x * 2 + 1, 1), 16);
            Rectangle Boundary = new Rectangle(x * BlockWidth + Margin, y * BlockHeight, BlockWidth, BlockHeight);

            brick.Type = (BrickType)Type;
            brick.ColorNumber = ColorNumber;
            brick.Boundary = Boundary;
            brick.IsHit = false;

            return brick;
        }

        public bool Load(string fileName)
        {
            List<IBrick> tempBricks = new List<IBrick>();

            BeatableBricksNumber = 0;

            AssemblyName assemblyName = new AssemblyName(@"zbrozonoidAssets");
            Assembly assembly = Assembly.Load(assemblyName);

            using (Stream resourceStream = assembly.GetManifestResourceStream(fileName))
            {
                if (resourceStream == null)
                {
                    return false;
                }

                var reader = new StreamReader(resourceStream);

                int y = 0;
                string data = "";

                while(reader.Peek() >= 0)
                { 
                    data = reader.ReadLine();

                    if (data.Length == MaxLineLength)
                    {
                        for (int x = 0; x < BlocksAmountInLine; x++)
                        {
                            IBrick brick = CreateBrick(data, x, y);

                            if (brick.IsBeatable)
                            {
                                BeatableBricksNumber++;
                            }

                            tempBricks.Add(brick);
                        }

                        ++y;
                    }
                }

                if (data.Length > 0)
                {
                    BackgroundPath = data;
                }

                Bricks = CreateNewBricks(tempBricks, (BlocksAmountInColumn - y) / 2 * BlockHeight);
            }
            return true;
        }
    }
}
