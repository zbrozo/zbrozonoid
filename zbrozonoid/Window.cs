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
    using Newtonsoft.Json;

    public class Window
    {
        private const string Name = "Zbrozonoid - a free arkanoid clone";

        private const int RemoteManipulatorId = 9000;

        private readonly IGameEngine game;

        private readonly RenderWindow app;

        private ILifetimeScope viewScope;
        private ILifetimeScope managerScope;

        private ViewScopeFactory viewScopeFactory = new ViewScopeFactory();

        private ManyMouseDispatcher manyMouseDispatcher;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IMenuViewModel menuViewModel;
        private readonly IGamePlayfieldView gamePlayfieldView;
        private readonly IViewStateMachine viewStateMachine;

        private readonly WebClient webClient = new WebClient();

        private Settings settings;

        private readonly int[] manipulators = new int[Settings.MaxPlayers];

        public Window(IGameEngine game)
        {
            this.game = game; // GAME ENGINE

            settings = Settings.LoadSettings() ?? new Settings(); 
            Settings.ValidateSettings(settings);
            game.GameConfig.Players = (int) settings.Players;

            managerScope = game.ManagerScope;

            manyMouseDispatcher = new ManyMouseDispatcher(game.GameConfig.Mouses);

            game.GetScreenSize(out int width, out int height);

            app = new RenderWindow(new VideoMode((uint)width, (uint)height), Name);
            app.SetVerticalSyncEnabled(true);

            app.Closed += OnClose;
            manyMouseDispatcher.MouseMoved += OnManyMouseMove;
            app.MouseButtonPressed += OnMouseButtonPressed;
            app.KeyPressed += OnKeyPressed;
            app.Resized += OnResized;

            viewScope = viewScopeFactory.Create(
                app, 
                game, 
                StartPlayAction,
                managerScope);

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
                    game.StopPlay(); // game over
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
                if (e.Code == Keyboard.Key.Backspace) // force level change :)
                {
                    game.ForceChangeLevel = true;
                    viewStateMachine.Transitions(game);
                    game.StopPlay(); 
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
                if (viewStateMachine.IsStartState)
                {
                    viewStateMachine.Transitions(game);
                    game.StartPlay();
                }

                if (viewStateMachine.IsGameOverState)
                {
                    viewStateMachine.Transitions(game);
                }
            }
        }

        public void OnChangeLevel(object sender, LevelEventArgs e)
        {
            gamePlayfieldView.PrepareBricksToDraw();
            gamePlayfieldView.PrepareBackground(e.Background);
        }

        public void OnLevelCompleted(object sender, EventArgs e)
        {
            game.InitPlay(manipulators, settings.PlayerOneLocation);
        }

        public void OnLostBalls(object sender, EventArgs args)
        {
            viewStateMachine.Transitions(game);
            game.StopPlay(); // game over
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

            if (settings.Remote)
            {
                RemotePadMovement(args.X, RemoteManipulatorId);
            }

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

            settings.Players = (uint) game.GameConfig.Players;
            Settings.SaveSettings(settings);

            managerScope.Dispose();
        }

        public void StartPlayAction()
        {
            InitManipulators();

            viewStateMachine.Transitions(game);

            game.InitPlay(manipulators, settings.PlayerOneLocation);
            game.StartPlay();
        }

        private void InitManipulators()
        {
            int id = 0;
            for (int i = 0; i < game.GameConfig.Mouses && i < game.GameConfig.Players; i++)
            {
                manipulators[i] = id;
                ++id;
            }

            if (settings.Remote)
            {
                manipulators[game.GameConfig.Players - 1] = RemoteManipulatorId;
            }
        }

        private void RemotePadMovement(int movement, uint id)
        {
            var movementJson = JsonConvert.SerializeObject(new PadMovement { PlayerId = (int)settings.PlayerOneId, Move = movement });
            webClient.Put((int)settings.PlayerOneId, movementJson);

            var response = webClient.Get((int)settings.PlayerTwoId);
            var padMovement = JsonConvert.DeserializeObject<PadMovement>(response);
            if (padMovement == null)
            {
                Logger.Error("Remote paddle movement not received");
                return;
            }

            game.SetPadMove(padMovement.Move, id);

        }
    }
}

