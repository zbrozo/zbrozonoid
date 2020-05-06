﻿using System;
using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public static class BrickExtenstions
    {
        public static IEnumerable<BrickWithNumber> DetectCollision(this IEnumerable<BrickWithNumber> bricks, IBall ball, ICollisionManager collisionManager)
        {
            var brickHitList = new List<BrickWithNumber>();

            foreach (var brick in bricks)
            {
                if (!brick.IsHit && brick.IsVisible)
                {
                    if (collisionManager.Detect(brick, ball))
                    {
                        brickHitList.Add(brick);
                    }
                }
            }

            return brickHitList;
        }
    }
}
