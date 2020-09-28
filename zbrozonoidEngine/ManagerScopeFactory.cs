using Autofac;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;

namespace zbrozonoidEngine
{
    public class ManagerScopeFactory
    {
        public ManagerScopeFactory()
        {
        }

        public ILifetimeScope Create(IScreen screen)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(screen).As<IScreen>();
            builder.RegisterType<LevelManager>().As<ILevelManager>().SingleInstance();
            builder.RegisterType<CollisionManager>().As<ICollisionManager>().SingleInstance();
            builder.RegisterType<ScreenCollisionManager>().As<IScreenCollisionManager>().SingleInstance();
            builder.RegisterType<TailManager>().As<ITailManager>().SingleInstance();
            builder.RegisterType<BallManager>().As<IBallManager>().SingleInstance();
            builder.RegisterType<BorderManager>().As<IBorderManager>().SingleInstance();
            builder.RegisterType<PadManager>().As<IPadManager>().SingleInstance();
            builder.RegisterType<BorderCollisionManager>().As<IBorderCollisionManager>().SingleInstance();

            return builder.Build();
        }
    }
}
