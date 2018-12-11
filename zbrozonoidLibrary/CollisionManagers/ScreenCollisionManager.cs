namespace zbrozonoid.CollisionManagers
{
    using zbrozonoidLibrary.Interfaces;

    public class ScreenCollisionManager : IScreenCollisionManager
    {
        private readonly IScreen screen;

        public ScreenCollisionManager(IScreen screen)
        {
            this.screen = screen;
        }

        public bool DetectAndVerify(IPad pad)
        {
            IElement padElement = pad as IElement;
            if (padElement == null)
            {
                return false;
            }

            if (padElement.PosX <= 0)
            {
                padElement.PosX = 0;
            }

            if (padElement.PosX > screen.Width - padElement.Width)
            {
                padElement.PosX = screen.Width - padElement.Width;
            }

            return false;
        }

        public bool DetectAndVerify(IBall ball)
        {
            IElement ballElement = ball as IElement;
            if (ballElement == null)
            {
                return false;
            }

            if (ballElement.PosX < 0)
            {
                ballElement.PosX = 0;
                return ball.Bounce(Edge.Right);
            }

            if (ballElement.PosX > screen.Width - ballElement.Width)
            {
                ballElement.PosX = screen.Width - ballElement.Width;
                return ball.Bounce(Edge.Left);
            }

            if (ballElement.PosY < 0)
            {
                ballElement.PosY = 0;
                return ball.Bounce(Edge.Bottom);
            }

            if (ballElement.PosY > screen.Height - ballElement.Height)
            {
                ballElement.PosY = screen.Height - ballElement.Height;
                return ball.Bounce(Edge.Top);
            }

            return false;
        }

    }
}
