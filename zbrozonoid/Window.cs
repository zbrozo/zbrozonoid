/*
Copyright(C) 2018 Tomasz Zbrożek

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<https://www.gnu.org/licenses/>.
*/
namespace zbrozonoid
{
    using System;

    using SFML.System;
    using SFML.Graphics;
    using SFML.Window;

    using zbrozonoidEngine;
    using zbrozonoidEngine.Interfaces;
    using zbrozonoid.Menu;
    using zbrozonoid.Views;
    using Autofac;
    using System.Collections.Generic;

    public class Window
    {
        private const string Name = "Zbrozonoid - a free arkanoid clone";

        private readonly IGame game;

        private readonly RenderWindow app;

        private IContainer container;

        private ManyMouseDispatcher manyMouseDispatcher;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Window(IGame game)
        {
            this.game = game;

            manyMouseDispatcher = new ManyMouseDispatcher(game.GameConfig.Mouses);

            game.GetScreenSize(out int width, out int height);

            app = new RenderWindow(new VideoMode((uint)width, (uint)height), Name);
            app.SetVerticalSyncEnabled(true);

            app.Closed += OnClose;
            manyMouseDispatcher.MouseMoved += OnManyMouseMove;
            app.MouseButtonPressed += OnMouseButtonPressed;
            app.KeyPressed += OnKeyPressed;
            app.Resized += OnResized;

            RegisterComponentsInContainer();

            container.Resolve<IViewStateMachine>().Initialize(container);
        }

        private void RegisterComponentsInContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(app).As<RenderWindow>();
            builder.RegisterInstance(game).As<IGame>();
            builder.RegisterType<PrepareTextLine>().As<IPrepareTextLine>().SingleInstance();
            builder.RegisterType<ViewModel>().As<IViewModel>().SingleInstance();
            builder.RegisterInstance(new MenuViewModel(game.GameConfig, CloseAction, InGameAction)).As<IMenuViewModel>().SingleInstance();
            builder.RegisterType<MenuView>().As<IMenuView>().SingleInstance();
            builder.RegisterType<DrawGameObjects>().As<IDrawGameObjects>().SingleInstance();
            builder.RegisterType<ViewStateMachine>().As<IViewStateMachine>().SingleInstance();

            builder.RegisterType<GameBeginView>().As<IView>().AsSelf().SingleInstance();
            builder.RegisterType<GameOverView>().As<IView>().AsSelf().SingleInstance(); 
            builder.RegisterType<GamePlayfieldView>().As<IGamePlayfieldView>().AsSelf().SingleInstance();
            builder.RegisterType<GamePlayView>().As<IView>().AsSelf().SingleInstance();
            builder.RegisterType<StartPlayView>().As<IView>().AsSelf().SingleInstance(); 
            builder.RegisterType<StopPlayView>().As<IView>().AsSelf().SingleInstance(); 

            container = builder.Build();



        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                game.GameState.Pause = true; 
                container.Resolve<IViewStateMachine>().Transitions(game);
                return;
            }

            if (e.Code == Keyboard.Key.Y)
            {
                game.GameState.Lifes = -1;
                container.Resolve<IViewStateMachine>().Transitions(game);
                return;
            }

            if (e.Code == Keyboard.Key.N)
            {
                game.GameState.Pause = false;
                container.Resolve<IViewStateMachine>().Transitions(game);
                return;
            }
        }

        private void OnResized(object sender, SizeEventArgs e)
        {
            // TODO
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (container.Resolve<IViewStateMachine>().IsMenuState)
            {
                container.Resolve<IMenuViewModel>().ExecuteCommand();
            }
            else
            {
                container.Resolve<IViewStateMachine>().Transitions(game);
            }
        }

        public void OnChangeLevel(object sender, LevelEventArgs e)
        {
            container.Resolve<GamePlayfieldView>().PrepareBricksToDraw();
            container.Resolve<GamePlayfieldView>().PrepareBackground(e.Background);
        }

        public void OnLostBalls(object sender, EventArgs args)
        {
            container.Resolve<IViewStateMachine>().Transitions(game);
        }

        public void OnBrickHit(object sender, BrickHitEventArgs arg)
        {
            container.Resolve<GamePlayfieldView>().Bricks[arg.Number].IsVisible = false;
        }

        private void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        /*
        private void OnMouseMove(object sender, MouseMoveEventArgs args)
        {
            int current = args.X;

            game.GetScreenSize(out int width, out int height);
            Vector2i pos = new Vector2i(width / 2, height / 2);

            int deltaX = current - pos.X;

            game.SetPadMove(deltaX, 0);

            int deltaY = args.Y - pos.Y;
            menuViewModel.Move(deltaY);
                       
        }
        */

        private void OnManyMouseMove(object sender, MouseMoveEventArgs args)
        {
            game.SetPadMove(args.X, args.Device);

            if (container.Resolve<IViewStateMachine>().IsMenuState)
            {
                container.Resolve<IMenuViewModel>().Move(args.Y);
            }
        }

        public void Initialize()
        {
        }

        private void SetMousePointerInTheMiddleOfTheScreen()
        {
            game.GetScreenSize(out int width, out int height);
            Vector2i pos = new Vector2i(width / 2, height / 2);
            Mouse.SetPosition(pos, app);
        }

        public void Run()
        {
            app.SetMouseCursorVisible(false);
            SetMousePointerInTheMiddleOfTheScreen();

            // Start the game loop
            while (app.IsOpen)
            {
                // Process events
                app.DispatchEvents();
                //manyMouse.DispatchEvents();
                manyMouseDispatcher.DispatchEvents();

                SetMousePointerInTheMiddleOfTheScreen();

                if (!game.GameState.Pause)
                {
                    game.Action();
                }

                container.Resolve<IViewStateMachine>().Action();

                // Update the window
                app.Display();
            }
        }

        public void CloseAction()
        {
            app.Close();
        }

        public void InGameAction()
        {
            container.Resolve<IViewStateMachine>().Transitions(game);
        }

    }
}
