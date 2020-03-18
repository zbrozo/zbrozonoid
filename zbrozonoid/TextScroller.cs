using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SFML.Graphics;
using SFML.System;

namespace zbrozonoid
{
    public class TextScroller
    {
        string text = "         ===WELCOME TO ZBROZONOID. PROGRAMMING BY TOMASZ ZBROZEK.";


        Dictionary<char, Texture> textures = new Dictionary<char, Texture>();

        Dictionary<char, Sprite> chars = new Dictionary<char, Sprite>();

        private Font font { get; set; }

        private RenderWindow render;
        Texture bitmap;

        private int scrollValue;
        private int scrollChar = 0;
        private int maxText = 40;
        private const int spaceWidth = 50;

        public TextScroller(RenderWindow render)
        {
            this.render = render;

            this.font = LoadFont("Bungee-Regular.ttf");
            //uint fontSize = 130;

           // Image image = new Image(20, 30, Color.Blue);
           // bitmap = this.font.GetTexture(fontSize);

            for (char i = 'A'; i <= 'Z'; i++)
            {
                CreateSprite("" + i);
            }
            CreateSprite(" ");
            CreateSprite(",");
            CreateSprite("-");
            CreateSprite(".");
            CreateSprite("=");


            //bitmap = new Texture(image);
            /*
            {
                string str = "" + i;

                int value = Char.ConvertToUtf32(str, 0);
                Glyph glyph = Font.GetGlyph((uint) value , fontSize, false, 0);
                Image image = new Image((uint) glyph.Bounds.Width, (uint) glyph.Bounds.Height);
                image.Copy(bitmap.CopyToImage(), 0, 0, glyph.TextureRect);
                Texture texture = new Texture(image);
                textures.Add(i, texture);
                Sprite sprite = new Sprite(texture);
                chars.Add(i, sprite);
            }
            */
        }


        public Sprite CreateSprite(string letter)
        {
            Text message = new Text(letter, font, 50)
            {
                FillColor = Color.White
            };

            FloatRect localBounds = message.GetLocalBounds();

            // int value = Char.ConvertToUtf32(str, 0);
            // Glyph glyph = font.GetGlyph((uint)value, 50, false, 0);


            float x = -2;
            float y = -14;

            Vector2f rect = new Vector2f(x, y);
            message.Position = rect;

            RectangleShape rec = new RectangleShape(new Vector2f((uint)localBounds.Width + message.LetterSpacing, (uint)localBounds.Height));
            rec.FillColor = Color.Cyan;
            rec.Position = new Vector2f(0, 0);

            RenderTexture r = new RenderTexture((uint) (localBounds.Width + message.LetterSpacing), (uint) localBounds.Height);
            //render.Draw(rec);

            r.Draw(message);
            r.Display();

            Texture tex = new Texture(r.Texture);

            //textures.Add(letter[0], tex);

            Sprite s = new Sprite(tex);

            chars.Add(letter[0], s);
            return s;

        }

        public void Display()
        {
            char l = text[scrollChar];

            int width = spaceWidth; 
            if (l != ' ')
            {
                width = chars[l].TextureRect.Width;
            }


            if (scrollValue >= width)
            {
                scrollChar++;
                scrollValue = 0;
                if (scrollChar >= text.Length)
                {
                    scrollChar = 0;
                }
            }

            int offset = 0;
            int n = scrollChar;
            for (var i = 0; i < maxText; i++, n++)
            {
                if (n >= text.Length)
                {
                    n = 0;
                }

                char letter = text[n];
                Sprite s = chars[letter];

                s.Position = new Vector2f(offset - scrollValue, 500);

                if (letter == ' ')
                {
                    offset += 50;

                }
                else
                {
                    offset += s.TextureRect.Width;
                }
                render.Draw(s);
            }

            ++scrollValue;
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
