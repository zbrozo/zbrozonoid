using System;
namespace zbrozonoid.Menu
{
    public interface IMenuItem
    {
        string Name { get; }

        void Execute();
    }

}
