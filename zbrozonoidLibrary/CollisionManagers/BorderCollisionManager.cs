

// Decorator on CollistionManager

namespace zbrozonoid.CollisionManagers
{
    using zbrozonoidLibrary.Interfaces;

    public class BorderCollisionManager : IBorderCollisionManager
    {
        private IBorder border;

        private ICollisionManager collisionManager;

        public BorderCollisionManager(IBorder border, ICollisionManager collisionManager)
        {
            this.border = border;
            this.collisionManager = collisionManager;
        }

        public bool DetectAndVerify(IPad pad)
        {
            if (collisionManager.Detect(border, pad))
            {
                IElement borderElement = border as IElement;
                IElement padElement = pad as IElement;

                if (border.Type == Edge.Left)
                {
                    padElement.PosX = borderElement.PosX + borderElement.Width;
                    return true;
                }

                if (border.Type == Edge.Right)
                {
                    padElement.PosX = borderElement.PosX - padElement.Width;
                    return true;
                }
            }

            return false;
        }

        public bool DetectAndVerify(IBall ball)
        {
            if (collisionManager.Detect(border, ball))
            {
                collisionManager.Bounce(ball);
                ball.SavePosition();
                return true;
            }
            return false;
        }
    }
}
