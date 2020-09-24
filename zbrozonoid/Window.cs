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
    using Autofac;

    using SFML.System;
    using SFML.Graphics;
    using SFML.Window;

    using zbrozonoidEngine;
    using zbrozonoidEngine.Interfaces;

    using zbrozonoid.Menu;
    using zbrozonoid.Views;

    public class Window
    {
        private const string Name = "Zbrozonoid - a free arkanoid clone";

        private readonly IGame game;

        private readonly RenderWindow app;

        private ILifetimeScope viewScope;
        private ViewScopeFactory viewScopeFactory = new ViewScopeFactory();

        private ManyMouseDispatcher manyMouseDispatcher;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IMenuViewModel menuViewModel;
        private readonly IGamePlayfieldView gamePlayfieldView;
        private readonly IViewStateMachine viewStateMachine;

        public Window(IGame game)
        {
            this.game = game; // GAME ENGINE

            manyMouseDispatcher = new ManyMouseDispatcher(game.GameConfig.Mouses);

            game.GetScreenSize(out int width, out int height);

            app = new RenderWindow(new VideoMode((uint)width, (uint)height), Name);
            app.SetVerticalSyncEnabled(true);

            app.Closed += OnClose;
            manyMouseDispatcher.MouseMoved += OnManyMouseMove;
            app.MouseButtonPressed += OnMouseButtonPressed;
            app.KeyPressed += OnKeyPressed;
            app.Resized += OnResized;

            viewScope = viewScopeFactory.Create(app, game, InGameAction);

            menuViewModel = viewScope.Resolve<IMenuViewModel>();
            gamePlayfieldView = viewScope.Resolve<IGamePlayfieldView>();
            viewStateMachine = viewScope.Resolve<IViewStateMachine>();

            viewStateMachine.Initialize(viewScope);
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                game.GameState.Pause = true; 
                viewStateMachine.Transitions(game);
                return;
            }

            if (viewStateMachine.IsStopState)
            {
                if (e.Code == Keyboard.Key.Y)
                {
                    game.GameState.Lifes = -1;
                    viewStateMachine.Transitions(game);
                    return;
                }

                if (e.Code == Keyboard.Key.N)
                {
                    game.GameState.Pause = false;
                    viewStateMachine.Transitions(game);
                    return;
                }

            }

            if (viewStateMachine.IsPlayState)
            {
                if (e.Code == Keyboard.Key.Backspace)
                {
                    game.ForceChangeLevel = true;
                    viewStateMachine.Transitions(game);
                    return;
                }
            }
        }

        private void OnResized(object sender, SizeEventArgs e)
        {
            // TODO ?
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (viewStateMachine.IsMenuState)
            {
                menuViewModel.ExecuteCommand();
            }
            else
            {
                viewStateMachine.Transitions(game);
            }
        }

        public void OnChangeLevel(object sender, LevelEventArgs e)
        {
            gamePlayfieldView.PrepareBricksToDraw();
            gamePlayfieldView.PrepareBackground(e.Background);
        }

        public void OnLostBalls(object sender, EventArgs args)
        {
            viewStateMachine.Transitions(game);
        }

        public void OnBrickHit(object sender, BrickHitEventArgs arg)
        {
            gamePlayfieldView.BrickHit(arg.Number);
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
            //Logger.Debug($"Mouse move: {args.X}, Device:{args.Device}");

            game.SetPadMove(args.X, args.Device);

            if (viewStateMachine.IsMenuState)
            {
                menuViewModel.Move(args.Y);
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

                viewStateMachine.Action();

                // Update the window
                app.Display();
            }
        }

        public void InGameAction()
        {
            viewStateMachine.Transitions(game);
        }
    }
}
