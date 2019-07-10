﻿/*
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
        private CollisionFlags flags = new CollisionFlags();

        public List<IBrick> BricksHit { get; set; }

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
                flags.XLeftInside = XLeftInside;
                flags.XRightInside = XRightInside;
                flags.YTopInside = YTopInside;
                flags.YBottomInside = YBottomInside;

                flags.XLeftOutside = XLeftOutside;
                flags.XRightOutside = XRightOutside;
                flags.YBottomOutside = YBottomOutside;
                flags.YTopOutside = YTopOutside;
                return true;
            }

            if ((XLeftInside || XRightInside) && YTopOutside && YBottomOutside)
            {
                flags.XLeftInside = XLeftInside;
                flags.XRightInside = XRightInside;
                flags.YTopInside = YTopInside;
                flags.YBottomInside = YBottomInside;

                flags.XLeftOutside = XLeftOutside;
                flags.XRightOutside = XRightOutside;
                flags.YBottomOutside = YBottomOutside;
                flags.YTopOutside = YTopOutside;
                return true;
            }

            if ((YTopInside || YBottomInside) && XLeftOutside && XRightOutside)
            {
                flags.XLeftInside = XLeftInside;
                flags.XRightInside = XRightInside;
                flags.YTopInside = YTopInside;
                flags.YBottomInside = YBottomInside;

                flags.XLeftOutside = XLeftOutside;
                flags.XRightOutside = XRightOutside;
                flags.YBottomOutside = YBottomOutside;
                flags.YTopOutside = YTopOutside;
                return true;
            }

            return false;
        }

        public bool Detect(IBoundary first, IBoundary second)
        {
            return Check(first as IBoundary, second as IBoundary);
        }


        public void Bounce(IPad pad, IBall ball)
        {
            BounceBall(pad, ball);
        }

        public void Bounce(IBorder border, IBall ball)
        {
            BounceBall(border, ball);
        }


        public void Bounce(IBoundary bounceFromObject, IBall ball)
        {
            if (BricksHit == null || BricksHit.Count == 0)
            {
                BounceBall(bounceFromObject, ball);
                return;
            }

            bool onlyOne = (BricksHit.Count == 1);

            if (onlyOne)
            {
                BounceBall(bounceFromObject, ball);
                return;
            }

            if (BricksHit.Count > 0)
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

        private void BounceBall(IBoundary bounceFromObject, IBall ball)
        {
            if (ball.Boundary.Size.X <= bounceFromObject.Boundary.Size.X)
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

            if (ball.Boundary.Size.Y <= bounceFromObject.Boundary.Size.Y)
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
            if (flags.OverlapInsideTop())
            {
                ball.Bounce(Edge.Top);
                return true;
            }

            if (flags.OverlapInsideBottom())
            {
                ball.Bounce(Edge.Bottom);
                return true;
            }
            return false;
        }

        private bool BounceSmallBallLeftOrRight(IBall ball)
        {
            if (flags.OverlapInsideRight())
            {
                ball.Bounce(Edge.Right);
                return true;
            }

            if (flags.OverlapInsideLeft())
            {
                ball.Bounce(Edge.Left);
                return true;
            }

            return false;
        }

        private bool BounceBigBallBottomOrTop(IBall ball)
        {
            if (flags.OverlapOutsideTop())
            {
                ball.Bounce(Edge.Top);
                return true;
            }

            if (flags.OverlapOutsideBottom())
            {
                ball.Bounce(Edge.Bottom);
                return true;
            }
            return false;
        }

        private bool BounceBigBallLeftOrRight(IBall ball)
        {
            if (flags.OverlapOutsideRight())
            {
                ball.Bounce(Edge.Right);
                return true;
            }

            if (flags.OverlapOutsideLeft())
            {
                ball.Bounce(Edge.Left);
                return true;
            }
            return false;
        }

        private bool BounceBallFromCorner(IBall ball)
        {
            if (flags.OverlapCornerBottomLeft())
            {
                ball.BounceCorner(Corner.BottomLeft);
                return true;
            }

            if (flags.OverlapCornerBottomRight())
            {
                ball.BounceCorner(Corner.BottomRight);
                return true;
            }

            if (flags.OverlapCornerTopLeft())
            {
                ball.BounceCorner(Corner.TopLeft);
                return true;
            }

            if (flags.OverlapCornerTopRight())
            {
                ball.BounceCorner(Corner.TopRight);
                return true;
            }

            return false;
        }

        private bool BallBounceFromHorizEdge(IBall ball)
        {
            if (flags.YTopInside && !flags.YBottomInside)
            {
                ball.Bounce(Edge.Top);
                return true;
            }

            if (!flags.YTopInside && flags.YBottomInside)
            {
                ball.Bounce(Edge.Bottom);
                return true;
            }

            return false;
        }

        private bool BallBounceFromVertEdge(IBall ball)
        {
            if (flags.XLeftInside && !flags.XRightInside)
            {
                ball.Bounce(Edge.Left);
                return true;
            }

            if (!flags.XLeftInside && flags.XRightInside)
            {
                ball.Bounce(Edge.Right);
                return true;
            }

            return false;
        }

        public bool HitBrick(out BrickType type)
        {
            type = BrickType.None;
            if (BricksHit.Count != 0)
            {
                foreach (var brick in BricksHit)
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
            if (BricksHit.Count == 0)
            {
                return false;
            }

            var PosY = BricksHit[0].Boundary.Min.Y;
            foreach (var value in BricksHit)
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
            if (BricksHit.Count == 0)
            {
                return false;
            }

            var PosX = BricksHit[0].Boundary.Min.X;
            foreach (var value in BricksHit)
            {
                if (PosX != value.Boundary.Min.X)
                {
                    return false;
                }
            }

            return true;
        }

        public CollisionFlags GetFlags()
        {
            return flags;
        }

        public void LogData()
        {
            Logger.Instance.Write(
                string.Format(
                    "Inside: {0}, {1}, {2}, {3}",
                    flags.XLeftInside,
                    flags.XRightInside,
                    flags.YTopInside,
                    flags.YBottomInside));

            Logger.Instance.Write(
                string.Format(
                    "Outside: {0}, {1}, {2}, {3}",
                    flags.XLeftOutside,
                    flags.XRightOutside,
                    flags.YTopOutside,
                    flags.YBottomOutside));
        }
    }
}
