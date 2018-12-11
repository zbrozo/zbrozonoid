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

    public class LevelManager : ILevelManager
    {

        private string LevelPath = "zbrozonoidAssets.Levels.";

        private readonly string[] levelNames = new string[] {"Level1.xml", "Level2.xml"};

        private int levelNr;

        private readonly ILevel level = new Level();

        public bool First()
        {
            levelNr = 0;


            return level.Load(LevelPath + levelNames[levelNr]);
        }

        public bool Next()
        {
            ++levelNr;
            if (levelNr >= levelNames.Length)
            {
                Logger.Instance.Write("Level goes from first");
                return First();
            }

            return level.Load(LevelPath + levelNames[levelNr]);
        }

        public ILevel GetCurrent()
        {
            return level;
        }

        public bool VerifyAllBricksAreHit()
        {
            if (level?.BeatableBricksNumber <= 0)
            {
                return true;
            }

            return false;
        }

    }
}
