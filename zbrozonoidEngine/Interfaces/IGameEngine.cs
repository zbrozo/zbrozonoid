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
namespace zbrozonoidEngine.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using zbrozonoidEngine.Counters;

    public interface IGameEngine
    {
        // external events to connect to GUI
        event EventHandler<LevelEventArgs> OnChangeLevelEvent;
        event EventHandler<BrickHitEventArgs> OnBrickHitEvent;
        event EventHandler<EventArgs> OnLostBallEvent;
        event EventHandler<EventArgs> OnLevelCompletedEvent;

        ILifetimeScope ManagerScope { get; }

        bool ForceChangeLevel { get; set; }

        FastBallCounter FastBallCounter { get; }
        FireBallCounter FireBallCounter { get; }

        IGameState GameState { get; }
        IGameConfig GameConfig { get; }

        ICollection<IBrick> Bricks { get; }

        void SetPadMove(int delta, uint manipulator);

        void Initialize();
        void InitPlay(int[] manipulators);
        void StartPlay();
        void StopPlay();

        void GetScreenSize(out int width, out int height);

        void Action(); // main loop
    }
}
