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
using System.Linq;
using System.Reflection;
using SFML.Graphics;
using SFML.System;
using zbrozonoid.Views;
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
        public Text TitleMessage { get; set; }
        public Text StopPlayMessage { get; set; }

        private Image backgroundImage;
        public Sprite Background { get; set; }

        private readonly RenderWindow render;
        private IPrepareTextLine prepareTextLine;
        public ViewModel(IPrepareTextLine prepareTextLine, IGame game)
        {
            render = prepareTextLine.Render;
            this.prepareTextLine = prepareTextLine;
            this.game = game;
    
            GameOverMessage = PrepareGameOverMessage();
            PressButtonToPlayMessage = PreparePressButtonToPlayMessage();
            TitleMessage = PrepareTitle();
            StopPlayMessage = PrepareStopPlayMessage();
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

        public void PrepareBricksToDraw()
        {
            Bricks.Clear();

            List<IBrick> bricks = game.Bricks;
            foreach (IBrick brick in bricks.Where(x => x.Type > 0))
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

        private Text PrepareGameOverMessage()
        {
            return prepareTextLine.Prepare("game over", 4);
        }

        private Text PrepareLifesAndScoresMessage()
        {
            const uint charSize = 20;
            int lifes = game.GameState.Lifes >= 0 ? game.GameState.Lifes : 0;
            int scores = game.GameState.Scores;
            return prepareTextLine.Prepare($"Lifes: {lifes}   Scores: {scores:D5}", 0, false, true, 20, 30, charSize);
        }

        private Text PreparePressButtonToPlayMessage()
        {
            return prepareTextLine.Prepare("Press mouse button to play", 4);
        }

        private Text PrepareTitle()
        {
            return prepareTextLine.Prepare("zbrozonoid", 0);
        }

        private Text PrepareFasterBallMessage()
        {
            int value = 0;
            foreach (var counter in game.GameState.FasterBallCountdown.Where(x => x.Value > value))
            {
                value = counter.Value;
            }

            const uint charSize = 20;
            return prepareTextLine.Prepare($"FasterBall: {value}", 0, false, true, 800, 20, charSize);
        }

        private Text PrepareFireBallMessage()
        {
            int value = 0;
            foreach (var counter in game.GameState.FireBallCountdown.Where(x => x.Value > value))
            {
                value = counter.Value;
            }

            const uint charSize = 20;
            return prepareTextLine.Prepare($"FireBall: {value}", 0, false, true, 800, 40, charSize);
        }

        private Text PrepareStopPlayMessage()
        {
            return prepareTextLine.Prepare("Stop play (y/n)", 4);
        }

        public void Dispose()
        {
            backgroundImage?.Dispose();
        }

    }
}
