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
using zbrozonoid.Views;
using zbrozonoidLibrary.Interfaces;

namespace zbrozonoid
{
    public class ViewStateMachine
    {
        private Window window;
        private IViewModel viewModel;

        private IDrawGameObjects draw;

        private IGameView menuState;
        private IGameView playState;
        private IGameView gameOverState;

        private IGameView currentState;

        public ViewStateMachine(Window window, IViewModel viewModel, IDrawGameObjects draw)
        {
            menuState = new GameBeginView(draw);
            playState = new GamePlayView(draw);
            gameOverState = new GameOverView(draw);

            this.window = window;
            this.draw = draw;
            this.viewModel = viewModel;
        }

        public void Action()
        {
            draw.DrawBackground(viewModel.Background);
            draw.DrawBorders();
            draw.DrawBricks(viewModel.Bricks);
            draw.DrawPad();
            draw.DrawBall();

            if (currentState != null)
            {
                currentState.Action();
            }
        }

        public void Transitions(IGame game)
        {
            if (!game.GameState.ShouldGo)
            {
                game.StartPlay();
                gotoPlay();
            }


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
