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
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    using zbrozonoidEngine.Interfaces;

    public class LevelTxt : ILevel
    {

        public List<IBrick> Bricks { get; set; } = new List<IBrick>();

        public string BackgroundPath { get; set; }

        public int BeatableBricksNumber { get; set; }

        private const int Margin = 12;

        private const int BlockWidth = 50;

        private const int BlockHeight = 25;

        private const int StartY = 150;

        public bool Load(string fileName)
        {
            Bricks.Clear();
            BeatableBricksNumber = 0;

            AssemblyName assemblyName = new AssemblyName(@"zbrozonoidAssets");
            Assembly assembly = Assembly.Load(assemblyName);

            using (Stream resourceStream = assembly.GetManifestResourceStream(fileName))
            {
                if (resourceStream == null)
                    return false;

                var reader = new StreamReader(resourceStream);

                int y = 0;
                while(reader.Peek() >= 0)
                { 
                    string data = reader.ReadLine();

                    if (data.Length == 40)
                    {
                        for (int x = 0; x < 20; x++)
                        {
                            var brick = new Brick(0, 0, 0);
                            int ColorNumber = Convert.ToInt32(data.Substring(x*2, 1), 16);
                            int Type = Convert.ToInt32(data.Substring(x*2 + 1, 1), 16);
                            Rectangle Boundary = new Rectangle(x * BlockWidth + Margin, y * BlockHeight + StartY, BlockWidth, BlockHeight);

                            brick.Type = (BrickType)Type;
                            brick.ColorNumber = ColorNumber;
                            brick.Boundary = Boundary;
                            brick.IsHit = false;

                            if (brick.IsBeatable)
                            {
                                BeatableBricksNumber++;
                            }
                            Bricks.Add(brick);
                        }

                        ++y;
                    }
                    else
                    {
                        BackgroundPath = data;
                    }
                }
            }
            return true;
        }
    }
}
