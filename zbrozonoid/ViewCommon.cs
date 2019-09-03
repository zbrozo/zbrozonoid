using System;
using System.IO;
using System.Reflection;
using SFML.Graphics;

namespace zbrozonoid
{
    public class ViewCommon : IDisposable
    {
        public Font Font { get; set; }
        public RenderWindow RenderWindow { get; set; }
        public int LineHeight { get; } = 40;

        private Image backgroundImage;
        public Sprite Background { get; set; }

        public ViewCommon(RenderWindow renderWindow)
        {
            RenderWindow = renderWindow;
            Font = LoadFont("Bungee-Regular.ttf");
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
            Font?.Dispose();
            backgroundImage?.Dispose();
        }
    }
}
