using System;
using System.Collections;
using System.Collections.Generic;

namespace zbrozonoid.Menu
{
    public class MenuViewModel : IMenuViewModel
    {
        public List<IMenuItem> Items { get; private set; } = new List<IMenuItem>();

        public IMenuItem CurrentItem => index.Current;

        private readonly IMenuItemEnum index;

        public MenuViewModel(Action Close, Action InGame)
        {
            Items.Add(new StartMenuItem(InGame));
            Items.Add(new QuitMenuItem(Close));

            index = GetEnumerator();
            index.MoveNext();
        }

        public void ExecuteCommand()
        {
            CurrentItem?.Execute();
        }

        public void Move(int delta)
        {
            if (delta > 1)
            {
                bool result = index.MoveNext();
                if (!result)
                {
                    index.Reset();
                    index.MoveNext();
                }

            } 
            else if (delta < -1)
            {
                bool result = index.MovePrevious();
                if (!result)
                {
                    if (!index.Last())
                    {
                        index.Reset();
                    }

                }

            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IMenuItemEnum GetEnumerator()
        {
            return new MenuItemEnum(Items);
        }

    }
}
