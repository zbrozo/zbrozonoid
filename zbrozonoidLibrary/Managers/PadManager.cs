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
namespace zbrozonoidLibrary.Managers
{
    using System.Collections;
    using System.Collections.Generic;

    using zbrozonoidLibrary.Interfaces;

    public class PadManager : IPadManager
    {
        private readonly Dictionary<Edge, IPad> pads = new Dictionary<Edge, IPad>();
        private IScreen screen;

        public PadManager(IScreen screen)
        {
            this.screen = screen;
        }

        public void Add(Edge edge)
        {
            IPad pad = new Pad();
            pad.SetSize(100, 24);

            int offset = 50;

            switch (edge)
            {
                case Edge.Top:
                    {
                        (pad as IElement).PosY = offset;
                        break;
                    }
                case Edge.Bottom:
                    {
                        (pad as IElement).PosY = screen.Height - offset;
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


        public IEnumerator<IPad> GetEnumerator()
        {
            return pads.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            //forces use of the non-generic implementation on the Values collection
            return ((IEnumerable)pads.Values).GetEnumerator();
        }
    }
}
