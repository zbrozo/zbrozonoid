﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.Counters
{
    public class FastBallCounter : IEnumerable<int>
    {
        private readonly Dictionary<IBall, int> counters = new Dictionary<IBall, int>();

        public int GetValue() => counters.Any() ? counters.Max(x => x.Value) : 0;

        public void TimerHandler(IBall ball, int value)
        {
            if (value <= 0)
            {
                counters.Remove(ball);
                return;
            }

            if (!counters.Keys.Contains(ball))
            {
                counters.Add(ball, value);
            }
            else
            {
                counters[ball] = value;
            }
        }

        public void Clear()
        {
            counters.Clear();
        }

        public IEnumerator<int> GetEnumerator()
        {
            return counters.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return counters.Values.GetEnumerator();
        }
    }
}
