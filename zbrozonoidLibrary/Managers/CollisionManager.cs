/*
Copyright(C) 2018 Tomasz Zbrożek

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<https://www.gnu.org/licenses/>.
*/
namespace zbrozonoidLibrary.Managers
{
    using System.Collections.Generic;

    using zbrozonoidLibrary;
    using zbrozonoidLibrary.Interfaces;

    public class CollisionManager : ICollisionManager
    {
        public bool XLeftInside { get; set; }
        public bool XRightInside { get; set; }
        public bool YTopInside { get; set; }
        public bool YBottomInside { get; set; }

        public bool YTopOutside { get; set; }
        public bool YBottomOutside { get; set; }
        public bool XLeftOutside { get; set; }
        public bool XRightOutside { get; set; }

        public List<IBrick> bricksHit { get; set; }

        private bool Check(IElement first, IElement second)
        {
            // second element inside first element
            bool XLeftInside = second.PosX > first.PosX && second.PosX < first.PosX + first.Width;
            bool XRightInside = (second.PosX + second.Width > first.PosX) && (second.PosX + second.Width < first.PosX + first.Width);
            bool YTopInside = (second.PosY > first.PosY) && (second.PosY < first.PosY + first.Height);
            bool YBottomInside = (second.PosY + second.Height > first.PosY) && (second.PosY + second.Height < first.PosY + first.Height);

            // second element outside first element
            bool XLeftOutside = second.PosX <= first.PosX;
            bool XRightOutside = second.PosX + second.Width >= first.PosX + first.Width;
            bool YTopOutside = second.PosY <= first.PosY;
            bool YBottomOutside = second.PosY + second.Height >= first.PosY + first.Height;

            if ((XLeftInside || XRightInside) && (YTopInside || YBottomInside))
            {
                this.XLeftInside = XLeftInside;
                this.XRightInside = XRightInside;
                this.YTopInside = YTopInside;
                this.YBottomInside = YBottomInside;

                this.XLeftOutside = XLeftOutside;
                this.XRightOutside = XRightOutside;
                this.YBottomOutside = YBottomOutside;
                this.YTopOutside = YTopOutside;
                return true;
            }

            if ((XLeftInside || XRightInside) && YTopOutside && YBottomOutside)
            {
                this.XLeftInside = XLeftInside;
                this.XRightInside = XRightInside;
                this.YTopInside = YTopInside;
                this.YBottomInside = YBottomInside;

                this.XLeftOutside = XLeftOutside;
                this.XRightOutside = XRightOutside;
                this.YBottomOutside = YBottomOutside;
                this.YTopOutside = YTopOutside;
                return true;
            }

            if ((YTopInside || YBottomInside) && XLeftOutside && XRightOutside)
            {
                this.XLeftInside = XLeftInside;
                this.XRightInside = XRightInside;
                this.YTopInside = YTopInside;
                this.YBottomInside = YBottomInside;

                this.XLeftOutside = XLeftOutside;
                this.XRightOutside = XRightOutside;
                this.YBottomOutside = YBottomOutside;
                this.YTopOutside = YTopOutside;
                return true;
            }

            return false;
        }

        public bool Detect(IBorder first, IPad second)
        {
            return Check(first as IElement, second as IElement);
        }

        public bool Detect(IBorder first, IBall second)
        {
            return Check(first as IElement, second as IElement);
        }

        public bool Detect(IPad first, IBall second)
        {
            return Check(first as IElement, second as IElement);
        }

        public bool Detect(IBrick first, IBall second)
        {
            if (Check(first as IElement, second as IElement))
            {
                //bricksHit.Add(first);
                return true;
            }
            return false;
        }

        public void Bounce(IBall ball)
        {
            if (bricksHit == null || bricksHit.Count == 0)
            {
                BounceBall(ball);
                return;
            }

            bool onlyOne = (bricksHit.Count == 1);

            if (onlyOne)
            {
                BounceBall(ball);
                return;
            }

            if (bricksHit.Count > 0)
            {
                if (IsPosXEqual())
                {
                    BallBounceFromVertEdge(ball);
                }
                else if (IsPosYEqual())
                {
                    BallBounceFromHorizEdge(ball);
                }
                else
                {
                    ball.BounceBack();
                }
            }
        }

        private void BounceBall(IBall ball)
        {
            if (BounceSmallBall(ball))
            {
                return;
            }

            if (BounceBallFromCorner(ball))
            {
                ball.CalculateNewDegree();
                return;
            }

            if (BounceBigBall(ball))
            {
                ball.CalculateNewDegree();
                return;
            }

            if (BounceBigBallUnusual(ball))
            {
                return;
            }
        }

