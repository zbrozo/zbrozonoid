using System;

namespace zbrozonoid.Menu.Items
{
    public class StartMenuItem : IMenuItem
    {
        public string Name => "Start game";

        private readonly Action InGame;

        public StartMenuItem(Action InGame)
        {
            this.InGame = InGame;
        }

        public void Execute()
        {
            InGame();
        }
    }
}
