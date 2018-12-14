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
namespace zbrozonoidLibrary.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IGame
    {
        event EventHandler<BackgroundEventArgs> OnChangeBackground;

        bool ShouldGo { get; set; }

        int Lives { get; set; }

        void Initialize();

        void SetScreenSize(int width, int height);

        void GetScreenSize(out int width, out int height);

        IBallManager GetBallManager();

        IBorderManager GetBorderManager();

        void GetPadPosition(out int posx, out int posy);

        void GetPadSize(out int width, out int height);

        void Action();

        void SetPadMove(int delta);

        void SetBallMove();

        void SetPadMinPosition();

        void SetPadMaxPosition();

        List<IBrick> GetBricks();

        string GetBackgroundPath();

        void StartPlay();

    }
}
