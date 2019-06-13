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
namespace zbrozonoidEngine
{
    using System;

    using zbrozonoidEngine.Interfaces;

    public class Ball : IBall
    {
        public Rectangle Boundary { get; set; } = new Rectangle();

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int DirectionX { get; set; }
        public int DirectionY { get; set; }
        public int Iteration { get; set; }
        public int SavedPosX { get; set; }
        public int SavedPosY { get; set; }
        public int Speed { get; set; }

        private int Degree { get; set; }
        private DegreeType DegreeType { get; set; }

        private readonly IRandomGenerator randomGenerator;

        public Ball(IRandomGenerator randomGenerator)
        {
            this.randomGenerator = randomGenerator;

            Degree = 45;
            DegreeType = DegreeType.Centre;  
            Speed = 6;
            DirectionX = 1; 
            DirectionY = 1;
        }

        public void SetSize(int width, int height)
        {
            Boundary.Size = new Vector2(width, height);
        }

        public void GetPosition(out int posX, out int posY)
        {
            posX = Boundary.Min.X;
            posY = Boundary.Min.Y;
        }

        public void GetSize(out int width, out int height)
        {
            width = Boundary.Size.X;
            height = Boundary.Size.Y;
        }

        public void InitStartPosition()
        {
            int x = Boundary.Min.X;
            int y = Boundary.Min.Y;

            OffsetX = x;
            OffsetY = y;

            SavedPosX = x;
            SavedPosY = y;

            Iteration = 0;
        }

        public bool MoveBall(bool reverse = false)
        {
            if (!reverse)
            {
                ++Iteration;
            }
            else
            {
                --Iteration;
            }

            if (Iteration < 0)
            {
                return false;
            }

            CalculateNewPosition();

            if (!reverse)
            {
                LogData();
            }
            else
            {
                LogData(true);
            }

            return true;
        }

        private void CalculateNewPosition()
        {
            int posX = CalculateBallPositionX(Degree, Iteration);
            int posY = CalculateBallPositionY(Degree, Iteration);

            int screenY = 0;
            if (DirectionY == -1)
            {
                screenY = OffsetY - posY;
            }
            else
            {
                screenY = OffsetY + posY;
            }

            int screenX = posX * DirectionX + OffsetX;

            Boundary.Min = new Vector2(screenX, screenY);
        }

        private int CalculateBallPositionX(double angle, double c)
        {
            double cos = Math.Cos(ChangeDegreeToRadians(angle));
            double x = c * cos;
            return (int)x;
        }

        private int CalculateBallPositionY(double angle, double c)
        {
            double sin = Math.Sin(ChangeDegreeToRadians(angle));
            double y = c * sin;
            return (int)y;
        }

        private double ChangeDegreeToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public bool Bounce(Edge edge)
        {
            bool bounce = false;
            if (edge == Edge.Bottom)
            {
                DirectionY = 1;
                bounce = true;
            }

            if (edge == Edge.Top)
            {
                DirectionY = -1;
                bounce = true;
            }

            if (edge == Edge.Left)
            {
                DirectionX = -1;
                bounce = true;
            }

            if (edge == Edge.Right)
            {
                DirectionX = 1;
                bounce = true;
            }

            if (bounce)
            {
                ResetIteration();
            }

            return bounce;
        }

        public bool BounceCorner(Corner corner)
        {
            bool bounce = false;

            if (corner == Corner.BottomLeft)
            {
                DirectionX = -1;
                DirectionY = 1;
                bounce = true;
            }

            if (corner == Corner.BottomRight)
            {
                DirectionX = 1;
                DirectionY = 1;
                bounce = true;
            }

            if (corner == Corner.TopLeft)
            {
                DirectionX = -1;
                DirectionY = -1;
                bounce = true;
            }

            if (corner == Corner.TopRight)
            {
                DirectionX = 1;
                DirectionY = -1;
                bounce = true;
            }

            if (bounce)
            {
                ResetIteration();
            }

            return bounce;

        }

        public void BounceBigFromLeft()
        {
            bool bounce = false;
            if (DirectionX == 1 && DirectionY == 1)
            {
                DirectionX = -1;
                bounce = true;
            }
            else if (DirectionX == 1 && DirectionY == -1)
            {
                DirectionX = -1;
                bounce = true;
            }

            if (bounce)
            {
                ResetIteration();
            }
        }

        public void BounceBigFromRight()
        {
            bool bounce = false;
            if (DirectionX == -1 && DirectionY == 1)
            {
                DirectionX = 1;
                bounce = true;
            }
            else if (DirectionX == -1 && DirectionY == -1)
            {
                DirectionX = 1;
                bounce = true;
            }

            if (bounce)
            {
                ResetIteration();
            }
        }

        public void BounceBigFromTop()
        {
            bool bounce = false;
            if (DirectionX == 1 && DirectionY == 1)
            {
                DirectionY = -1;
                bounce = true;
            }
            else if (DirectionX == -1 && DirectionY == 1)
            {
                DirectionY = -1;
                bounce = true;
            }

            if (bounce)
            {
                ResetIteration();
            }
        }

        public void BounceBigFromBottom()
        {
            bool bounce = false;

            if (DirectionX == 1 && DirectionY == -1)
            {
                DirectionY = 1;
                bounce = true;
            }
            else if (DirectionX == -1 && DirectionY == -1)
            {
                DirectionY = 1;
                bounce = true;
            }

            if (bounce)
            {
                ResetIteration();
            }
        }

        public void BounceBigFromInside()
        {
            BounceBigFromLeft();
            BounceBigFromRight();
        }

        public void BounceBack()
        {
            DirectionX *= -1;
            DirectionY *= -1;

            ResetIteration();
        }

        private void ResetIteration()
        {
            OffsetX = SavedPosX;
            OffsetY = SavedPosY;
            Iteration = 0;
        }

        public void SavePosition()
        {
            SavedPosX = Boundary.Min.X;
            SavedPosY = Boundary.Min.Y;
        }

        public void CalculateNewDegree(DegreeType type)
        {
            if (type != DegreeType)
            {
                Degree = randomGenerator.GenerateDegree(Degree, type);
                DegreeType = type;
            }
        }

        public void LogData(bool reverse = false)
        {
            Logger.Instance.Write(
                string.Format(
                    "Ball {0}: {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                    reverse ? "reverse" : "",
                    Boundary.Min.X,
                    Boundary.Min.Y,
                    Boundary.Size.X,
                    Boundary.Size.Y,
                    OffsetX,
                    OffsetY,
                    DirectionX,
                    DirectionY,
                    Iteration,
                    Degree));
        }

    }
}
