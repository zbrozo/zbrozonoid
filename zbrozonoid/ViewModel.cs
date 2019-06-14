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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SFML.Graphics;
using SFML.System;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{
    public class ViewModel : IViewModel
    {
        private List<Brick> bricksToDraw = new List<Brick>();

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

        public Sprite Background { get; set; }
        public List<Brick> Bricks => bricksToDraw;
        public Font Font { get; set; }

        public Text LiveAndScoresMessage
        {
            get { return PrepareLifesAndScoresMessage(); }
        }

        public Text Title { get; set; }
        public Text GameOverMessage { get; set; }
        public Text PressButtonToPlayMessage { get; set; }

        public ViewModel(IGame game)
        {
            this.game = game;

            Font = LoadFont("Bungee-Regular.ttf");
            Title = PrepareTitle();
            GameOverMessage = PrepareGameOverMessage();
            PressButtonToPlayMessage = PreparePressButtonToPlayMessage();
        }

        public void Dispose()
        {
            Font?.Dispose();
            backgroundImage?.Dispose();
        }

        public void PrepareBricksToDraw()
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
                        rectangle.Position = new Vector2f(brick.Boundary.Min.X, brick.Boundary.Min.Y);
                        rectangle.Size = new Vector2f(brick.Boundary.Size.X, brick.Boundary.Size.Y);
                        rectangle.FillColor = color;

                        Brick brickToDraw = new Brick(rectangle);
                        bricksToDraw.Add(brickToDraw);
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

        public Font LoadFont(string name)
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

        private Text PrepareTitle()
        {
            uint charSize = 50;
            Text message = new Text("zbrozonoid", Font, charSize);
            message.Color = Color.White;

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();
            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, (float)height / 6 - localBounds.Height / 3);
            message.Position = rect;

            return message;
        }

        private Text PrepareGameOverMessage()
        {
            uint charSize = 50;
            Text message = new Text("game over", Font, charSize);
            message.Color = Color.White;

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();
            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, (float)height / 6 - localBounds.Height / 2);
            message.Position = rect;

            return message;
        }

        private Text PrepareLifesAndScoresMessage()
        {
            uint charSize = 20;
            int lifes = game.GameState.Lifes >= 0 ? game.GameState.Lifes : 0;
            int scores = game.GameState.Scores;
            Text message = new Text($"Lifes: {lifes}   Scores: {scores:D5}", Font, charSize);
            message.Color = Color.White;

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();

            int offsetX = 20;
            int offsetY = 30;
            Vector2f rect = new Vector2f(offsetX, (float)height - localBounds.Height - offsetY);
            message.Position = rect;

            return message;
        }

        private Text PreparePressButtonToPlayMessage()
        {
            uint charSize = 50;
            Text message = new Text("Press mouse button to play", Font, charSize);
            message.Color = Color.White;

            game.GetScreenSize(out int width, out int height);
            FloatRect localBounds = message.GetLocalBounds();
            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, (float)height / 4 - localBounds.Height / 2);
            message.Position = rect;

            return message;
        }


    }
}
