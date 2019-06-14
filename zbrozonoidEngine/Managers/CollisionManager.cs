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
namespace zbrozonoidEngine.Managers
{
    using System.Collections.Generic;

    using zbrozonoidEngine;
    using zbrozonoidEngine.Interfaces;

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

        private bool Check(IBoundary first, IBoundary second)
        {
            // second element inside first element
            bool XLeftInside = second.Boundary.Min.X > first.Boundary.Min.X  
                               && second.Boundary.Min.X < first.Boundary.Max.X;
            bool XRightInside = second.Boundary.Max.X > first.Boundary.Min.X 
                                && second.Boundary.Max.X < first.Boundary.Max.X;
            bool YTopInside = second.Boundary.Min.Y > first.Boundary.Min.Y 
                              && second.Boundary.Min.Y < first.Boundary.Max.Y;
            bool YBottomInside = second.Boundary.Max.Y > first.Boundary.Min.Y
                                 && second.Boundary.Max.Y < first.Boundary.Max.Y;

            // second element outside first element
            bool XLeftOutside = second.Boundary.Min.X <= first.Boundary.Min.X;
            bool XRightOutside = second.Boundary.Max.X >= first.Boundary.Max.X;
            bool YTopOutside = second.Boundary.Min.Y <= first.Boundary.Min.Y;
            bool YBottomOutside = second.Boundary.Max.Y >= first.Boundary.Max.Y;

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
            return Check(first as IBoundary, second as IBoundary);
        }

        public bool Detect(IBorder first, IBall second)
        {
            return Check(first as IBoundary, second as IBoundary);
        }

        public bool Detect(IPad first, IBall second)
        {
            return Check(first as IBoundary, second as IBoundary);
        }

        public bool Detect(IBrick first, IBall second)
        {
            if (Check(first as IBoundary, second as IBoundary))
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
                ball.CalculateNewDegree(DegreeType.Centre);
                return;
            }

            if (BounceBallFromCorner(ball))
            {
                ball.CalculateNewDegree(DegreeType.Corner);
                return;
            }

            if (BounceBigBall(ball))
            {
                ball.CalculateNewDegree(DegreeType.Centre);
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

            var PosY = bricksHit[0].Boundary.Min.Y;
            foreach (var value in bricksHit)
            {
                if (PosY != value.Boundary.Min.Y)
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

            var PosX = bricksHit[0].Boundary.Min.X;
            foreach (var value in bricksHit)
            {
                if (PosX != value.Boundary.Min.X)
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
