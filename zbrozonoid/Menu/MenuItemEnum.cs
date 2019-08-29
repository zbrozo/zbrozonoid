using System;
using System.Collections;
using System.Collections.Generic;

namespace zbrozonoid.Menu
{
    public class MenuItemEnum : IMenuItemEnum
    {
        public List<IMenuItem> Items;

        int position = -1;

        public MenuItemEnum(List<IMenuItem> items)
        {
            Items = items;
        }

        public bool MoveNext()
        {
            position++;

            return (position < Items.Count);
        }

        public bool MovePrevious()
        {
            position--;

            return (position >= 0);
        }

        public void Reset()
        {
            position = -1;
        }

        public bool Last()
        {
            position = Items.Count - 1;
            return position < Items.Count;
        }

        void IDisposable.Dispose() { }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public IMenuItem Current
        {
            get
            {
                try
                {
                    return Items[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}

