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
using zbrozonoid.Views.Interfaces;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{
    public class ViewStateMachine : IViewStateMachine
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private IView currentState;
        private ILifetimeScope scope;

        public bool IsMenuState => currentState is IGameBeginView;
        public bool IsGameOverState => currentState is IGameOverView;
        public bool IsPlayState => currentState is IGamePlayView;
        public bool IsStopState => currentState is IStopPlayView;
        public bool IsStartState => currentState is IStartPlayView;

        public void Initialize(ILifetimeScope scope)
        {
            this.scope = scope;
            currentState = scope.Resolve<IGameBeginView>();
        }

        public void Action()
        {
            currentState?.Display();
        }

        public void Transitions(IGameState gameState)
        {
            if (currentState is IGameBeginView)
            {
                Logger.Info("State: Begin -> PlayGame");
                currentState = scope.Resolve<IGamePlayView>();
                return;
            }

            if (currentState is IGamePlayView 
                && gameState.Lifes < 0
                && !gameState.Pause)
            {
                Logger.Info("State: PlayGame -> GameOver");
                currentState = scope.Resolve<IGameOverView>();
                return;
            }

            if (currentState is IGamePlayView 
                && gameState.Lifes >= 0 
                && !gameState.Pause)
            {
                Logger.Info("State: PlayGame -> StartPlay");

                currentState = scope.Resolve<IStartPlayView>();
                return;
            }

            if (currentState is IGamePlayView 
                && gameState.Pause)
            {
                Logger.Info("State: PlayGame -> StopPlay");

                currentState = scope.Resolve<IStopPlayView>();
                return;
            }

            if (currentState is IStopPlayView
                && gameState.Lifes < 0
                && gameState.Pause)
            {
                Logger.Info("State: StopPlay -> GameOver");

                currentState = scope.Resolve<IGameOverView>(); 
                return;
            }

            if (currentState is IStopPlayView
                && !gameState.Pause)
            {
                Logger.Info("State: StopPlay -> GamePlay");

                currentState = scope.Resolve<IGamePlayView>();
                return;
            }

            if (currentState is IStartPlayView)
            {
                Logger.Info("State: StartPlay -> GamePlay");
                currentState = scope.Resolve<IGamePlayView>();
                return;
            }

            if (currentState is IGameOverView)
            {
                Logger.Info("State: GameOver -> Begin");
                currentState = scope.Resolve<IGameBeginView>();
                return;
            }
        }
    }
}
