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
    using System.Timers;
    using NLog;
    using zbrozonoidEngine.Interfaces;

    public class Ball : IBall, IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Rectangle Boundary { get; set; } = new Rectangle();

        private Vector2 SavedPosition { get; set; }

        public int Speed { get; private set; }

        private DegreeType DegreeType { get; set; }

        private readonly IRandomGenerator randomGenerator;

        private readonly IMovement movement;

        private readonly Timer timer = new Timer();

        private const int timerInterval = 1000;
        private const int timerMaxTime = 20; // 20 seconds
        private int timerCounter;

        public Action<IBall, int> BallSpeedTimerCallback { get; set; }

        public Ball(IRandomGenerator randomGenerator, IMovement movement)
        {
            this.randomGenerator = randomGenerator;
            this.movement = movement;

            timer.Elapsed += OnTimerEvent;
            timer.Interval = timerInterval;
            timer.AutoReset = true;
            timer.Start();

            DegreeType = DegreeType.Centre;
            Speed = (int)BallSpeed.Default;
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

            movement.Offset = new Vector2(x, y);
            movement.Iteration = 0;

            SavedPosition = new Vector2(x, y);
        }

        public bool MoveBall(bool reverse = false)
        {
            if (!reverse)
            {
                bool result = movement.Move(out Vector2 position);
                if (!result)
                {
                    return false;
                }

                Boundary.Min = position;
            }
            else
            {
                bool result = movement.ReverseMove(out Vector2 position);
                if (!result)
                {
                    return false;
                }

                Boundary.Min = position;
            }
            return true;
        }

        public bool Bounce(Edge edge)
        {
            bool bounce = false;
            if (edge == Edge.Top)
            {
                movement.Direction = new Vector2(movement.Direction.X, 1);
                bounce = true;
            }

            if (edge == Edge.Bottom)
            {
                movement.Direction = new Vector2(movement.Direction.X, -1);
                bounce = true;
            }

            if (edge == Edge.Right)
            {
                movement.Direction = new Vector2(-1, movement.Direction.Y);
                bounce = true;
            }

            if (edge == Edge.Left)
            {
                movement.Direction = new Vector2(1, movement.Direction.Y);
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
                movement.Direction = new Vector2(1, -1);
                bounce = true;
            }

            if (corner == Corner.BottomRight)
            {
                movement.Direction = new Vector2(-1, -1);
                bounce = true;
            }

            if (corner == Corner.TopLeft)
            {
                movement.Direction = new Vector2(1, 1);
                bounce = true;
            }

            if (corner == Corner.TopRight)
            {
                movement.Direction = new Vector2(-1, 1);
                bounce = true;
            }

            if (bounce)
            {
                ResetIteration();
            }

            return bounce;
        }

        public void BounceBack()
        {
            movement.Direction = new Vector2(movement.Direction.X * -1, movement.Direction.Y * -1);
            ResetIteration();
        }

        private void ResetIteration()
        {
            movement.Offset = new Vector2(SavedPosition.X, SavedPosition.Y);
            movement.Iteration = 0;
        }

        public void SavePosition()
        {
            SavedPosition = Boundary.Min;
        }

        public void CalculateNewDegree(DegreeType type)
        {
            if (type != DegreeType)
            {
                movement.Degree = randomGenerator.GenerateDegree(type);
                DegreeType = type;
            }
        }

        public void SetYPosition(int y)
        {
            Boundary.Min = new Vector2(Boundary.Min.X, y);
            movement.Offset = new Vector2(movement.Offset.X, y);
            SavedPosition = new Vector2(Boundary.Min.X, y);
        }

        public void LogData(bool reverse = false)
        {
            Logger.Info(
                string.Format(
                    "Ball {0}: min: {1}| size: {2}| pos: {3}| direction: {4}| iteration: {5}| degree: {6}",
                    reverse ? "reverse" : "",
                    Boundary.Min,
                    Boundary.Size,
                    movement.Offset,
                    movement.Direction,
                    movement.Iteration,
                    movement.Degree));
        }

        public void SetFasterSpeed()
        {
            Speed = (int)BallSpeed.Faster;

            timerCounter = 0;

            timer.Stop();
            timer.Start();
        }

        public void SetDefaultSpeed()
        {
            Speed = (int)BallSpeed.Default;

            timer.Stop();

            BallSpeedTimerCallback?.Invoke(this, 0);
        }

        private void OnTimerEvent(object source, ElapsedEventArgs e)
        {
            if (timerCounter > timerMaxTime)
            {
                Speed = (int)BallSpeed.Default;
                return;
            }

            BallSpeedTimerCallback?.Invoke(this, timerMaxTime - timerCounter);

            ++timerCounter;
        }

        public void Dispose()
        {
            timer.Stop();
            BallSpeedTimerCallback?.Invoke(this, 0);
            timerCounter = 0;
        }
    }
}
