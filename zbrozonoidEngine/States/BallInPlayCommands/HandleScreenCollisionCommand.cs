using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class HandleScreenCollisionCommand : IHandleCollisionCommand
    {
        private IScreenCollisionManager screenCollisionManager;
        BallCollisionState collisionState;

        public HandleScreenCollisionCommand(IScreenCollisionManager screenCollisionManager, 
                                            BallCollisionState collisionState)
        {
            this.screenCollisionManager = screenCollisionManager;
            this.collisionState = collisionState;
        }

        public void Execute(IBall ball)
        {
            HandleScreenCollision(ball);
        }

        private void HandleScreenCollision(IBall ball)
        {
            if (screenCollisionManager.Detect(ball))
            {
                collisionState.SetScreenCollistionState(true, false);
            }
        }
    }
}
