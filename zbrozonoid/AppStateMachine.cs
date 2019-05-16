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

namespace zbrozonoid
{
    public class AppStateMachine
    {
        private Window window;
        private IDrawGameObjects draw;

        public AppStateMachine(Window window, IDrawGameObjects draw)
        {
            this.window = window;
            this.draw = draw;
        }

        public void Action()
        {
            draw.DrawBackground(window.background);
            draw.DrawBorders();
            draw.DrawBricks(window.bricksToDraw);
            draw.DrawPad();
            draw.DrawBall();
            draw.DrawTexts();
        }

    }
}
