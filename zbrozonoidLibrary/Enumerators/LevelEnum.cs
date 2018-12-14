

namespace zbrozonoidLibrary.Enumerators
{
    using System;
    using System.Collections.Generic;
    using System.Collections;

    public class LevelEnum : IEnumerator
    {
        private readonly string[] levelNames;

        int position = -1;

        public LevelEnum(string[] levelNames)
        {
            this.levelNames = levelNames;
        }

        public bool MoveNext()
        {
            position++;
            return position < levelNames.Length;
        }

        public void Reset()
        {
            position = -1;
        }

        //void IDisposable.Dispose() { }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public string Current
        {
            get
            {
                try
                {
                    return levelNames[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
