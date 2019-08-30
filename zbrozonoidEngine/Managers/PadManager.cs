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
namespace zbrozonoidEngine.Managers
{
    using System.Collections;
    using System.Collections.Generic;

    using zbrozonoidEngine.Interfaces;

    public class PadManager : IPadManager
    {
        private readonly Dictionary<Edge, IPad> pads = new Dictionary<Edge, IPad>();
        private IScreen screen;

        public PadManager(IScreen screen)
        {
            this.screen = screen;
        }

        public void Create(IGameConfig config)
        {
            pads.Clear();

            if (config.Players == 2)
            {
                Add(Edge.Top);
                Add(Edge.Bottom);
            }
            else if (config.Players == 1)
            {
                Add(Edge.Bottom);
            }
        }

        public void Add(Edge edge)
        {
            IPad pad = new Pad();

            int width = 100;
            int height = 24;
            pad.SetSize(width, height);

            int offset = 50;

            switch (edge)
            {
                case Edge.Top:
                    {
                        pad.Boundary.Min = new Vector2(pad.Boundary.Min.X, offset);
                        //pad.PosY = offset;
                        break;
                    }
                case Edge.Bottom:
                    {
                        pad.Boundary.Min = new Vector2(pad.Boundary.Min.X, screen.Height - height - offset);
                        //(pad as IElement).PosY = screen.Height - height - offset;
                        break;
                    }
                default:
                    break;
            }

            pads.Add(edge, pad);
        }

        public IPad GetFirst()
        {
            var e = pads.GetEnumerator();
            e.MoveNext();
            return e.Current.Value;
        }

        public void Clear()
        {
            pads.Clear();
        }

        public void SetBallStartPosition(IPad pad, IBall ball)
        {
            bool found = FindEdge(pad, out Edge edge);

            if (!found)
            {
                return;
            }

            int x = pad.Boundary.Min.X + pad.Boundary.Size.X / 2 - ball.Boundary.Size.X / 2;

            if (edge == Edge.Top)
            {
                int y = pad.Boundary.Max.Y;
                ball.Boundary.Min = new Vector2(x, y);
                ball.InitStartPosition();
            }
            else if (edge == Edge.Bottom)
            {
                int y = pad.Boundary.Min.Y - ball.Boundary.Size.Y;
                ball.Boundary.Min = new Vector2(x, y);
                ball.InitStartPosition();
            }
        }

        public void RestartBallYPosition(IPad pad, IBall ball)
        {
            bool found = FindEdge(pad, out Edge edge);

            if (!found)
            {
                return;
            }

            if (edge == Edge.Top)
            {
                ball.SetYPosition(pad.Boundary.Max.Y);
            }
            else if (edge == Edge.Bottom)
            {
                ball.SetYPosition(pad.Boundary.Min.Y - ball.Boundary.Size.Y);
            }
        }

        public IEnumerator<IPad> GetEnumerator()
        {
            return pads.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (pads.Values).GetEnumerator();
        }

        private bool FindEdge(IPad pad, out Edge edge)
        {
            bool found = false;
            edge = Edge.Bottom;

            foreach (var pair in pads)
            {
                if (pair.Value == pad)
                {
                    found = true;
                    edge = pair.Key;
                    break;
                }
            }

            return found;
        }

    }
}
