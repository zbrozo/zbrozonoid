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
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{
    public class ViewStateMachine
    {
        private Window window;
        private IViewModel viewModel;

        private IDrawGameObjects draw;

        private IGameView gameBegin;
        private IGameView gamePlay;
        private IGameView gameOver;
        private IGameView startPlay;

        private IGameView currentState;

        public ViewStateMachine(Window window, IViewModel viewModel, IDrawGameObjects draw)
        {
            gameBegin = new GameBeginView(draw);
            gamePlay = new GamePlayView(draw);
            gameOver = new GameOverView(draw);
            startPlay = new StartPlayView(draw);

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
            if (currentState is GameBeginView)
            {
                currentState = gamePlay;
                game.StartPlay();
                return;
            }

            if (currentState is GamePlayView && game.GameState.Lives < 0)
            {
                currentState = gameOver;
                return;
            }

            if (currentState is GamePlayView && game.GameState.Lives >= 0)
            {
                currentState = startPlay;
                return;
            }

            if (currentState is StartPlayView)
            {
                currentState = gamePlay;
                game.StartPlay();
                return;
            }

            if (currentState is GameOverView)
            {
                currentState = gameBegin;
                return;
            }
        }

        public void gotoMenu()
        {
            currentState = gameBegin;
        }

        public void gotoPlay()
        {
            currentState = gamePlay;
        }

        public void gotoGameOver()
        {
            currentState = gameOver;
        }

    }
}
