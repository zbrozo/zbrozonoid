﻿/*
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
using zbrozonoidLibrary;

namespace zbrozonoidEngine.Interfaces
{
    public interface ICollisionManager
    {
        CollisionFlags Flags { get; }

        bool Detect(IBoundary first, IBoundary second);
        void Bounce(List<IBrick> bricksHit, IBoundary obstacle, IBall ball);

        void LogData();
    }
}
