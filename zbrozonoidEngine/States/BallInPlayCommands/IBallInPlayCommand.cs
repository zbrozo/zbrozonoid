using zbrozonoidEngine.Interfaces;

namespace zbrozonoidLibrary.States.BallInPlayCommands
{
    public interface IBallInPlayCommand
    {
        bool CollisionResult { get; }

        bool Execute(IBall ball);
    }
}
