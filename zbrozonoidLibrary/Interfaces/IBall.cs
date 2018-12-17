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
namespace zbrozonoidLibrary.Interfaces
{
    public interface IBall
    {
        int OffsetX { get; set; }
        int OffsetY { get; set; }
        double Degree { get; set; }
        int DirectionX { get; set; }
        int DirectionY { get; set; }
        int Iteration { get; set; }
        int SavedPosX { get; set; }
        int SavedPosY { get; set; }
        int Speed { get; set; }

        void SetSize(int width, int height);
        void GetPosition(out int posX, out int posY);
        void GetSize(out int width, out int height);
        bool MoveBall(bool reverse = false);
        void BounceBigFromLeft();
        void BounceBigFromRight();
        void BounceBigFromTop();
        void BounceBigFromBottom();
        bool Bounce(Edge edge);
        bool BounceCorner(Corner corner);
        void BounceBack();

        void CalculateNewDegree();
        void SavePosition();
        void LogData(bool reverse = false);
    }
}
