﻿using SFML.Graphics;
using SFML.System;
using zbrozonoid.Views;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.Menu
{
    public class MenuView : IView
    {
        private readonly IPrepareTextLine writer;
        private readonly IMenuViewModel menuViewModel;

        public MenuView(IPrepareTextLine writer, 
                        IMenuViewModel menuViewModel) 
        {
            this.writer = writer;
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
                Text name = writer.PrepareMenuItem(item.Name, i, isCurrent);
                writer.Render.Draw(name);
                ++i;
            }
        }
    }
}
