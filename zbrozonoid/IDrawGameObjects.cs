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
using zbrozonoid.Views;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{
    public interface IDrawGameObjects
    {

        RenderWindow Render { get; }
        IPrepareTextLine PrepareTextLine { get; }
        IGame game { get; }

        void DrawBackground(Sprite background);
        //void DrawBorders();
        //void DrawBricks(List<Brick> bricksToDraw);
        void DrawBall();
        void DrawPad();
        //void DrawTitle();

        //void DrawLifesAndScoresInfo();
        void DrawGameOver();
        void DrawPressPlayToPlay();
        //void DrawFasterBallTimer();
        //void DrawFireBallTimer();
        void DrawStopPlayMessage();
    }
}
