using System;
using System.Collections.Generic;
using Autofac;
using SFML.Graphics;
using zbrozonoid.Menu;
using zbrozonoid.Views;
using zbrozonoid.Views.Interfaces;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{
    public class ViewScopeFactory
    {
        public ViewScopeFactory()
        {
        }

        public ILifetimeScope Create(RenderWindow app, IGameEngine game, Action InGameAction, ILifetimeScope managerScope)
        {
            var scope = managerScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(app).As<RenderWindow>();
                builder.RegisterInstance(game).As<IGameEngine>();
                builder.RegisterType<PrepareTextLine>().As<IPrepareTextLine>().SingleInstance();

                builder.RegisterInstance(new MenuViewModel(game.GameConfig, app.Close, InGameAction)).As<IMenuViewModel>().SingleInstance();
                builder.RegisterType<MenuView>().As<IMenuView>().SingleInstance();

                builder.RegisterType<RenderProxy>().As<IRenderProxy>().SingleInstance();

                builder.RegisterType<ViewStateMachine>().As<IViewStateMachine>().SingleInstance();

                builder.RegisterType<GameBeginView>().As<IGameBeginView>().SingleInstance();
                builder.RegisterType<GameOverView>().As<IGameOverView>().SingleInstance();
                builder.RegisterType<GamePlayView>().As<IGamePlayView>().SingleInstance();
                builder.RegisterType<StartPlayView>().As<IStartPlayView>().SingleInstance();
                builder.RegisterType<StopPlayView>().As<IStopPlayView>().SingleInstance();

                builder.RegisterType<GamePlayfieldView>()
                    .As<IGamePlayfieldView>()
                    .WithParameter(new TypedParameter(typeof(ICollection<IBrick>), game.Bricks))
                    .SingleInstance();

                builder.RegisterType<InfoPanelView>().As<IInfoPanelView>().SingleInstance();

            });

            return scope;
        }
    }
}
