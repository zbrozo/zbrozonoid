using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States
{
    public class BallCollisionState
    {
        public List<int> BricksHitList { get; private set; } = new List<int>();
        public List<IBorder> BordersHitList { get; private set; } = new List<IBorder>();
        public IPad Pad { get; private set; }

        public bool CollisionWithBrick { get; private set; }
        public bool BounceFromBrick { get; private set; }

        public bool CollisionWithBorder { get; private set; }
        public bool BounceFromBorder { get; private set; }

        public bool CollisionWithScreen { get; private set; }
        public bool BounceFromScreen { get; private set; }

        public bool CollisionWithPad { get; private set; }
        public bool BounceFromPad { get; private set; }

        public void SetBrickCollisionState(bool hit, bool bounce, List<int> bricksHitList)
        {
            CollisionWithBrick = hit;
            BounceFromBrick = bounce;
            BricksHitList = bricksHitList;
        }

        public void SetBorderCollistionState(bool hit, bool bounce, List<IBorder> bordersHitList)
        {
            CollisionWithBorder = hit;
            BounceFromBorder = bounce;
            BordersHitList = bordersHitList;
        }

        public void SetScreenCollistionState(bool hit, bool bounce)
        {
            CollisionWithScreen = hit;
            BounceFromScreen = bounce;
        }

        public void SetPadCollistionState(bool hit, bool bounce, IPad pad)
        {
            CollisionWithPad = hit;
            BounceFromPad = bounce;
            Pad = pad;
        }

        public void Clear()
        {
            BricksHitList.Clear();
            BordersHitList.Clear();

            Pad = null;

            CollisionWithBrick = false;
            BounceFromBrick = false;

            CollisionWithPad = false;
            BounceFromPad = false;

            CollisionWithScreen = false;
            BounceFromScreen = false;

            CollisionWithBorder = false;
            BounceFromBorder = false;
        }

    }
}
