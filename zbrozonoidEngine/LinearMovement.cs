using System;
using zbrozonoidEngine;
using zbrozonoidLibrary.Interfaces;

namespace zbrozonoidLibrary
{
    public class LinearMovement : IMovement
    {
        private int Iteration { get; set; }
        private Vector2 Offset { get; set; }
        private Vector2 Direction { get; set; }

        public bool Move(bool reverse = false)
        {
            return false;
        }

    }
}
