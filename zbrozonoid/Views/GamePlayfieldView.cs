using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

using SFML.Graphics;
using SFML.System;

using zbrozonoidEngine.Interfaces;
using zbrozonoid.Models;

namespace zbrozonoid.Views
{
    public class GamePlayfieldView : IGamePlayfieldView
    {
        private IGame game;
        private IRenderProxy render;

        private Image backgroundImage;
        public Sprite Background { get; set; }

        private readonly GamePlayfieldModel model = new GamePlayfieldModel();

        public List<Brick> Bricks { get; } = new List<Brick>();

        public GamePlayfieldView(IRenderProxy render, IGame game)
        {
            this.render = render;
            this.game = game;
        }

        public void Display()
        {
            DrawBackground(Background);
            DrawBorders();
            DrawBricks();
        }

        public void BrickHit(int number)
        {
            Bricks[number].IsVisible = false;
        }

        public void DrawBackground(Sprite background)
        {
            render.Draw(background);
        }

        private void DrawBricks()
        {
            foreach (var brick in Bricks.Where(x => x.IsVisible))
            {
                render.Draw(brick.Rect);
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

                render.Draw(rectangle);
            }
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
