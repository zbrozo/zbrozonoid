using System.Collections;
using System.Collections.Generic;

namespace zbrozonoid.Menu
{
    public class MenuViewModel : IMenuViewModel
    {
        public List<IMenuItem> Items { get; private set; } = new List<IMenuItem>();

        public IMenuItem CurrentItem => index.Current;

        private readonly IEnumerator<IMenuItem> index;

        public MenuViewModel()
        {
            Items.Add(new StartMenuItem());
            Items.Add(new QuitMenuItem());

            index = GetEnumerator();
            index.MoveNext();
        }

        public void ExecuteCommand()
        {

        }

        public void Move(int delta)
        {
            if (delta != 0)
            {
                bool result = index.MoveNext();
                if (!result)
                {
                    index.Reset();
                    index.MoveNext();
                }

            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IMenuItem> GetEnumerator()
        {
            return new MenuItemEnum(Items);
        }

    }
}
