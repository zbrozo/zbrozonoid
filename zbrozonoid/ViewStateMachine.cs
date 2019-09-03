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
        private IViewModel viewModel;

        private IDrawGameObjects draw;

        private readonly IView gameBegin;
        private readonly IView gamePlay;
        private readonly IView gameOver;
        private readonly IView startPlay;

        private IView currentState;

        public bool IsMenuState => currentState is GameBeginView;

        private ViewCommon viewCommon;

        public ViewStateMachine(ViewCommon viewCommon, IViewModel viewModel, IView menuView, IDrawGameObjects draw)
        {
            this.viewCommon = viewCommon;

            gameBegin = new GameBeginView(viewCommon, menuView);
            gamePlay = new GamePlayView(draw);
            gameOver = new GameOverView(draw);
            startPlay = new StartPlayView(draw);

            this.draw = draw;
            this.viewModel = viewModel;
        }

        public void Action()
        {
            draw.DrawBackground(viewCommon.Background);
            draw.DrawBorders();
            draw.DrawBricks(viewModel.Bricks);
            draw.DrawPad();
            draw.DrawBall();

            currentState?.Display();
        }

        public void Transitions(IGame game)
        {
            if (currentState is GameBeginView)
            {
                currentState = gamePlay;
                game.StartPlay();
                return;
            }

            if (currentState is GamePlayView && game.GameState.Lifes < 0)
            {
                currentState = gameOver;
                return;
            }

            if (currentState is GamePlayView && game.GameState.Lifes >= 0)
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

        public void GotoMenu()
        {
            currentState = gameBegin;
        }

        public void GotoPlay()
        {
            currentState = gamePlay;
        }

        public void GotoGameOver()
        {
            currentState = gameOver;
        }

    }
}
