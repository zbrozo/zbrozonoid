using System.IO;
using System.Reflection;
using SFML.Graphics;
using SFML.System;

namespace zbrozonoid.Views
{
    public class PrepareTextLine : IPrepareTextLine
    {
        private Font Font { get; set; }

        private readonly int LineHeight = 40;

        public RenderWindow Render { get; private set; }

        private readonly uint width;
        private readonly uint height;

        public PrepareTextLine(RenderWindow render)
        {
            Render = render;

            width = Render.Size.X;
            height = Render.Size.Y;

            Font = LoadFont("Bungee-Regular.ttf");
        }

        public Text PrepareMenuItem(string name, int number, bool isCurrent)
        {
            uint charSize = 50;
            Text message = new Text(name, Font, charSize);

            if (!isCurrent)
                message.FillColor = Color.White;
            else
                message.FillColor = Color.Green;

            message.OutlineColor = Color.Black;
            message.OutlineThickness = 2;

            FloatRect localBounds = message.GetLocalBounds();
            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, LineHeight * number);
            message.Position = rect;

            return message;
        }

        /*
        public Text Prepare(string text, int lineNumber, uint fontSize = 50)
        {
            uint charSize = fontSize;
            Text message = new Text(text, Font, charSize);
            message.FillColor = Color.White;

            FloatRect localBounds = message.GetLocalBounds();

            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, LineHeight * lineNumber);
            message.Position = rect;

            return message;
        }
        */

        public Text Prepare(string text, int lineNumber, 
                            bool horizCenter = true, 
                            bool vertReverse = false, 
                            int offsetX = 0, 
                            int offsetY = 0,
                            uint fontSize = 50)
        {
            uint charSize = fontSize;
            Text message = new Text(text, Font, charSize)
            {
                FillColor = Color.White,
                OutlineColor = Color.Black,
                OutlineThickness = 2
            };

            FloatRect localBounds = message.GetLocalBounds();

            float x = 0;
            float y = 0;

            if (horizCenter)
            {
                x = (width - localBounds.Width) / 2;
            }
            else
            {
                x = offsetX;
            }

            if (vertReverse)
            {
                y = height - localBounds.Height - offsetY;
            }
            else
            {
                y = LineHeight * lineNumber;
            }

            Vector2f rect = new Vector2f(x, y);
            message.Position = rect;

            return message;
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


    }
}
