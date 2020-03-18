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
using SFML.Graphics;
using zbrozonoid.Views;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid
{
    public class ViewModel : IViewModel
    {
        private IGame game;
        public Text GameOverMessage { get; set; }
        public Text PressButtonToPlayMessage { get; set; }
        public Text StopPlayMessage { get; set; }

        private readonly RenderWindow render;
        private IPrepareTextLine prepareTextLine;
        public ViewModel(IPrepareTextLine prepareTextLine, IGame game)
        {
            render = prepareTextLine.Render;
            this.prepareTextLine = prepareTextLine;
            this.game = game;
    
            GameOverMessage = PrepareGameOverMessage();
            PressButtonToPlayMessage = PreparePressButtonToPlayMessage();
            StopPlayMessage = PrepareStopPlayMessage();
        }

        private Text PrepareGameOverMessage()
        {
            return prepareTextLine.Prepare("game over", 4);
        }

        private Text PreparePressButtonToPlayMessage()
        {
            return prepareTextLine.Prepare("Press mouse button to play", 4);
        }

        private Text PrepareStopPlayMessage()
        {
            return prepareTextLine.Prepare("Stop play (y/n)", 4);
        }

        public void Dispose()
        {
        }

    }
}
