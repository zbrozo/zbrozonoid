using System.Collections.Generic;
using System.Linq;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.Counters
{
    public class FireBallCounter
    {
        private Dictionary<ITail, int> counters = new Dictionary<ITail, int>();
        private ITailManager tailManager;

        public int GetValue() => counters.Any() ? counters.Max(x => x.Value) : 0;

        public FireBallCounter(ITailManager tailManager)
        {
            this.tailManager = tailManager;
        }

        public void FireBallTimerHandler(ITail tail, int value)
        {
            if (value <= 0)
            {
                if (tailManager.Remove(tail))
                {
                    counters.Remove(tail);
                }
                return;
            }

            if (!counters.Keys.Contains(tail))
            {
                counters.Add(tail, value);
            }
            else
            {
                counters[tail] = value;
            }
        }

        public void Clear()
        {
            counters.Clear();
        }
    }
}
