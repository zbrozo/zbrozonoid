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

    public class Window
    {
        private const string Name = "Zbrozonoid - a free arkanoid clone";

        private readonly IGame game;

        private readonly RenderWindow app;

        private ViewStateMachine appStateMachine;

        private IDrawGameObjects drawGameObjects;

        private IViewModel viewModel;
        private IMenuViewModel menuViewModel;

        private IView menuView;
        private IPrepareTextLine prepareTextLine;

        private ManyMouseDispatcher manyMouseDispatcher;

        //private ManyMouse manyMouse;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public Window(IGame game/*, ManyMouse manyMouse*/)
        {
            this.game = game;
            //  this.manyMouse = manyMouse;

            manyMouseDispatcher = new ManyMouseDispatcher(game.GameConfig.Mouses);

            game.GetScreenSize(out int width, out int height);
            
            ContextSettings settings = new ContextSettings();
            settings.DepthBits = 32;
            settings.StencilBits = 8;
            settings.AntialiasingLevel = 4;
            settings.MajorVersion = 3;
            settings.MinorVersion = 0;

            app = new RenderWindow(new VideoMode((uint)width, (uint)height), Name);
            app.SetVerticalSyncEnabled(true);

            app.Closed += OnClose;
            manyMouseDispatcher.MouseMoved += OnManyMouseMove;
            //manyMouse.MouseMoved += OnManyMouseMove;
            //app.MouseMoved += OnMouseMove;
            app.MouseButtonPressed += OnMouseButtonPressed;
            app.KeyPressed += OnKeyPressed;
            app.Resized += OnResized;

            prepareTextLine = new PrepareTextLine(app);

            viewModel = new ViewModel(prepareTextLine, game);

            menuViewModel = new MenuViewModel(game.GameConfig, CloseAction, InGameAction);
            menuView = new MenuView(prepareTextLine, menuViewModel);

            drawGameObjects = new DrawGameObjects(app, viewModel, menuViewModel, game);
            appStateMachine = new ViewStateMachine(viewModel, menuView, drawGameObjects);
            appStateMachine.GotoMenu();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                game.GameState.Pause = true;
                appStateMachine.Transitions(game);
                return;
            }

            if (e.Code == Keyboard.Key.Y)
            {
                game.GameState.Lifes = -1;
                appStateMachine.Transitions(game);
                return;
            }

            if (e.Code == Keyboard.Key.N)
            {
                game.GameState.Pause = false;
                appStateMachine.Transitions(game);
                return;
            }
        }

        private void OnResized(object sender, SizeEventArgs e)
        {
            // TODO
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (appStateMachine.IsMenuState)
            {
                menuViewModel.ExecuteCommand();
            }
            else
            {
                appStateMachine.Transitions(game);
            }
        }

        public void OnChangeLevel(object sender, LevelEventArgs e)
        {
            viewModel.PrepareBricksToDraw();
            viewModel.PrepareBackground(e.Background);
        }

        public void OnLostBalls(object sender, EventArgs args)
        {
            appStateMachine.Transitions(game);
        }

        public void OnBrickHit(object sender, BrickHitEventArgs arg)
        {
            viewModel.Bricks[arg.Number].IsVisible = false;
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

            if (appStateMachine.IsMenuState)
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

                appStateMachine.Action();

                // Update the window
                app.Display();
            }

            viewModel.Dispose();
        }

        public void CloseAction()
        {
            app.Close();
        }

        public void InGameAction()
        {
            appStateMachine.Transitions(game);
        }

    }
}
