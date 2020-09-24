using Autofac;

namespace zbrozonoidEngine
{
    public class ManagerScopeFactory
    {
        public ManagerScopeFactory()
        {
        }

        public ILifetimeScope Create()
        {
            var builder = new ContainerBuilder();
            return builder.Build();
        }
    }
}
