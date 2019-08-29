using System.Collections;
using System.Collections.Generic;

namespace zbrozonoid.Menu
{
    public interface IMenuViewModel : IEnumerable
    {
        IMenuItem CurrentItem { get; }

        List<IMenuItem> Items { get; }

        void Move(int delta);

    }
}
