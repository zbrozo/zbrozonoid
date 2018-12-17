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
namespace zbrozonoid
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Reflection;

    using zbrozonoidLibrary;
    using zbrozonoidLibrary.Interfaces;

    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();

        Dictionary<int, Color> colors = new Dictionary<int, Color>
                                            {
                                                {  0, Color.Black },
                                                {  1, Color.White },
                                                {  2, Color.Red },
                                                {  3, Color.Cyan }, 
                                                {  4, Color.Violet },
                                                {  5, Color.Green },
                                                {  6, Color.Blue },
                                                {  7, Color.Yellow },
                                                {  8, Color.Orange },
                                                {  9, Color.Brown },
                                                { 10, Color.FromArgb(255,119,119) }, // LightRed
                                                { 11, Color.DarkGray },
                                                { 12, Color.Gray },
                                                { 13, Color.LightGreen },
                                                { 14, Color.LightBlue },
                                                { 15, Color.LightGray }
                                            };

        private static Point prevPoint;

        private BufferedGraphicsContext context;
        private BufferedGraphics grafx;

        private SolidBrush brush = new SolidBrush(Color.Orange);

        private IGame Game { get; set; }

        //private PictureBox pb1 = new PictureBox();
        private Image background;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys key);


        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            //bool success = User32.GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }

        public Form1()
        {
            InitializeComponent();

            t.Interval = 25;
            t.Tick += new EventHandler(TimerEventHandler);
            t.Start();

            Load += LoadHandler;
//            PreviewKeyDown += PreviewKeyDownHandler;
           // KeyDown += KeyDownHandler;
            SizeChanged += WindowSizeChanged;

            Point point = GetCursorPosition();
            prevPoint = point;

            //Paint += new System.Windows.Forms.PaintEventHandler(MainForm_Paint);
        }

        public void OnChangeBackground(object sender, BackgroundEventArgs e)
        {
            if (background != null)
            {
                background.Dispose();
            }

            LoadBackground(e.Value);
        }

        private void LoadHandler(object sender, EventArgs e)
        {
            KeyPreview = true;
        }

        private void TimerEventHandler(object sender, EventArgs e)
        {
            Point point = GetCursorPosition();
            Game.SetPadMove(point.X - prevPoint.X);
            prevPoint = point;

            Game.Action();
            UpdateScreen();
            grafx.Render(Graphics.FromHwnd(this.Handle));
        }


        public void SetGame(IGame game)
        {
            Game = game;

            Game.GetScreenSize(out int width, out int height);
            this.ClientSize = new Size(width, height);
            Game.SetScreenSize(ClientSize.Width, ClientSize.Height);

            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(ClientSize.Width + 1, ClientSize.Height + 1);

            grafx = context.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));

            LoadBackground(game.GetBackgroundPath());
        }

        private void WindowSizeChanged(object sender, EventArgs e)
        {
            Game.SetScreenSize(ClientSize.Width, ClientSize.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.RControlKey)
            {
                UpdateScreen();
            }
        }

        private void PreviewKeyDownHandler(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Left:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
            }

            if (e.KeyCode == Keys.Q)
            {
            }
        }

        private void UpdateScreen()
        {
            grafx.Graphics.DrawImage(background, 0,0, 1024, 768);

            //ClearScreen(grafx.Graphics);

            DrawBorders(grafx.Graphics);
            DrawPad(grafx.Graphics);
            DrawBall(grafx.Graphics);
            DrawBricks(grafx.Graphics);
        }

        private void MainForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            grafx.Graphics.DrawImage(background, 0, 0, 1024, 768);

            DrawBorders(grafx.Graphics);
            DrawPad(grafx.Graphics);
            DrawBall(grafx.Graphics);
            DrawBricks(grafx.Graphics);
        }

        /*
        private void ClearScreen(Graphics g)
        {
            SolidBrush myBrush = new SolidBrush(Color.Gray);
            g.FillRectangle(myBrush, new Rectangle(0, 0, Size.Width, Size.Height));
            myBrush.Dispose();
        }
        */

        private void DrawPad(Graphics g)
        {
            Game.GetPadPosition(out int posX, out int posY);
            Game.GetPadSize(out var width, out var height);

            LinearGradientBrush linGrBrush = new LinearGradientBrush(
                new Point(0, 20),
                new Point(0, 70),
                Color.LightBlue,   // Opaque red
                Color.Blue);  // Opaque blue

            g.FillRectangle(linGrBrush, new Rectangle(posX, posY, width, height));
            linGrBrush.Dispose();
        }

        private void DrawBall(Graphics g)
        {
            var ballManager = Game.GetBallManager();
            foreach(IBall ball in ballManager)
            {
                ball.GetPosition(out var posX , out var posY);
                ball.GetSize(out var width, out var height);

                DrawTail(g, ball);

                SolidBrush myBrush = new SolidBrush(Color.LightCoral);
                g.FillEllipse(myBrush, new Rectangle(posX, posY, width, height));
                //g.FillRectangle(myBrush, new Rectangle(posX, posY, width, height));

                myBrush.Dispose();
            }
        }

        private void DrawTail(Graphics g, IBall ball)
        {
            ITail tail = Game.TailManager.Find(ball);
            if (tail != null)
            {
                int i = 0;
                int opacity = 150;
                foreach(Position position in tail)
                {
                    ++i;
                    if (i % 12 == 0)
                    {
                        if (opacity < 0)
                        {
                            opacity = 0;
                        }

                            ball.GetSize(out var width, out var height);

                            SolidBrush tailBrush = new SolidBrush(Color.FromArgb(opacity, Color.LightCoral));
                            g.FillEllipse(
                                tailBrush,
                                new Rectangle(position.X, position.Y, width, height));
                            tailBrush.Dispose();

                        opacity = opacity - 40;

                    }
                }
            }
        }

        private void DrawBorders(Graphics g)
        {
            var borderManager = Game.GetBorderManager();

            foreach(IBorder border in borderManager)
            {
                var element = border as IElement;

                System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Bisque);
                g.FillRectangle(myBrush, new Rectangle(element.PosX, element.PosY, element.Width, element.Height));
                myBrush.Dispose();

            }
        }

        private void DrawBricks(Graphics g)
        {
            List<IBrick> bricks = Game.GetBricks();

            foreach(var entry in bricks)
            {
                IBrick brick = entry;

                if (!brick.Hit && brick.Type > 0)
                {
                    if (colors.TryGetValue((int)brick.ColorNumber, out Color color))
                    {
                        brush.Color = color;
                        g.FillRectangle(brush, new Rectangle(brick.PosX, brick.PosY, brick.Width, brick.Height));
                    }
                }
            }

        }

        private void LoadBackground(string name)
        {
            name = name.Replace("/", ".");
            name = "zbrozonoidAssets." + name;

            AssemblyName assemblyName = new AssemblyName(@"zbrozonoidAssets");
            Assembly assembly = Assembly.Load(assemblyName);

            using (Stream resourceStream = assembly.GetManifestResourceStream(name))
            {
                if (resourceStream == null)
                    return;

                background = Image.FromStream(resourceStream);
            }
        }

    }
}
