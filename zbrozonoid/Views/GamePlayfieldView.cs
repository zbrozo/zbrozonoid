using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

using SFML.Graphics;
using SFML.System;

using zbrozonoidEngine.Interfaces;
using zbrozonoid.Models;
using NLog;
using Autofac;

namespace zbrozonoid.Views
{
    public class GamePlayfieldView : IGamePlayfieldView
    {
        private IRenderProxy render;
        private IBorderManager borderManager;

        private Image backgroundImage;
        public Sprite Background { get; set; }

        private readonly GamePlayfieldModel model = new GamePlayfieldModel();

        private List<Brick> ViewBricks { get; } = new List<Brick>();
        private readonly ICollection<IBrick> bricks;

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public GamePlayfieldView(
            IRenderProxy render, 
            ICollection<IBrick> bricks,
            IBorderManager borderManager
            )
        {
            this.render = render;
            this.bricks = bricks;
            this.borderManager = borderManager;
        }

        public void Display()
        {
            DrawBackground(Background);
            DrawBorders();
            DrawBricks();
        }

        public void BrickHit(int number)
        {
            ViewBricks[number].IsVisible = false;
        }

        public void DrawBackground(Sprite background)
        {
            render.Draw(background);
        }

        private void DrawBricks()
        {
            foreach (var brick in ViewBricks.Where(x => x.IsVisible))
            {
                render.Draw(brick.Rect);
            }
        }

        public void DrawBorders()
        {
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
            ViewBricks.Clear();
            foreach (IBrick brick in bricks)
            {
                if (model.colors.TryGetValue((int)brick.ColorNumber, out Color color))
                {
                    RectangleShape rectangle = new RectangleShape();
                    rectangle.Position = new Vector2f(brick.Boundary.Min.X, brick.Boundary.Min.Y);
                    rectangle.Size = new Vector2f(brick.Boundary.Size.X, brick.Boundary.Size.Y);
                    rectangle.FillColor = color;

                    Brick brickToDraw = new Brick(rectangle);
                    brickToDraw.IsVisible = brick.Type > 0;
                    ViewBricks.Add(brickToDraw);
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
