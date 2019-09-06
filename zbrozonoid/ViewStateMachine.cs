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
        private readonly IView stopPlay;

        private IView currentState;

        public bool IsMenuState => currentState is GameBeginView;

        public ViewStateMachine(IViewModel viewModel, IView menuView, IDrawGameObjects draw)
        {
            gameBegin = new GameBeginView(draw, menuView);
            gamePlay = new GamePlayView(draw);
            gameOver = new GameOverView(draw);
            startPlay = new StartPlayView(draw);
            stopPlay = new StopPlayView(draw);

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

            if (currentState is GamePlayView 
                && game.GameState.Lifes < 0
                && !game.GameState.Pause)
            {
                currentState = gameOver;
                return;
            }

            if (currentState is GamePlayView 
                && game.GameState.Lifes >= 0 
                && !game.GameState.Pause)
            {
                currentState = startPlay;
                return;
            }

            if (currentState is GamePlayView 
                && game.GameState.Pause)
            {
                currentState = stopPlay;
                return;
            }

            if (currentState is StopPlayView
                && game.GameState.Lifes < 0
                && game.GameState.Pause)
            {
                currentState = gameOver;
                return;
            }

            if (currentState is StopPlayView
                && !game.GameState.Pause)
            {
                currentState = gamePlay;
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
                game.GameIsOver();

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
