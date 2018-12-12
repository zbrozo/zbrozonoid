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
namespace zbrozonoid_sfml
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

    public class Window
    {
        private readonly IGame game;

        private readonly RenderWindow app;

        private Image backgroundImage;

        private Sprite background;

        private Text pressButtonToPlayMessage;

        private Vector2i currentMousePosition = new Vector2i();

        private Vector2i previousMousePosition = new Vector2i();

        private Font font;

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

        public Window(IGame game)
        {
            this.game = game;

            game.GetScreenSize(out int width, out int height);
            app = new RenderWindow(new VideoMode((uint)width, (uint)height), "Zbrożonoid - a free arkanoid clone");
            app.SetVerticalSyncEnabled(true);
            
            app.Closed += OnClose;
            app.MouseMoved += OnMouseMove;
            app.MouseLeft += OnMouseLeft;
            app.MouseButtonPressed += OnMouseButtonPressed;
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            game.ShouldGo = true;
        }

        public void OnChangeBackground(object sender, BackgroundEventArgs e)
        {
            if (backgroundImage != null)
            {
                backgroundImage.Dispose();
            }

            LoadBackground(e.Value);
            Texture backgroundTexture = new Texture(backgroundImage);
            background = new Sprite(backgroundTexture);
        }

        private void OnMouseLeft(object sender, EventArgs e)
        {
            game.GetScreenSize(out int width, out int height);
            Vector2i pos = new Vector2i(width / 2, height / 2);
            Mouse.SetPosition(pos, app);
        }

        private void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        void OnMouseMove(object sender, EventArgs e)
        {
            MouseMoveEventArgs args = (MouseMoveEventArgs)e;

            currentMousePosition.X = args.X;
            currentMousePosition.Y = args.Y;

            int delta = currentMousePosition.X - previousMousePosition.X;

            game.GetScreenSize(out int width, out int height);
            Vector2i pos = new Vector2i(width / 2, height / 2);
            Mouse.SetPosition(pos, app);

            previousMousePosition = pos;

            game.SetPadMove(delta);

            if (!game.ShouldGo)
            {
                game.SetBallMove();
            }
        }

        public void Initialize()
        {
            LoadFont("Bungee-Regular.ttf");
            pressButtonToPlayMessage = PreparePressButtonToPlayMessage();
        }

        public void Run()
        {
            app.SetMouseCursorVisible(false);

            game.GetScreenSize(out int width, out int height);
            Vector2i pos = new Vector2i(width / 2, height / 2);
            Mouse.SetPosition(pos, app);

            // Start the game loop
            while (app.IsOpen)
            {
                // Process events
                app.DispatchEvents();

                game.Action();

                if (background != null)
                {
                    app.Draw(background);
                }

                DrawBorders(app);
                DrawBricks(app);
                DrawPad(app);
                DrawBall(app);

                if (!game.ShouldGo)
                {
                    app.Draw(pressButtonToPlayMessage);
                }

                // Update the window
                app.Display();
            }
        }

        private void DrawBorders(RenderWindow app)
        {
            var borderManager = game.GetBorderManager();

            borderManager.First();
            while (!borderManager.IsLast())
            {
                var element = borderManager.GetCurrent() as IElement;
                if (element is null)
                {
                    continue;
                }

                RectangleShape rectangle = new RectangleShape();
                rectangle.Position = new Vector2f(element.PosX, element.PosY);
                rectangle.Size = new Vector2f(element.Width, element.Height);
                rectangle.FillColor = Color.White;

                app.Draw(rectangle);

                borderManager.Next();
            }
        }

        private void DrawPad(RenderWindow app)
        {
            game.GetPadPosition(out int posX, out int posY);
            game.GetPadSize(out var width, out var height);

            VertexArray rect = new VertexArray(PrimitiveType.Quads, 4);
            rect.Append(new Vertex(new Vector2f(posX, posY), Color.White));
            rect.Append(new Vertex(new Vector2f(posX + width, posY), Color.White));
            rect.Append(new Vertex(new Vector2f(posX + width, posY + height), Color.Blue));
            rect.Append(new Vertex(new Vector2f(posX, posY + height), Color.Blue));

            app.Draw(rect);
        }

        private void DrawBricks(RenderWindow app)
        {
            List<IBrick> bricks = game.GetBricks();

            foreach (var entry in bricks)
            {
                IBrick brick = entry;

                if (!brick.Hit && brick.Type > 0)
                {
                    if (colors.TryGetValue((int)brick.ColorNumber, out Color color))
                    {
                        RectangleShape rectangle = new RectangleShape();
                        rectangle.Position = new Vector2f(brick.PosX, brick.PosY);
                        rectangle.Size = new Vector2f(brick.Width, brick.Height);
                        rectangle.FillColor = color;

                        app.Draw(rectangle);
                    }
                }
            }
        }

        private void DrawBall(RenderWindow app)
        {
            var ballManager = game.GetBallManager();
            ballManager.First();
            while (!ballManager.IsLast())
            {
                IBall ball = ballManager.GetCurrent();
                if (ball == null)
                {
                    continue;
                }

                ball.GetPosition(out var posX, out var posY);
                ball.GetSize(out var width, out var height);

                //DrawTail(g, ball);

                CircleShape circle = new CircleShape();
                circle.Position = new Vector2f(posX, posY);
                circle.Radius = 10;
                circle.FillColor = Color.Cyan;

                app.Draw(circle);

                ballManager.Next();
            }
        }

        private void LoadBackground(string name)
        {
            name = name.Replace("/", ".");
            name = "zbrozonoidAssets." + name;

            AssemblyName assemblyName = new AssemblyName(@"zbrozonoidAssets");
            Assembly assembly = Assembly.Load(assemblyName);

            using (Stream resourceStream = assembly.GetManifestResourceStream(name))
            {
                if (resourceStream == null)
                    return;

                backgroundImage = new Image(resourceStream);
            }
        }

        private void LoadFont(string name)
        {
            name = name.Replace("/", ".");
            name = "zbrozonoidAssets.Fonts." + name;

            AssemblyName assemblyName = new AssemblyName(@"zbrozonoidAssets");
            Assembly assembly = Assembly.Load(assemblyName);

            Stream resourceStream = assembly.GetManifestResourceStream(name);
            if (resourceStream is null)
            {
                return;
            }

            font = new Font(resourceStream);
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

    }
}
