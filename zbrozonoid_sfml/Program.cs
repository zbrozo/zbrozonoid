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
using zbrozonoidLibrary;

namespace zbrozonoid_sfml
{
    static class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            Window window = new Window(game);

            game.OnChangeBackground += window.OnChangeBackground;

            window.Run();
        }
    }
}
