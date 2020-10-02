using Autofac;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.Views
{
    public interface IViewStateMachine
    {
        void Initialize(ILifetimeScope scope);

        bool IsMenuState { get; }
        bool IsPlayState { get; }
        bool IsStopState { get; }
        bool IsStartState { get; }
        bool IsGameOverState { get; }

        void Action();

        void Transitions(IGameEngine game);
    }
}
