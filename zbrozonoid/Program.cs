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

namespace zbrozonoid
{
    using zbrozonoidEngine;
    using zbrozonoidEngine.Interfaces;
    //using ManyMouseWrapper;
    using NLog;
    using ManyMouseSharp;

    static class Program
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            config.AddRule(LogLevel.Debug, LogLevel.Debug, logfile);
            LogManager.Configuration = config;

            Logger.Info("Zbrozonoid starts");

            int number = ManyMouse.Init();
            if (number == 0)
            {
                return;
            }

            /*
            ManyMouse manyMouse = new ManyMouse();
            int number = manyMouse.Init();
            if (number == 0)
            {
                return;
            }
            */
            IGame game = new Game(number);
            Window window = new Window(game/*, manyMouse*/);

            game.OnChangeLevel += window.OnChangeLevel;
            game.OnBrickHit += window.OnBrickHit;
            game.OnLostBallsEvent += window.OnLostBalls;

            game.Initialize();
            window.Initialize();

            window.Run();

            Logger.Info("Zbrozonoid quits");

            LogManager.Shutdown();
        }
    }
}
