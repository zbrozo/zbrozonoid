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
using zbrozonoid.Views.Interfaces;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{
    public class ViewStateMachine : IViewStateMachine
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private IGame game;

        private IView currentState;
        private IContainer container;

        public bool IsMenuState => currentState is IGameBeginView;

        public ViewStateMachine(IGame game)
        {
            this.game = game;
        }

        public void Initialize(IContainer container)
        {
            this.container = container;
            GotoMenu();
        }

        public void Action()
        {
            currentState?.Display();
        }

        public void Transitions(IGame game)
        {
            if (currentState is IGameBeginView)
            {
                Logger.Info("State: Begin -> PlayGame");
                currentState = container.Resolve<IGamePlayView>();
                game.StartPlay();
                return;
            }

            if (currentState is IGamePlayView 
                && game.GameState.Lifes < 0
                && !game.GameState.Pause)
            {
                Logger.Info("State: PlayGame -> GameOver");
                currentState = container.Resolve<IGameOverView>();
                return;
            }

            if (currentState is IGamePlayView 
                && game.GameState.Lifes >= 0 
                && !game.GameState.Pause)
            {
                Logger.Info("State: PlayGame -> GameOver");

                currentState = container.Resolve<IStartPlayView>();
                return;
            }

            if (currentState is IGamePlayView 
                && game.GameState.Pause)
            {
                Logger.Info("State: PlayGame -> StopPlay");

                currentState = container.Resolve<IStopPlayView>();
                return;
            }

            if (currentState is IStopPlayView
                && game.GameState.Lifes < 0
                && game.GameState.Pause)
            {
                Logger.Info("State: StopPlay -> GameOver");

                currentState = container.Resolve<IGameOverView>(); 
                return;
            }

            if (currentState is IStopPlayView
                && !game.GameState.Pause)
            {
                Logger.Info("State: StopPlay -> GamePlay");

                currentState = container.Resolve<IGamePlayView>();
                return;
            }

            if (currentState is IStartPlayView)
            {
                Logger.Info("State: StartPlay -> GamePlay");

                currentState = container.Resolve<IGamePlayView>();
                game.StartPlay();
                return;
            }

            if (currentState is IGameOverView)
            {
                Logger.Info("State: GameOver -> Begin");

                game.GameIsOver();

                currentState = container.Resolve<IGameBeginView>();
                return;
            }
        }

        public void GotoMenu()
        {
            currentState = container.Resolve<IGameBeginView>();
        }

        public void GotoPlay()
        {
            currentState = container.Resolve<IGamePlayView>();
        }

        public void GotoGameOver()
        {
            currentState = container.Resolve<IGameOverView>();
        }

    }
}
