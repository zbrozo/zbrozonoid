﻿/*
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
    using zbrozonoid.AppSettings;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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

        private readonly Settings settings;
        private readonly RemotePadMovement remotePadMovement;
        private ICollection<zbrozonoidEngine.Player> Players;



        public Window(IGameEngine game)
        {
            this.game = game; // GAME ENGINE

            settings = Settings.LoadSettings() ?? new Settings();
            Settings.ValidateSettings(settings);
            game.GameConfig.Players = settings.Players.PlayersAmount;

            remotePadMovement = new RemotePadMovement(settings);

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
                viewStateMachine.Transitions(game.GameState);
                return;
            }

            if (viewStateMachine.IsStopState)
            {
                if (e.Code == Keyboard.Key.Y)
                {
                    game.GameState.Lifes = -1;
                    viewStateMachine.Transitions(game.GameState);
                    game.StopPlay(); // game over
                    return;
                }

                if (e.Code == Keyboard.Key.N)
                {
                    game.GameState.Pause = false;
                    viewStateMachine.Transitions(game.GameState);
                    return;
                }

            }

            if (viewStateMachine.IsPlayState)
            {
                if (e.Code == Keyboard.Key.Backspace) // force level change :)
                {
                    game.ForceChangeLevel = true;
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
                if (viewStateMachine.IsPlayState)
                {
                    game.StartBall();
                }

                if (viewStateMachine.IsStartState)
                {
                    viewStateMachine.Transitions(game.GameState);
                    game.StartPlay();
                }

                if (viewStateMachine.IsGameOverState)
                {
                    viewStateMachine.Transitions(game.GameState);
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
            viewStateMachine.Transitions(game.GameState);
            game.StopPlay();
            game.InitPlay(Players);
        }

        public void OnLostBalls(object sender, EventArgs args)
        {
            viewStateMachine.Transitions(game.GameState);
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
                remotePadMovement.PutAndGet(args.X, RemoteManipulatorId, game.SetPadMove);
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

            settings.Players.PlayersAmount = game.GameConfig.Players;
            Settings.SaveSettings(settings);

            managerScope.Dispose();
        }

        public void StartPlayAction()
        {
            ICollection<int> manipulators = PrepareManipulators();
            PreparePlayers(manipulators);

            viewStateMachine.Transitions(game.GameState);

            game.InitPlay(Players);
            game.StartPlay();
        }

        private ICollection<int> PrepareManipulators()
        {
            IList<int> manipulators = new List<int>();

            int id = 0;
            for (int i = 0; i < game.GameConfig.Mouses && i < game.GameConfig.Players; i++)
            {
                manipulators.Add(id);
                ++id;
            }

            return manipulators;
        }

        private void PreparePlayers(ICollection<int> manipulators)
        {
            Players = new List<zbrozonoidEngine.Player>();
            foreach (var details in settings.Players.PlayersDetails)
            {
                var player = new zbrozonoidEngine.Player();

                player.nr = details.Nr;

                if (manipulators.Count <= player.nr - 1)
                {
                    player.manipulator = manipulators.ElementAt(0);
                }
                else
                {
                    player.manipulator = manipulators.ElementAt(player.nr - 1);
                }
                player.location = details.Location;

                Players.Add(player);
            }

            if (settings.Remote)
            {
                var player = Players.First(x => x.nr == 2);
                if (player != null)
                {
                    player.manipulator = RemoteManipulatorId;
                }
            }
        }
    }
}

