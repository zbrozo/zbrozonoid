using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using zbrozonoidEngine.Interfaces;
using zbrozonoid.Models;
using System.Reflection;
using System.IO;

namespace zbrozonoid.Views
{
    public class GamePlayfieldView : IGamePlayfieldView
    {
        private IGame game;
        private IDrawGameObjects draw;

        private Image backgroundImage;
        public Sprite Background { get; set; }

        private readonly GamePlayfieldModel model = new GamePlayfieldModel();

        public List<Brick> Bricks { get; } = new List<Brick>();

        private Text LiveAndScoresMessage
        {
            get { return PrepareLifesAndScoresMessage(); }
        }

        private Text FasterBallMessage
        {
            get { return PrepareFasterBallMessage(); }
        }

        private Text FireBallMessage
        {
            get { return PrepareFireBallMessage(); }
        }

        public GamePlayfieldView(IDrawGameObjects draw)
        {
            this.draw = draw;
            this.game = draw.game;
        }

        public void Display()
        {
            DrawBackground(Background);
            DrawBorders();
            DrawBricks();

            DrawLifesAndScoresInfo();
            DrawFasterBallTimer();
            DrawFireBallTimer();
        }

        public void DrawBackground(Sprite background)
        {
            draw.Render.Draw(background);
        }

        private void DrawLifesAndScoresInfo()
        {
            draw.Render.Draw(LiveAndScoresMessage);
        }

        private void DrawFasterBallTimer()
        {
            if (FasterBallMessage.DisplayedString.Length > 0)
            {
                draw.Render.Draw(FasterBallMessage);
            }
        }

        private void DrawFireBallTimer()
        {
            if (FireBallMessage.DisplayedString.Length > 0)
            {
                draw.Render.Draw(FireBallMessage);
            }
        }

        private void DrawBricks()
        {
            foreach (var brick in Bricks.Where(x => x.IsVisible))
            {
                draw.Render.Draw(brick.Rect);
            }
        }

        public void DrawBorders()
        {
            var borderManager = game.BorderManager;

            foreach (IBorder border in borderManager)
            {
                RectangleShape rectangle = new RectangleShape();
                rectangle.Position = new Vector2f(border.Boundary.Min.X, border.Boundary.Min.Y);
                rectangle.Size = new Vector2f(border.Boundary.Size.X, border.Boundary.Size.Y);
                rectangle.FillColor = Color.White;

                draw.Render.Draw(rectangle);
            }
        }


        private Text PrepareLifesAndScoresMessage()
        {
            const uint charSize = 20;
            int lifes = game.GameState.Lifes >= 0 ? game.GameState.Lifes : 0;
            int scores = game.GameState.Scores;
            return draw.PrepareTextLine.Prepare($"Lifes: {lifes}   Scores: {scores:D5}", 0, false, true, 20, 30, charSize);
        }

        private Text PrepareFasterBallMessage()
        {
            int value = 0;
            foreach (var counter in game.GameState.FasterBallCountdown.Where(x => x.Value > value))
            {
                value = counter.Value;
            }

            const uint charSize = 20;
            return draw.PrepareTextLine.Prepare($"FasterBall: {value}", 0, false, true, 800, 20, charSize);
        }

        private Text PrepareFireBallMessage()
        {
            int value = 0;
            foreach (var counter in game.GameState.FireBallCountdown.Where(x => x.Value > value))
            {
                value = counter.Value;
            }

            const uint charSize = 20;
            return draw.PrepareTextLine.Prepare($"FireBall: {value}", 0, false, true, 800, 40, charSize);
        }

        public void PrepareBricksToDraw()
        {
            Bricks.Clear();

            List<IBrick> bricks = game.Bricks;
            foreach (IBrick brick in bricks.Where(x => x.Type > 0))
            {
                if (model.colors.TryGetValue((int)brick.ColorNumber, out Color color))
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

        public void Dispose()
        {
            backgroundImage?.Dispose();
        }

    }
}
