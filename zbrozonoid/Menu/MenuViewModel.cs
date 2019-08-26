using System.Collections.Generic;

namespace zbrozonoid.Menu
{
    public class MenuViewModel
    {
        public List<IMenuItem> Items { get; private set; } = new List<IMenuItem>();

        public IMenuItem CurrentItem { get; private set; }

        public MenuViewModel()
        {
            Items.Add(new StartMenuItem());
            Items.Add(new QuitMenuItem());

            CurrentItem = Items[0];
        }

        public void ExecuteCurrentCommand()
        {

        }


    }
}
