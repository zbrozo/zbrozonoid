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

namespace zbrozonoidLibrary.Interfaces
{
    public interface ICollisionManager
    {
        List<IBrick> bricksHit { get; set; }

        bool XLeftInside { get; set; }
        bool XRightInside { get; set; }
        bool YTopInside { get; set; }
        bool YBottomInside { get; set; }

        bool YTopOutside { get; set; }
        bool YBottomOutside { get; set; }
        bool XLeftOutside { get; set; }
        bool XRightOutside { get; set; }

        bool Detect(IBorder first, IPad second);
        bool Detect(IBorder first, IBall second);
        bool Detect(IPad first, IBall second);
        bool Detect(IBrick first, IBall second);

        void Bounce(IBall ball);
        bool HitBrick(out BrickType type);

        void LogData();

    }
}
