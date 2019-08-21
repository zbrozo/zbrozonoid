﻿using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public enum BallSpeed
    {
        Default = 4,
        Faster = 6
    }

    public delegate void BallSpeedTimerCallbackDelegate(IBall ball, int value);

}