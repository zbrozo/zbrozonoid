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

using Autofac;
using zbrozonoid.Views;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{
    public class ViewStateMachine : IViewStateMachine
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private IGame game;
        private IViewModel viewModel;

        private IDrawGameObjects draw;

        private IView currentState;
        private IContainer container;

        public bool IsMenuState => currentState is GameBeginView;

        public ViewStateMachine(IGame game, IViewModel viewModel, IView menuView, IDrawGameObjects draw)
        {
            this.game = game;
            this.draw = draw;
            this.viewModel = viewModel;
        }


        public void Initialize(IContainer container)
        {
            this.container = container;
            GotoMenu();
        }

        public void Action()
        {
            //draw.DrawBackground(viewModel.Background);
            //draw.DrawBorders();
            //draw.DrawBricks(viewModel.Bricks);
            //draw.DrawPad();
            //draw.DrawBall();

            currentState?.Display();
        }

        public void Transitions(IGame game)
        {
            if (currentState is GameBeginView)
            {
                Logger.Info("State: Begin -> PlayGame");
                currentState = container.Resolve<GamePlayView>();
                game.StartPlay();
                return;
            }

            if (currentState is GamePlayView 
                && game.GameState.Lifes < 0
                && !game.GameState.Pause)
            {
                Logger.Info("State: PlayGame -> GameOver");
                currentState = container.Resolve<GameOverView>();
                return;
            }

            if (currentState is GamePlayView 
                && game.GameState.Lifes >= 0 
                && !game.GameState.Pause)
            {
                Logger.Info("State: PlayGame -> GameOver");

                currentState = container.Resolve<StartPlayView>();
                return;
            }

            if (currentState is GamePlayView 
                && game.GameState.Pause)
            {
                Logger.Info("State: PlayGame -> StopPlay");

                currentState = container.Resolve<StopPlayView>();
                return;
            }

            if (currentState is StopPlayView
                && game.GameState.Lifes < 0
                && game.GameState.Pause)
            {
                Logger.Info("State: StopPlay -> GameOver");

                currentState = container.Resolve<GameOverView>(); 
                return;
            }

            if (currentState is StopPlayView
                && !game.GameState.Pause)
            {
                Logger.Info("State: StopPlay -> GamePlay");

                currentState = container.Resolve<GamePlayView>();
                return;
            }

            if (currentState is StartPlayView)
            {
                Logger.Info("State: StartPlay -> GamePlay");

                currentState = container.Resolve<GamePlayView>();
                game.StartPlay();
                return;
            }

            if (currentState is GameOverView)
            {
                Logger.Info("State: GameOver -> Begin");

                game.GameIsOver();

                currentState = container.Resolve<GameBeginView>();
                return;
            }
        }

        public void GotoMenu()
        {
            currentState = container.Resolve<GameBeginView>();
        }

        public void GotoPlay()
        {
            currentState = container.Resolve<GamePlayView>();
        }

        public void GotoGameOver()
        {
            currentState = container.Resolve<GameOverView>();
        }

    }
}
