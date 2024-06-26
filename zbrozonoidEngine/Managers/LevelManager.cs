﻿/*
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
namespace zbrozonoidEngine.Managers
{
    using System.Collections;
    using System.IO;
    using zbrozonoidEngine.Interfaces;

    public class LevelManager : ILevelManager
    {
        private readonly string LevelPath = "zbrozonoidAssets.Levels.";

        private readonly string[] levelNames = new string[] {"Level1.txt", "Level2.txt"};
        
        private ILevel level;

        private readonly IEnumerator index;

        public LevelManager()
        {
            index = GetEnumerator();
        }

        public bool Load()
        {
            var name = index.Current.ToString();
            if (Path.GetExtension(name) == ".txt")
            {
                level = new LevelTxt();
            }
            else
            {
                // there was also xml level loader but I've abadoned that idea
                //level = new Level();
            }
            return level.Load(LevelPath + name);
        }

        public ILevel GetCurrent()
        {
            return level;
        }

        public void MoveNext()
        {
            index.MoveNext();
        }

        public void Reset()
        {
            index.Reset();
        }

        public bool VerifyAllBricksAreHit()
        {
            if (level?.BeatableBricksNumber <= 0)
            {
                return true;
            }

            return false;
        }

        public void Restart()
        {
            Reset();
            MoveNext();
            Load();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return levelNames.GetEnumerator();
        }

    }
}
