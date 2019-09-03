using SFML.Graphics;
using SFML.System;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.Menu
{
    public class MenuView : IView
    {
        private readonly ViewCommon view;
        private readonly IMenuViewModel menuViewModel;

        public MenuView(ViewCommon viewCommon, 
                        IMenuViewModel menuViewModel) 
        {
            this.view = viewCommon;
            this.menuViewModel = menuViewModel;
        }

        public void Display()
        {
            Draw();
        }

        private void Draw()
        {
            IMenuViewModel menu = menuViewModel;
            int i = 2;
            foreach (var item in menu.Items)
            {
                bool isCurrent = (item == menu.CurrentItem);
                Text name = PrepareMenuItem(item.Name, i, isCurrent);
                view.RenderWindow.Draw(name);
                ++i;
            }
        }

        private Text PrepareMenuItem(string name, int number, bool isCurrent)
        {
            uint charSize = 50;
            Text message = new Text(name, view.Font, charSize);

            if (!isCurrent)
                message.FillColor = Color.White;
            else
                message.FillColor = Color.Green;

            uint width = view.RenderWindow.Size.X;
            uint height = view.RenderWindow.Size.Y;

            FloatRect localBounds = message.GetLocalBounds();
            Vector2f rect = new Vector2f((width - localBounds.Width) / 2, view.LineHeight * number);
            message.Position = rect;

            return message;
        }
    }
}
