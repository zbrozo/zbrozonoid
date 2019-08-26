using System.Collections.Generic;

namespace zbrozonoid.Menu
{
    public interface IMenuViewModel
    {
        List<IMenuItem> Items { get; }
        IMenuItem CurrentItem { get; }
    }
}
