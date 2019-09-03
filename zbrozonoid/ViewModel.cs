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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SFML.Graphics;
using SFML.System;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{
    public class ViewModel : IViewModel
    {
        private IGame game;

        private Dictionary<int, Color> colors = new Dictionary<int, Color>
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

        private Image backgroundImage;

        private readonly int lineHeight = 40;

        public Sprite Background { get; set; }
        public List<Brick> Bricks { get; } = new List<Brick>();

        public Text LiveAndScoresMessage
        {
            get { return PrepareLifesAndScoresMessage(); }
        }

        public Text FasterBallMessage
        {
            get { return PrepareFasterBallMessage(); }
        }

        public Text FireBallMessage
        {
            get { return PrepareFireBallMessage(); }
        }

        public Text GameOverMessage { get; set; }
        public Text PressButtonToPlayMessage { get; set; }

        private ViewCommon viewCommon;

        public ViewModel(ViewCommon viewCommon, IGame game)
        {
            this.game = game;
            this.viewCommon = viewCommon;

            GameOverMessage = PrepareGameOverMessage();
            PressButtonToPlayMessage = PreparePressButtonToPlayMessage();
        }

        public void Dispose()
        {
            backgroundImage?.Dispose();
        }

        public void PrepareBricksToDraw()
        {
            Bricks.Clear();

            List<IBrick> bricks = game.Bricks;
            foreach (IBrick brick in bricks)
            {
                if (/*!brick.Hit &&*/ brick.Type > 0)
                {
                    if (colors.TryGetValue((int)brick.ColorNumber, out Color color))
                    {
                        RectangleShape rectangle = new RectangleShape();
                        rectangle.Position = new Vector2f(brick.Boundary.Min.X, brick.Boundary.Min.Y);
                        rectangle.Size = new Vector2f(brick.Boundary.Size.X, brick.Boundary.Size.Y);
                        rectangle.FillColor = color;

                        Brick brickToDraw = new Brick(rectangle);
                        Bricks.Add(brickToDraw);
                    }
                }
            }
        }

        public void PrepareBackground(string backgroundName)
        {
            backgroundImage?.Dispose();
            backgroundImage = LoadBackground(backgroundName);

            Texture backgroundTexture = new Texture(backgroundImage);
            Background = new Sprite(backgroundTexture);
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

        private Text PrepareGameOverMessage()
        {
            uint charSize = 50;
            Text message = new Text("game over", viewCommon.Font, charSize);
            message.FillColor = Color.White;

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();
            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, lineHeight * 4);
            message.Position = rect;

            return message;
        }

        private Text PrepareLifesAndScoresMessage()
        {
            uint charSize = 20;
            int lifes = game.GameState.Lifes >= 0 ? game.GameState.Lifes : 0;
            int scores = game.GameState.Scores;
            Text message = new Text($"Lifes: {lifes}   Scores: {scores:D5}", viewCommon.Font, charSize);
            message.FillColor = Color.White;

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();

            int offsetX = 20;
            int offsetY = 30;
            Vector2f rect = new Vector2f(offsetX, height - localBounds.Height - offsetY);
            message.Position = rect;

            return message;
        }

        private Text PreparePressButtonToPlayMessage()
        {
            uint charSize = 50;
            Text message = new Text("Press mouse button to play", viewCommon.Font, charSize);
            message.FillColor = Color.White;

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();
            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, lineHeight * 4);
            message.Position = rect;

            return message;
        }

        private Text PrepareFasterBallMessage()
        {
            int value = 0;
            foreach (var counter in game.GameState.FasterBallCountdown)
            {
                if (counter.Value > value)
                {
                    value = counter.Value;
                }
            }

            uint charSize = 20;
            Text message = new Text($"FasterBall: {value}", viewCommon.Font, charSize)
            {
                FillColor = Color.White
            };

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();

            int offsetX = 800;
            int offsetY = 20;
            Vector2f rect = new Vector2f(offsetX, height - localBounds.Height - offsetY);
            message.Position = rect;

            return message;
        }

        private Text PrepareFireBallMessage()
        {
            int value = 0;
            foreach (var counter in game.GameState.FireBallCountdown)
            {
                if (counter.Value > value)
                {
                    value = counter.Value;
                }
            }

            uint charSize = 20;
            Text message = new Text($"FireBall: {value}", viewCommon.Font, charSize)
            {
                FillColor = Color.White
            };

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();

            int offsetX = 800;
            int offsetY = 40;
            Vector2f rect = new Vector2f(offsetX, height - localBounds.Height - offsetY);
            message.Position = rect;

            return message;
        }

    }
}
