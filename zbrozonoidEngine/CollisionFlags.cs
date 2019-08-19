namespace zbrozonoidEngine
{
    public class CollisionFlags
    {
        public bool XLeftInside { get; set; }
        public bool XRightInside { get; set; }
        public bool YTopInside { get; set; }
        public bool YBottomInside { get; set; }

        public bool YTopOutside { get; set; }
        public bool YBottomOutside { get; set; }
        public bool XLeftOutside { get; set; }
        public bool XRightOutside { get; set; }

        public bool OverlapInsideTop()
        {
            return XLeftInside && XRightInside && YTopInside && !YBottomInside;
        }

        public bool OverlapInsideBottom()
        {
            return XLeftInside && XRightInside && !YTopInside && YBottomInside;
        }

        public bool OverlapInsideRight()
        {
            return !XLeftInside && XRightInside && YTopInside && YBottomInside;
        }

        public bool OverlapInsideLeft()
        {
            return XLeftInside && !XRightInside && YTopInside && YBottomInside;
        }

        public bool OverlapInsideFull()
        {
            return XLeftInside && XRightInside && YTopInside && YBottomInside;
        }

        public bool OverlapOutsideBottom()
        {
            return !XLeftInside && !XRightInside && !YTopInside && YBottomInside && !YBottomOutside && YTopOutside && XLeftOutside && XRightOutside;
        }

        public bool OverlapOutsideTop()
        {
            return !XLeftInside && !XRightInside && YTopInside && !YBottomInside && YBottomOutside && !YTopOutside && XLeftOutside && XRightOutside;
        }

        public bool OverlapOutsideLeft()
        {
            return XLeftInside && !XRightInside && !YTopInside && !YBottomInside && YBottomOutside && YTopOutside && !XLeftOutside && XRightOutside;
        }

        public bool OverlapOutsideRight()
        {
            return !XLeftInside && XRightInside && !YTopInside && !YBottomInside && YBottomOutside && YTopOutside && XLeftOutside && !XRightOutside;
        }

        public bool OverlapCornerTopLeft()
        {
            return XLeftInside && !XRightInside && YTopInside && !YBottomInside;
        }

        public bool OverlapCornerTopRight()
        {
            return !XLeftInside && XRightInside && YTopInside && !YBottomInside;
        }

        public bool OverlapCornerBottomLeft()
        {
            return XLeftInside && !XRightInside && !YTopInside && YBottomInside;
        }

        public bool OverlapCornerBottomRight()
        {
            return !XLeftInside && XRightInside && !YTopInside && YBottomInside;
        }

    }
}
