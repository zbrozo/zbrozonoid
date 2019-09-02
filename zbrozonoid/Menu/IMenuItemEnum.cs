using System.Collections.Generic;
using zbrozonoid.Menu.Items;

namespace zbrozonoid.Menu
{
    public interface IMenuItemEnum : IEnumerator<IMenuItem>
    {
        bool MovePrevious();
        bool Last();
    }
}
