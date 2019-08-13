using zbrozonoidEngine.Interfaces;

namespace zbrozonoidLibrary.States.BallInPlayCommands
{
    public class MoveBallCommand : IBallInPlayCommand
    {
        public void Execute(IBall ball)
        {
            ball.MoveBall();
        }
    }
}
