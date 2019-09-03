using SFML.Graphics;
using SFML.System;

namespace zbrozonoid
{
    public class TitleMessageView : IView
    {
        private ViewCommon view;
        public Text Title { get; set; }

        public TitleMessageView(ViewCommon viewCommon)
        {
            view = viewCommon;
            Title = PrepareTitle();
        }

        public void Display()
        {
            view.RenderWindow.Draw(Title);
        }

        private Text PrepareTitle()
        {
            uint charSize = 50;
            Text message = new Text("zbrozonoid", view.Font, charSize);
            message.FillColor = Color.White;

            uint width = view.RenderWindow.Size.X;
            uint height = view.RenderWindow.Size.Y;

            FloatRect localBounds = message.GetLocalBounds();
            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, 0);
            message.Position = rect;

            return message;
        }

    }
}
