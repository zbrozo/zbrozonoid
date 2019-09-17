using System;
using System.Linq;
using ManyMouseSharp;

namespace zbrozonoid
{
    public class MouseMoveEventArgs : EventArgs
    {
        public uint Device;
        public int X;
        public int Y;

        public override string ToString()
        {
            return "[MouseMoveEventArgs]" + " Device:" + Device +
                   " X(" + X + ")" +
                   " Y(" + Y + ")";
        }
    }

    public class ManyMouseDispatcher
    {
        public event EventHandler<MouseMoveEventArgs> MouseMoved;

        private class MouseData
        {
            public uint Device;
            public int X;
            public int Y;
            public bool MouseMoved;
        }

        private readonly MouseData[] mouseData;
        private readonly int mouseDataSize;

        public ManyMouseDispatcher(int number)
        {
            mouseDataSize = number;
            mouseData = new MouseData[mouseDataSize];

            for (int i = 0; i < mouseDataSize; i++)
            {
                mouseData[i] = new MouseData();
            }
        }

        public void DispatchEvents()
        {
            foreach (var data in mouseData)
            {
                data.X = 0;
                data.Y = 0;
                data.MouseMoved = false;
            }

            while (ManyMouse.PollEvent(out ManyMouseEvent mmevent) > 0)
            {
                if (mmevent.type == ManyMouseEventType.MANYMOUSE_EVENT_RELMOTION)
                {
                    int x = 0;
                    int y = 0;

                    uint device = mmevent.device;
                    if (mmevent.item == 0)
                    {
                        x = mmevent.value;
                    }
                    else
                    {
                        y = mmevent.value;
                    }

                    mouseData[device].Device = mmevent.device;
                    mouseData[device].X += x;
                    mouseData[device].Y += y;
                    mouseData[device].MouseMoved = true;
                }
                else if (mmevent.type == ManyMouseEventType.MANYMOUSE_EVENT_ABSMOTION)
                {
                }
                else if (mmevent.type == ManyMouseEventType.MANYMOUSE_EVENT_BUTTON)
                {
                }
                else if (mmevent.type == ManyMouseEventType.MANYMOUSE_EVENT_SCROLL)
                {
                }
                else if (mmevent.type == ManyMouseEventType.MANYMOUSE_EVENT_MAX)
                {
                }
                else if (mmevent.type == ManyMouseEventType.MANYMOUSE_EVENT_DISCONNECT)
                {
                }
            }

            foreach (var data in mouseData.Where(x => x.MouseMoved))
            {
                var eventArgs = new MouseMoveEventArgs
                {
                    Device = data.Device,
                    X = data.X,
                    Y = data.Y
                };

                MouseMoved?.Invoke(this, eventArgs);
            }
        }
    }
}