        private bool BounceSmallBall(IBall ball)
        {
            if (XLeftInside && XRightInside && YTopInside && !YBottomInside)
            {
                ball.Bounce(Edge.Bottom);
                return true;
            }

            if (XLeftInside && XRightInside && !YTopInside && YBottomInside)
            {
                ball.Bounce(Edge.Top);
                return true;
            }

            if (!XLeftInside && XRightInside && YTopInside && YBottomInside)
            {
                ball.Bounce(Edge.Left);
                return true;
            }

            if (XLeftInside && !XRightInside && YTopInside && YBottomInside)
            {
                ball.Bounce(Edge.Right);
                return true;
            }

            return false;
        }

        private bool BounceBigBall(IBall ball)
        {
            if (!XLeftInside && XRightInside && !YTopInside && !YBottomInside && YBottomOutside && YTopOutside && XLeftOutside && !XRightOutside)
            {
                ball.BounceBigFromLeft();
                return true;
            }

            if (XLeftInside && !XRightInside && !YTopInside && !YBottomInside && YBottomOutside && YTopOutside && !XLeftOutside && XRightOutside)
            {
                ball.BounceBigFromRight();
                return true;
            }

            if (!XLeftInside && !XRightInside && YTopInside && !YBottomInside && YBottomOutside && !YTopOutside && XLeftOutside && XRightOutside)
            {
                ball.BounceBigFromBottom();
                return true;
            }

            if (!XLeftInside && !XRightInside && !YTopInside && YBottomInside && !YBottomOutside && YTopOutside && XLeftOutside && XRightOutside)
            {
                ball.BounceBigFromTop();
                return true;
            }

            return false;
        }

        private bool BounceBigBallUnusual(IBall ball)
        {
            if (XLeftInside && XRightInside && !YTopInside && !YBottomInside && YBottomOutside && YTopOutside && !XLeftOutside && !XRightOutside)
            {
                ball.BounceBigFromLeft();
                ball.BounceBigFromRight();
                return true;
            }

            if (!XLeftInside && !XRightInside && YTopInside && YBottomInside && !YBottomOutside && !YTopOutside && XLeftOutside && XRightOutside)
            {
                ball.BounceBigFromTop();
                ball.BounceBigFromBottom();
                return true;
            }

            if (!XLeftInside && !XRightInside && !YTopInside && !YBottomInside && YBottomOutside && YTopOutside && XLeftOutside && XRightOutside)
            {
                ball.BounceBigFromTop();
                ball.BounceBigFromBottom();
                return true;
            }

            return false;
        }

        private bool BounceBallFromCorner(IBall ball)
        {
            if (!XLeftInside && XRightInside && YTopInside && !YBottomInside)
            {
                ball.BounceCorner(Corner.BottomLeft);
                return true;
            }

            if (XLeftInside && !XRightInside && YTopInside && !YBottomInside)
            {
                ball.BounceCorner(Corner.BottomRight);
                return true;
            }

            if (!XLeftInside && XRightInside && !YTopInside && YBottomInside)
            {
                ball.BounceCorner(Corner.TopLeft);
                return true;
            }

            if (XLeftInside && !XRightInside && !YTopInside && YBottomInside)
            {
                ball.BounceCorner(Corner.TopRight);
                return true;
            }

            return false;
        }

        private bool BallBounceFromHorizEdge(IBall ball)
        {
            if (YTopInside && !YBottomInside)
            {
                ball.Bounce(Edge.Bottom);
                return true;
            }

            if (!YTopInside && YBottomInside)
            {
                ball.Bounce(Edge.Top);
                return true;
            }

            return false;
        }

        private bool BallBounceFromVertEdge(IBall ball)
        {
            if (XLeftInside && !XRightInside)
            {
                ball.Bounce(Edge.Right);
                return true;
            }

            if (!XLeftInside && XRightInside)
            {
                ball.Bounce(Edge.Left);
                return true;
            }

            return false;
        }

        /*
        public void Prepare()
        {
            bricksHit.Clear();
        }
        */
        public bool HitBrick(out BrickType type)
        {
            type = BrickType.None;
            if (bricksHit.Count != 0)
            {
                foreach (var brick in bricksHit)
                {
                    if (brick.IsBeatable() && brick.IsVisible())
                    {
                        brick.Hit = true;
                        type = brick.Type;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsPosYEqual()
        {
            if (bricksHit.Count == 0)
            {
                return false;
            }

            var PosY = bricksHit[0].PosY;
            foreach (var value in bricksHit)
            {
                if (PosY != value.PosY)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsPosXEqual()
        {
            if (bricksHit.Count == 0)
            {
                return false;
            }

            var PosX = bricksHit[0].PosX;
            foreach (var value in bricksHit)
            {
                if (PosX != value.PosX)
                {
                    return false;
                }
            }

            return true;
        }

        public void LogData()
        {
            Logger.Instance.Write(
                string.Format(
                    "Inside: {0}, {1}, {2}, {3}",
                    XLeftInside,
                    XRightInside,
                    YTopInside,
                    YBottomInside));

            Logger.Instance.Write(
                string.Format(
                    "Outside: {0}, {1}, {2}, {3}",
                    XLeftOutside,
                    XRightOutside,
                    YTopOutside,
                    YBottomOutside));
        }
    }
}
