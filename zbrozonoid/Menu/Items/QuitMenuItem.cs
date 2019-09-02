using System;

namespace zbrozonoid.Menu.Items
{
    public class QuitMenuItem : IMenuItem
    {
        public string Name => "Quit";

        private readonly Action Close;

        public QuitMenuItem(Action Close)
        {
            this.Close = Close;
        }

        public void Execute()
        {
            Close();
        }
    }
}
