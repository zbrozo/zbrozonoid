using zbrozonoidEngine.Interfaces;

namespace zbrozonoidLibrary.States.BallInPlayCommands
{
    public interface IBallInPlayCommand
    {
        bool Execute(IBall ball);
    }
}
