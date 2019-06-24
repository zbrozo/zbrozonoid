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
namespace zbrozonoidEngine
{
    using System;

    using zbrozonoidEngine.Interfaces;

    public class RandomGenerator : IRandomGenerator
    {
        private readonly Random random = new Random();

        public int GenerateSpeed()
        {
            int speedMin = 4;
            int speedMax = 16;
            return random.Next(speedMin, speedMax);
        }

        public int GenerateDirection()
        {
            int min = 0;
            int max = 1;
            int pos = random.Next(min, max);
            return pos == 0 ? -1 : 1;
        }

        public int GenerateDegree(int Degree, DegreeType type)
        {
            const int degreeMargin = 10;
            const int degreeRangeMax = 30;
            int degree = random.Next(degreeRangeMax);
            Degree = degree - degreeRangeMax / 2;
            Degree += degreeRangeMax / 2;

            switch (type)
            {
                case DegreeType.Corner:
                    Degree += degreeMargin;
                    break;

                case DegreeType.Average:
                    Degree += degreeRangeMax;
                    break;

                case DegreeType.Centre:
                    Degree += degreeRangeMax * 2;
                    Degree -= degreeMargin;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unknwon type of DegreeType");

            }
            return Degree;
        }
    }
}
