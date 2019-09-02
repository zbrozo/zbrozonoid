using System;
namespace zbrozonoid.Menu.Items
{
    public interface IMenuItem
    {
        string Name { get; }

        void Execute();
    }

}
