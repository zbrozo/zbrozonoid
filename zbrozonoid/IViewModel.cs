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
using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace zbrozonoid
{
    public interface IViewModel : IDisposable
    {
        Font LoadFont(string name);

        List<Brick> Bricks { get; }
        Sprite Background { get; }
        Font Font { get; }

        Text Title { get; }
        Text GameOverMessage { get; }
        Text LiveAndScoresMessage { get; }
        Text PressButtonToPlayMessage { get; }
        Text FasterBallMessage { get; }
        Text FireBallMessage { get; }

        void PrepareBricksToDraw();
        void PrepareBackground(string backgroundName);
        Text PrepareMenuItem(string name, int number, bool isCurrent);

    }
}
