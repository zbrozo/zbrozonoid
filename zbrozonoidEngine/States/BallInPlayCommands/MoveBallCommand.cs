using zbrozonoidEngine.Interfaces;

namespace zbrozonoidLibrary.States.BallInPlayCommands
{
    public class MoveBallCommand : IBallInPlayCommand
    {
        public bool Execute(IBall ball)
        {
            return ball.MoveBall();
        }
    }
}
