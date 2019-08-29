using System.Collections.Generic;

namespace zbrozonoid.Menu
{
    public interface IMenuItemEnum : IEnumerator<IMenuItem>
    {
        bool MovePrevious();
        bool Last();
    }
}
