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
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using SFML.System;
    using SFML.Graphics;
    using SFML.Window;

    using zbrozonoidLibrary;
    using zbrozonoidLibrary.Interfaces;

    internal class Brick
    {
        public bool IsVisible { get; set; } = true;
        public RectangleShape Rect { get; set; }

        public Brick(RectangleShape rect)
        {
            Rect = rect;
        }
    }

    public class Window
    {
        private const string Name = "Zbrozonoid - a free arkanoid clone";

        private readonly IGame game;

        private readonly RenderWindow app;

        private Image backgroundImage;

        private Sprite background;

        private Text pressButtonToPlayMessage;

        private Text livesMessage;

        private Text gameOverMessage;

        private Font font;

        private List<Brick> bricksToDraw = new List<Brick>();

        Dictionary<int, Color> colors = new Dictionary<int, Color>
                                            {
                                                { 0, Color.Black },
                                                { 1, Color.White },
                                                { 2, Color.Red },
                                                { 3, Color.Cyan },
                                                { 4, new Color(204, 68, 204) },
                                                { 5, Color.Green },
                                                { 6, Color.Blue },
                                                { 7, Color.Yellow },
                                                { 8, new Color(221, 136, 85) },
                                                { 9, new Color(255, 119, 199) },
                                                { 10, new Color(255, 119, 119) }, // LightRed
                                                { 11, new Color(51, 51, 51) },
                                                { 12, new Color(119, 119, 119) },
                                                { 13, new Color(170, 255, 102) },
                                                { 14, new Color(0, 136, 255) },
                                                { 15, new Color(187, 187, 187) }
                                            };

        private bool Pause = false;

        public Window(IGame game)
        {
            this.game = game;

            game.GetScreenSize(out int width, out int height);

            ContextSettings settings = new ContextSettings();
            settings.DepthBits = 32;
            settings.StencilBits = 8;
            settings.AntialiasingLevel = 4;
            settings.MajorVersion = 3;
            settings.MinorVersion = 0;

            app = new RenderWindow(new VideoMode((uint)width, (uint)height), Name, Styles.Default, settings);
            app.SetVerticalSyncEnabled(true);

            app.Closed += OnClose;
            app.MouseMoved += OnMouseMove;
            app.MouseButtonPressed += OnMouseButtonPressed;
            app.KeyPressed += OnKeyPressed;
            app.Resized += OnResized;
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                game.ShouldGo = false;
                Pause = true;

                app.SetMouseCursorVisible(true);
            }
        }

        private void OnResized(object sender, SizeEventArgs e)
        {
            // TODO
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (!game.ShouldGo)
            {
                game.StartPlay();
            }
        }

        public void OnChangeLevel(object sender, LevelEventArgs e)
        {
            PrepareBricksToDraw();

            backgroundImage?.Dispose();
            backgroundImage = LoadBackground(e.Background);
                
            Texture backgroundTexture = new Texture(backgroundImage);
            background = new Sprite(backgroundTexture);
        }

        public void OnBrickHit(object sender, BrickHitEventArgs arg)
        {
            bricksToDraw[arg.Number].IsVisible = false;
        }

        private void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        void OnMouseMove(object sender, MouseMoveEventArgs args)
        {
            int current = args.X;

            game.GetScreenSize(out int width, out int height);
            Vector2i pos = new Vector2i(width / 2, height / 2);

            int delta = current - pos.X;

            game.SetPadMove(delta);
        }

        public void Initialize()
        {
            font?.Dispose();
            font = LoadFont("Bungee-Regular.ttf");
            pressButtonToPlayMessage = PreparePressButtonToPlayMessage();
            gameOverMessage = PrepareGameOverMessage();
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

                if (!Pause)
                {
                    SetMousePointerInTheMiddleOfTheScreen();
                }

                game.Action();

                DrawBackground(app);
                DrawBorders(app);
                DrawBricks(app);
                DrawPad(app);
                DrawBall(app);
                DrawTexts(app);

                // Update the window
                app.Display();
            }

            font?.Dispose();
            background?.Dispose();
        }

        private void DrawTexts(RenderWindow app)
        {
            if (!game.ShouldGo)
            {
                app.Draw(pressButtonToPlayMessage);
            }

            if (game.Lives < 0)
            {
                app.Draw(gameOverMessage);
            }

            livesMessage = PrepareLivesMessage();
            app.Draw(livesMessage);
        }

        private void DrawBackground(RenderWindow app)
        {
            if (background != null)
            {
                app.Draw(background);
            }
        }

        private void DrawBorders(RenderWindow app)
        {
            var borderManager = game.BorderManager;

            foreach (IBorder border in borderManager)
            {
                var element = border as IElement;
                if (element == null)
                {
                    continue;
                }

                RectangleShape rectangle = new RectangleShape();
                rectangle.Position = new Vector2f(element.PosX, element.PosY);
                rectangle.Size = new Vector2f(element.Width, element.Height);
                rectangle.FillColor = Color.White;

                app.Draw(rectangle);
            }
        }

        private void DrawPad(RenderWindow app)
        {

            foreach (IPad pad in game.PadManager)
            {
                game.GetPadPosition(pad, out int posX, out int posY);

                game.GetPadSize(pad, out int width, out int height);

                VertexArray rect = new VertexArray(PrimitiveType.Quads, 4);
                rect.Append(new Vertex(new Vector2f(posX, posY), Color.White));
                rect.Append(new Vertex(new Vector2f(posX + width, posY), Color.White));
                rect.Append(new Vertex(new Vector2f(posX + width, posY + height), Color.Blue));
                rect.Append(new Vertex(new Vector2f(posX, posY + height), Color.Blue));
                app.Draw(rect);
            }
        }

        private void DrawBricks(RenderWindow app)
        {
            foreach (Brick brick in bricksToDraw)
            {
                if (brick.IsVisible)
                {
                    app.Draw(brick.Rect);
                }
            }
        }

        private void DrawBall(RenderWindow app)
        {
            var ballManager = game.BallManager;
            foreach (IBall ball in ballManager)
            {
                ball.GetPosition(out int posX, out int posY);
                ball.GetSize(out int width, out int height);

                DrawTail(app, ball);

                CircleShape circle = new CircleShape();
                circle.Position = new Vector2f(posX, posY);
                circle.Radius = (float)width / 2;
                circle.FillColor = Color.Cyan;

                app.Draw(circle);
            }
        }

        private Image LoadBackground(string name)
        {
            name = name.Replace("/", ".");
            name = "zbrozonoidAssets." + name;

            AssemblyName assemblyName = new AssemblyName(@"zbrozonoidAssets");
            Assembly assembly = Assembly.Load(assemblyName);

            using (Stream resourceStream = assembly.GetManifestResourceStream(name))
            {
                if (resourceStream == null)
                {
                    return null;
                }

                return new Image(resourceStream);
            }
        }

        private Font LoadFont(string name)
        {
            name = name.Replace("/", ".");
            name = "zbrozonoidAssets.Fonts." + name;

            AssemblyName assemblyName = new AssemblyName(@"zbrozonoidAssets");
            Assembly assembly = Assembly.Load(assemblyName);

            Stream resourceStream = assembly.GetManifestResourceStream(name);
            if (resourceStream == null)
            {
                return null;
            }

            return new Font(resourceStream);
        }

        private Text PreparePressButtonToPlayMessage()
        {
            uint charSize = 50;
            Text message = new Text("Press mouse button to play", font, charSize);
            message.Color = new Color(Color.White);

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();
            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, (float)height / 4 - localBounds.Height / 2);
            message.Position = rect;

            return message;
        }

        private Text PrepareLivesMessage()
        {
            uint charSize = 20;
            int lives = game.Lives >= 0 ? game.Lives : 0;
            int scores = game.Scores;
            Text message = new Text($"Lives: {lives}   Scores: {scores:D5}", font, charSize);
            message.Color = new Color(Color.White);

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();

            int offsetX = 20;
            int offsetY = 30;
            Vector2f rect = new Vector2f(offsetX, (float)height - localBounds.Height - offsetY);
            message.Position = rect;

            return message;
        }

        private Text PrepareGameOverMessage()
        {
            uint charSize = 50;
            Text message = new Text("game over", font, charSize);
            message.Color = new Color(Color.White);

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();
            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, (float)height / 6 - localBounds.Height / 2);
            message.Position = rect;

            return message;
        }

        private void DrawTail(RenderWindow app, IBall ball)
        {
            ITail tail = game.TailManager.Find(ball);
            if (tail != null)
            {
                int i = 0;
                int opacity = 150;
                foreach (Position position in tail)
                {
                    ++i;
                    if (i % 14 == 0)
                    {
                        if (opacity < 0)
                        {
                            break;
                        }

                        ball.GetSize(out int width, out int height);

                        Color color = Color.Cyan;
                        color.A = (byte)opacity;

                        CircleShape circle = new CircleShape();
                        circle.Position = new Vector2f(position.X, position.Y);
                        circle.Radius = (float)width / 2;
                        circle.FillColor = color;
                        app.Draw(circle);

                        opacity = opacity - 60;
                    }
                }
            }
        }

        private void PrepareBricksToDraw()
        {
            bricksToDraw.Clear();

            List<IBrick> bricks = game.Bricks;
            foreach (IBrick brick in bricks)
            {
                if (/*!brick.Hit &&*/ brick.Type > 0)
                {
                    if (colors.TryGetValue((int)brick.ColorNumber, out Color color))
                    {
                        RectangleShape rectangle = new RectangleShape();
                        rectangle.Position = new Vector2f(brick.PosX, brick.PosY);
                        rectangle.Size = new Vector2f(brick.Width, brick.Height);
                        rectangle.FillColor = color;

                        Brick brickToDraw = new Brick(rectangle);
                        bricksToDraw.Add(brickToDraw);
                    }
                }
            }
        }
    }
}
