using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public interface IHandleCollisionCommand
    {
        void Execute(IBall ball);
    }
}
