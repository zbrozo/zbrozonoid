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
    using zbrozonoidLibrary;

    public class CollisionManager : ICollisionManager
    {
        public CollisionFlags Flags { get; set; } = new CollisionFlags();

        private bool Check(IBoundary first, IBoundary second)
        {
            // second element inside first element
            bool XLeftInside = second.Boundary.Min.X >= first.Boundary.Min.X  
                               && second.Boundary.Min.X < first.Boundary.Max.X;
            bool XRightInside = second.Boundary.Max.X > first.Boundary.Min.X 
                                && second.Boundary.Max.X <= first.Boundary.Max.X;
            bool YTopInside = second.Boundary.Min.Y >= first.Boundary.Min.Y 
                              && second.Boundary.Min.Y < first.Boundary.Max.Y;
            bool YBottomInside = second.Boundary.Max.Y > first.Boundary.Min.Y
                                 && second.Boundary.Max.Y <= first.Boundary.Max.Y;

            // second element outside first element
            bool XLeftOutside = second.Boundary.Min.X < first.Boundary.Min.X;
            bool XRightOutside = second.Boundary.Max.X > first.Boundary.Max.X;
            bool YTopOutside = second.Boundary.Min.Y < first.Boundary.Min.Y;
            bool YBottomOutside = second.Boundary.Max.Y > first.Boundary.Max.Y;

            if ((XLeftInside || XRightInside) && (YTopInside || YBottomInside))
            {
                Flags.XLeftInside = XLeftInside;
                Flags.XRightInside = XRightInside;
                Flags.YTopInside = YTopInside;
                Flags.YBottomInside = YBottomInside;

                Flags.XLeftOutside = XLeftOutside;
                Flags.XRightOutside = XRightOutside;
                Flags.YBottomOutside = YBottomOutside;
                Flags.YTopOutside = YTopOutside;
                return true;
            }

            if ((XLeftInside || XRightInside) && YTopOutside && YBottomOutside)
            {
                Flags.XLeftInside = XLeftInside;
                Flags.XRightInside = XRightInside;
                Flags.YTopInside = YTopInside;
                Flags.YBottomInside = YBottomInside;

                Flags.XLeftOutside = XLeftOutside;
                Flags.XRightOutside = XRightOutside;
                Flags.YBottomOutside = YBottomOutside;
                Flags.YTopOutside = YTopOutside;
                return true;
            }

            if ((YTopInside || YBottomInside) && XLeftOutside && XRightOutside)
            {
                Flags.XLeftInside = XLeftInside;
                Flags.XRightInside = XRightInside;
                Flags.YTopInside = YTopInside;
                Flags.YBottomInside = YBottomInside;

                Flags.XLeftOutside = XLeftOutside;
                Flags.XRightOutside = XRightOutside;
                Flags.YBottomOutside = YBottomOutside;
                Flags.YTopOutside = YTopOutside;
                return true;
            }

            return false;
        }

        public bool Detect(IBoundary first, IBoundary second)
        {
            return Check(first as IBoundary, second as IBoundary);
        }
       
        public void Bounce(List<IBrick> bricksHit, IBoundary obstacle, IBall ball)
        {
            if (bricksHit == null || bricksHit.Count == 0)
            {
                BounceBall(obstacle, ball);
                return;
            }

            bool onlyOne = (bricksHit.Count == 1);

            if (onlyOne)
            {
                BounceBall(obstacle, ball);
                return;
            }

            if (bricksHit.Count > 0)
            {
                if (IsPosXEqual(bricksHit))
                {
                    BallBounceFromVertEdge(ball);
                }
                else if (IsPosYEqual(bricksHit))
                {
                    BallBounceFromHorizEdge(ball);
                }
                else
                {
                    ball.BounceBack();
                }
            }
        }

        private void BounceBall(IBoundary obstacle, IBall ball)
        {
            if (ball.Boundary.Size.X <= obstacle.Boundary.Size.X)
            {
                if(BounceSmallBallBottomOrTop(ball))
                {
                    ball.CalculateNewDegree(DegreeType.Centre);
                    return;
                }
            }
            else
            {
                if (BounceBigBallBottomOrTop(ball))
                {
                    ball.CalculateNewDegree(DegreeType.Centre);
                    return;
                }
            }

            if (ball.Boundary.Size.Y <= obstacle.Boundary.Size.Y)
            {
                if (BounceSmallBallLeftOrRight(ball))
                {
                    ball.CalculateNewDegree(DegreeType.Centre);
                    return;
                }
            }
            else
            {
                if (BounceBigBallLeftOrRight(ball))
                {
                    ball.CalculateNewDegree(DegreeType.Centre);
                    return;
                }
            }

            if (BounceBallFromCorner(ball))
            {
                ball.CalculateNewDegree(DegreeType.Corner);
                return;
            }
        }

        private bool BounceSmallBallBottomOrTop(IBall ball)
        {
            if (Flags.OverlapInsideTop())
            {
                ball.Bounce(Edge.Top);
                return true;
            }

            if (Flags.OverlapInsideBottom())
            {
                ball.Bounce(Edge.Bottom);
                return true;
            }
            return false;
        }

        private bool BounceSmallBallLeftOrRight(IBall ball)
        {
            if (Flags.OverlapInsideRight())
            {
                ball.Bounce(Edge.Right);
                return true;
            }

            if (Flags.OverlapInsideLeft())
            {
                ball.Bounce(Edge.Left);
                return true;
            }

            return false;
        }

        private bool BounceBigBallBottomOrTop(IBall ball)
        {
            if (Flags.OverlapOutsideTop())
            {
                ball.Bounce(Edge.Top);
                return true;
            }

            if (Flags.OverlapOutsideBottom())
            {
                ball.Bounce(Edge.Bottom);
                return true;
            }
            return false;
        }

        private bool BounceBigBallLeftOrRight(IBall ball)
        {
            if (Flags.OverlapOutsideRight())
            {
                ball.Bounce(Edge.Right);
                return true;
            }

            if (Flags.OverlapOutsideLeft())
            {
                ball.Bounce(Edge.Left);
                return true;
            }
            return false;
        }

        private bool BounceBallFromCorner(IBall ball)
        {
            if (Flags.OverlapCornerBottomLeft())
            {
                ball.BounceCorner(Corner.BottomLeft);
                return true;
            }

            if (Flags.OverlapCornerBottomRight())
            {
                ball.BounceCorner(Corner.BottomRight);
                return true;
            }

            if (Flags.OverlapCornerTopLeft())
            {
                ball.BounceCorner(Corner.TopLeft);
                return true;
            }

            if (Flags.OverlapCornerTopRight())
            {
                ball.BounceCorner(Corner.TopRight);
                return true;
            }

            return false;
        }

        private bool BallBounceFromHorizEdge(IBall ball)
        {
            if (Flags.YTopInside && !Flags.YBottomInside)
            {
                ball.Bounce(Edge.Top);
                return true;
            }

            if (!Flags.YTopInside && Flags.YBottomInside)
            {
                ball.Bounce(Edge.Bottom);
                return true;
            }

            return false;
        }

        private bool BallBounceFromVertEdge(IBall ball)
        {
            if (Flags.XLeftInside && !Flags.XRightInside)
            {
                ball.Bounce(Edge.Left);
                return true;
            }

            if (!Flags.XLeftInside && Flags.XRightInside)
            {
                ball.Bounce(Edge.Right);
                return true;
            }

            return false;
        }

        private bool IsPosYEqual(List<IBrick> bricksHit)
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

        private bool IsPosXEqual(List<IBrick> bricksHit)
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
                    Flags.XLeftInside,
                    Flags.XRightInside,
                    Flags.YTopInside,
                    Flags.YBottomInside));

            Logger.Instance.Write(
                string.Format(
                    "Outside: {0}, {1}, {2}, {3}",
                    Flags.XLeftOutside,
                    Flags.XRightOutside,
                    Flags.YTopOutside,
                    Flags.YBottomOutside));
        }
    }
}
