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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using zbrozonoidEngine.Interfaces;

    public class PadManager : IPadManager
    {
        private readonly List<Tuple<Edge, uint, IPad>> pads = new List<Tuple<Edge, uint, IPad>>();

        private IScreen screen;

        public PadManager(IScreen screen)
        {
            this.screen = screen;
        }

        public void Create(IGameConfig config, int[] manipulators, Edge playerOneLocation)
        {
            pads.Clear();

            if (config.Players == 2)
            {
                Add(playerOneLocation, (uint) manipulators[0]);
                Add(playerOneLocation == Edge.Bottom ? Edge.Top : Edge.Bottom, (uint) manipulators[1]);
            }
            else if (config.Players == 1)
            {
                Add(playerOneLocation, (uint) manipulators[0]);
            }
        }

        private void Add(Edge edge, uint manipulator)
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
                        break;
                    }
                case Edge.Bottom:
                    {
                        pad.Boundary.Min = new Vector2(pad.Boundary.Min.X, screen.Height - height - offset);
                        break;
                    }
                default:
                    break;
            }

            pads.Add(new Tuple<Edge, uint, IPad>(edge, manipulator, pad));
        }

        public IPad GetFirst()
        {
            var e = pads.GetEnumerator();
            e.MoveNext();
            return e.Current.Item3;
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

        public IEnumerator<Tuple<Edge, uint, IPad>> GetEnumerator()
        {
            return pads.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool FindEdge(IPad pad, out Edge edge)
        {
            var padPair = pads.FirstOrDefault(p => p.Item3 == pad);
            edge = padPair?.Item1 ?? Edge.Bottom;
            return padPair != null;
        }

    }
}
