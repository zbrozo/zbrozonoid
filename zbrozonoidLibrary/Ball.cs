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
namespace zbrozonoidLibrary
{
    using System;

    using zbrozonoidLibrary.Interfaces;

    public class Ball : IBall, IElement
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int DirectionX { get; set; }
        public int DirectionY { get; set; }
        public double Degree { get; set; }
        public int Iteration { get; set; }
        public int SavedPosX { get; set; }
        public int SavedPosY { get; set; }
        public int Speed { get; set; }

        private ITail tail = null;

        private readonly IRandomGenerator randomGenerator;

        public Ball(IRandomGenerator randomGenerator)
        {
            this.randomGenerator = randomGenerator;

            Degree = randomGenerator.GenerateDegree();
            Speed = 6; //randomGenerator.GenerateSpeed();

            DirectionX = randomGenerator.GenerateDirection();
            DirectionY = 1;
        }

        public void AddTail(ITail tail)
        {
            this.tail = tail;
        }

        public ITail GetTail()
        {
            return tail;
        }

        public void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void GetPosition(out int posX, out int posY)
        {
            posX = PosX;
            posY = PosY;
        }

        public void GetSize(out int width, out int height)
        {
            width = Width;
            height = Height;
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

            PosX = posX * DirectionX + OffsetX;

            if (DirectionY == -1)
            {
                PosY = OffsetY - posY;
            }
            else
            {
                PosY = OffsetY + posY;
            }
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
            Position position = new Position { X = PosX, Y = PosY };
            tail?.Add(position);

            SavedPosX = PosX;
            SavedPosY = PosY;
        }

        public void CalculateNewDegree()
        {
            Degree = randomGenerator.CalculateNewDegree(Degree);
        }

        public void LogData(bool reverse = false)
        {
            Logger.Instance.Write(
                string.Format(
                    "Ball {0}: {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                    reverse ? "reverse" : "",
                    PosX,
                    PosY,
                    Width,
                    Height,
                    OffsetX,
                    OffsetY,
                    DirectionX,
                    DirectionY,
                    Iteration,
                    Degree));
        }

    }
}
