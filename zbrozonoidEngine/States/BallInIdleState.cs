using Autofac;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Interfaces.States;

namespace zbrozonoidEngine.States
{
    public class BallInIdleState : IBallState
    {
        private readonly IPadManager padManager; 

        public BallInIdleState(ILifetimeScope scope)
        {
            this.padManager = scope.Resolve<IPadManager>();
        }

        public bool action(IBall ball)
        {
            IPad pad = padManager.GetFirst();
            padManager.SetBallStartPosition(pad, ball);
            return true;
        }
    }
}
