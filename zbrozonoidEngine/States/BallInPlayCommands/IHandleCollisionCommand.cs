using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public interface IHandleCollisionCommand
    {
        bool CollisionResult { get; }

        bool Execute(IBall ball);
    }
}
