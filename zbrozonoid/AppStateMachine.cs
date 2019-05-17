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
using zbrozonoid.States;

namespace zbrozonoid
{
    public class AppStateMachine
    {
        private Window window;
        private IDrawGameObjects draw;

        private AppInMenuState menuState;
        private AppInPlayState playState;
        private AppInGameOverState gameOverState;

        private IAppState currentState;

        public AppStateMachine(Window window, IDrawGameObjects draw)
        {
            menuState = new AppInMenuState();
            playState = new AppInPlayState();
            gameOverState = new AppInGameOverState();

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

            currentState.Action();
        }

        public void gotoMenu()
        {
            currentState = menuState;
        }

        public void gotoPlay()
        {
            currentState = playState;
        }

        public void gotoGameOver()
        {
            currentState = gameOverState;
        }

    }
}
