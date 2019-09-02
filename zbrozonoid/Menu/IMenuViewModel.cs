using System.Collections;
using System.Collections.Generic;
using zbrozonoid.Menu.Items;

namespace zbrozonoid.Menu
{
    public interface IMenuViewModel : IEnumerable
    {
        IMenuItem CurrentItem { get; }

        List<IMenuItem> Items { get; }

        void Move(int delta);

        void ExecuteCommand();

    }
}
