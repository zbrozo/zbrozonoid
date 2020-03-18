using System;
using Autofac;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.Views
{
    public interface IViewStateMachine
    {
        void Initialize(IContainer container);

        bool IsMenuState { get; }

        void GotoMenu();

        void GotoPlay();

        void GotoGameOver();

        void Action();

        void Transitions(IGame game);
    }
}
