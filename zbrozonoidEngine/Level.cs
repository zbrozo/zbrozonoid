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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
	using System.Xml;
    using System.Xml.Linq;

    using zbrozonoidLibrary.Interfaces;

    public class Level : ILevel
    {
        private XDocument levelXml = new XDocument();

        public List<IBrick> Bricks { get; set; } = new List<IBrick>();

        public string BackgroundPath { get; set; }

        public int BeatableBricksNumber { get; set; }

        private const int Margin = 12;

        private const int BlockWidth = 50;

        private const int BlockHeight = 25;

        private const int StartY = 250;

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

                levelXml = XDocument.Load(resourceStream, LoadOptions.SetLineInfo);

                var levelElement = levelXml.Element("Level");
                if (levelElement == null)
                {
                    return false;
                }

                var backgroundElement = levelElement?.Element("Background");
                if (backgroundElement != null)
                {
                    var fileElement = backgroundElement?.Attribute("file");
                    BackgroundPath = fileElement?.Value;
                }

                var rowsElement = levelElement?.Element("Rows");
                if (rowsElement == null)
                {
                    return false;
                }

                if (!rowsElement.HasElements)
                {
                    return false;
                }

                var rows = rowsElement.Descendants();

                foreach (XElement rowElement in rows)
                {
                    XName name = rowElement.Name;
                    if (name != "Row")
                    {
                        continue;
                    }

                    var rowId = rowElement?.Attribute("id");
                    int rowNumber = Convert.ToInt32(rowId?.Value);

                    var blocksElement = rowElement?.Element("Bricks");
                    if (blocksElement == null)
                    {
                        return false;
                    }

                    var blockElements = blocksElement.DescendantsAndSelf();
                    if (!blocksElement.HasElements)
                    {
                        return false;
                    }

                    Bricks = Bricks.Concat(ParseBlocks(blockElements, rowNumber)).ToList();
                }
            }

            return true;
        }

        private List<IBrick> ParseBlocks(IEnumerable<XElement> blockElements, int rowNumber)
        {
            var blocks = blockElements.Elements("Brick").Select(d => new Brick(0,0,0)
            {
                ColorNumber = d.Attribute("color")?.Value != "" ? Convert.ToInt32(d.Attribute("color")?.Value) : 0,
                Type = d.Attribute("type")?.Value != "" ? (BrickType) Convert.ToInt32(d.Attribute("type")?.Value) : 0,
                Boundary = new Rectangle(
                    Convert.ToInt32(d.Attribute("column")?.Value) * BlockWidth + Margin,
                    rowNumber * BlockHeight + StartY,
                    BlockWidth,
                    BlockHeight),
                Hit = false
            }).ToList();

            List<IBrick> result = new List<IBrick>();
            foreach (var block in blocks)
            {
                if (block.IsBeatable())
                {
                    BeatableBricksNumber++;
                }

                result.Add(block);
            }

            return result;
        }

    }

}
