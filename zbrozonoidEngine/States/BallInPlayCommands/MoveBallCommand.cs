using zbrozonoidEngine.Interfaces;

namespace zbrozonoidLibrary.States.BallInPlayCommands
{
    public class MoveBallCommand : IBallInPlayCommand
    {
        public bool CollisionResult { set; get; }

        public bool Execute(IBall ball)
        {
            CollisionResult = ball.MoveBall();
            return true;
        }
    }
}
