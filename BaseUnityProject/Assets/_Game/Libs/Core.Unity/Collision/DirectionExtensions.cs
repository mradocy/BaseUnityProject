using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.Collision {

    public static class DirectionExtensions {

        public static Direction GetOpposite(this Direction direction) {
            switch (direction) {
            case Direction.Right:
                return Direction.Left;
            case Direction.Up:
                return Direction.Down;
            case Direction.Left:
                return Direction.Right;
            case Direction.Down:
                return Direction.Up;
            default:
                return Direction.None;
            }
        }

        /// <summary>
        /// Returns a Vector2 of length 1 in this direction.
        /// </summary>
        public static Vector2 AsVector2(this Direction direction) {
            switch (direction) {
            case Direction.Right:
                return Vector2.right;
            case Direction.Up:
                return Vector2.up;
            case Direction.Left:
                return Vector2.left;
            case Direction.Down:
                return Vector2.down;
            default:
                return Vector2.zero;
            }
        }
    }
}