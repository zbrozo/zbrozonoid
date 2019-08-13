using zbrozonoidEngine.Interfaces;

namespace zbrozonoidLibrary.States.BallInPlayCommands
{
    public interface IBallInPlayCommand
    {
        void Execute(IBall ball);
    }
}
