using System;
using Autofac;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.Views
{
    public interface IViewStateMachine
    {
        void Initialize(IContainer container);

        bool IsMenuState { get; }
        bool IsPlayState { get; }
        bool IsStopState { get; }

        void Action();

        void Transitions(IGame game);
    }
}
