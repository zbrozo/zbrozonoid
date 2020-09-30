using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class ScreenCollisionCommand : ICollisionCommand
    {
        private IScreenCollisionManager screenCollisionManager;
        BallCollisionState collisionState;

        public ScreenCollisionCommand(
            IScreenCollisionManager screenCollisionManager, 
            BallCollisionState collisionState)
        {
            this.screenCollisionManager = screenCollisionManager;
            this.collisionState = collisionState;
        }

        public void Detect(IBall ball)
        {
            DetectScreenCollision(ball);
        }

        public void Bounce(IBall ball)
        {
        }

        private void DetectScreenCollision(IBall ball)
        {
            if (screenCollisionManager.Detect(ball))
            {
                collisionState.SetScreenCollistionState(true, false);
            }
        }
    }
}
