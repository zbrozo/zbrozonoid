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
using System.Linq;
using SFML.Graphics;
using SFML.System;
using zbrozonoid.Menu;
using zbrozonoid.Views;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{

    public class DrawGameObjects : IDrawGameObjects
    {
        public RenderWindow Render { get; private set; }
        public IPrepareTextLine PrepareTextLine { get; private set; }
        public IGame game { get; private set; }

        private IViewModel viewModel;
        private IMenuViewModel menuViewModel;

        public DrawGameObjects(RenderWindow renderWindow, IPrepareTextLine prepareTextLine, IViewModel viewModel, IMenuViewModel menuViewModel, IGame game)
        {
            this.Render = renderWindow;
            this.game = game;
            this.viewModel = viewModel;
            this.PrepareTextLine = prepareTextLine;
            this.menuViewModel = menuViewModel;
        }

        public void DrawGameOver()
        {
            Render.Draw(viewModel.GameOverMessage);
        }

        public void DrawPressPlayToPlay()
        {
            Render.Draw(viewModel.PressButtonToPlayMessage);
        }

        public void DrawStopPlayMessage()
        {
            Render.Draw(viewModel.StopPlayMessage);
        }

    }
}
