using System;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class LinearMovement : IMovement
    {
        public int Iteration { get; set; }
        public int Degree { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 Direction { get; set; }

        private const double oneDegreeConstans = Math.PI / 180.0;

        public bool Move(out Vector2 position)
        {
            ++Iteration;

            position = CalculateNewPosition();

            return true;
        }

        public bool ReverseMove(out Vector2 position)
        {
            --Iteration;

            position = new Vector2();
            if (Iteration < 0)
            {
                return false;
            }

            position = CalculateNewPosition();


            return true;
        }

        private Vector2 CalculateNewPosition()
        {
            int posX = CalculateBallPositionX(Degree, Iteration);
            int posY = CalculateBallPositionY(Degree, Iteration);

            int screenX = posX * Direction.X + Offset.X;
            int screenY = posY * Direction.Y + Offset.Y;

            return new Vector2(screenX, screenY);
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
            return oneDegreeConstans * angle;
        }
    }
}
