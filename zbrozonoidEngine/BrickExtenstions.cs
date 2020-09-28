using System;
using System.Collections.Generic;
using System.Linq;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public static class BrickExtenstions
    {
        public static IEnumerable<int> DetectCollision(this IEnumerable<IBrick> bricks, IBall ball, ICollisionManager collisionManager)
        {
            var brickHitList = new List<int>();

            int i = 0;
            foreach (var brick in bricks)
            {
                if (!brick.IsHit && brick.IsVisible)
                {
                    if (collisionManager.Detect(brick, ball))
                    {
                        brickHitList.Add(i);
                    }
                }

                ++i;
            }

            return brickHitList;
        }

        public static List<KeyValuePair<IBrick, int>> FilterByIndex(this ICollection<IBrick> bricks, IEnumerable<int> indexes)
        {
            return indexes.Select(i => new KeyValuePair<IBrick, int>(bricks.ElementAt(i), i)).ToList();
        }
    }
}
