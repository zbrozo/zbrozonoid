using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public interface ICollisionCommand
    {
        void Detect(IBall ball);
        void Bounce(IBall ball);
    }
}
