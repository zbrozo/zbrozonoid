using System;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.Views
{
    public interface IViewStateMachine
    {
        bool IsMenuState { get; }

        void GotoMenu();

        void GotoPlay();

        void GotoGameOver();

        void Action();

        void Transitions(IGame game);
    }
}
