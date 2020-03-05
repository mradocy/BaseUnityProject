using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.RewiredExtensions {

    /// <summary>
    /// Class that maps Unity <see cref="KeyCode"/>s to other information.
    /// </summary>
    public class KeyboardMap {

        /// <summary>
        /// Gets the index of the text frame corresponding to the given keycode in the keyboard text icon textures.
        /// </summary>
        /// <param name="keyCode">KeyCode</param>
        /// <returns>image index</returns>
        public int GetTextImageIndex(KeyCode keyCode) {
            int imageIndex;
            if (_keycodeTextMapping.TryGetValue(keyCode, out imageIndex)) {
                return imageIndex;
            }
            return 0;
        }

        /// <summary>
        /// Gets the index of the background frame corresponding to the given keycode in the keyboard background icon textures.
        /// </summary>
        /// <param name="keyCode">KeyCode</param>
        /// <returns>image index</returns>
        public int GetBackgroundImageIndex(KeyCode keyCode) {
            switch (keyCode) {
            // wide background
            case KeyCode.Backspace:
            case KeyCode.Tab:
            case KeyCode.RightShift:
            case KeyCode.LeftShift:
            case KeyCode.RightControl:
            case KeyCode.LeftControl:
            case KeyCode.RightCommand:
            case KeyCode.LeftCommand:
                return 1;
            // enter background
            case KeyCode.Return:
                return 2;
            // space background
            case KeyCode.Space:
                return 3;
            // square background
            default:
                return 0;
            }
        }

        /// <summary>
        /// Mapping from <see cref="KeyCode"/> to text image index.
        /// </summary>
        private static Dictionary<KeyCode, int> _keycodeTextMapping = new Dictionary<KeyCode, int>() {
            [KeyCode.Backspace] = 1,
            [KeyCode.Tab] = 2,
            [KeyCode.Clear] = 3,
            [KeyCode.Return] = 4,
            [KeyCode.Pause] = 5,
            [KeyCode.Escape] = 6,
            [KeyCode.Space] = 7,
            [KeyCode.Exclaim] = 8,
            [KeyCode.DoubleQuote] = 9,
            [KeyCode.Hash] = 10,
            [KeyCode.Dollar] = 11,
            [KeyCode.Percent] = 12,
            [KeyCode.Ampersand] = 13,
            [KeyCode.Quote] = 14,
            [KeyCode.LeftParen] = 15,
            [KeyCode.RightParen] = 16,
            [KeyCode.Asterisk] = 17,
            [KeyCode.Plus] = 18,
            [KeyCode.Comma] = 19,
            [KeyCode.Minus] = 20,
            [KeyCode.Period] = 21,
            [KeyCode.Slash] = 22,
            [KeyCode.Alpha0] = 23,
            [KeyCode.Alpha1] = 24,
            [KeyCode.Alpha2] = 25,
            [KeyCode.Alpha3] = 26,
            [KeyCode.Alpha4] = 27,
            [KeyCode.Alpha5] = 28,
            [KeyCode.Alpha6] = 29,
            [KeyCode.Alpha7] = 30,
            [KeyCode.Alpha8] = 31,
            [KeyCode.Alpha9] = 32,
            [KeyCode.Colon] = 33,
            [KeyCode.Semicolon] = 34,
            [KeyCode.Less] = 35,
            [KeyCode.Equals] = 36,
            [KeyCode.Greater] = 37,
            [KeyCode.Question] = 38,
            [KeyCode.At] = 39,
            [KeyCode.LeftBracket] = 40,
            [KeyCode.Backslash] = 41,
            [KeyCode.RightBracket] = 42,
            [KeyCode.Caret] = 43,
            [KeyCode.Underscore] = 44,
            [KeyCode.BackQuote] = 45,
            [KeyCode.A] = 46,
            [KeyCode.B] = 47,
            [KeyCode.C] = 48,
            [KeyCode.D] = 49,
            [KeyCode.E] = 50,
            [KeyCode.F] = 51,
            [KeyCode.G] = 52,
            [KeyCode.H] = 53,
            [KeyCode.I] = 54,
            [KeyCode.J] = 55,
            [KeyCode.K] = 56,
            [KeyCode.L] = 57,
            [KeyCode.M] = 58,
            [KeyCode.N] = 59,
            [KeyCode.O] = 60,
            [KeyCode.P] = 61,
            [KeyCode.Q] = 62,
            [KeyCode.R] = 63,
            [KeyCode.S] = 64,
            [KeyCode.T] = 65,
            [KeyCode.U] = 66,
            [KeyCode.V] = 67,
            [KeyCode.W] = 68,
            [KeyCode.X] = 69,
            [KeyCode.Y] = 70,
            [KeyCode.Z] = 71,
            [KeyCode.LeftCurlyBracket] = 72,
            [KeyCode.Pipe] = 73,
            [KeyCode.RightCurlyBracket] = 74,
            [KeyCode.Tilde] = 75,
            [KeyCode.Delete] = 76,
            [KeyCode.Keypad0] = 23,
            [KeyCode.Keypad1] = 24,
            [KeyCode.Keypad2] = 25,
            [KeyCode.Keypad3] = 26,
            [KeyCode.Keypad4] = 27,
            [KeyCode.Keypad5] = 28,
            [KeyCode.Keypad6] = 29,
            [KeyCode.Keypad7] = 30,
            [KeyCode.Keypad8] = 31,
            [KeyCode.Keypad9] = 32,
            [KeyCode.KeypadPeriod] = 21,
            [KeyCode.KeypadDivide] = 22,
            [KeyCode.KeypadMultiply] = 17,
            [KeyCode.KeypadMinus] = 20,
            [KeyCode.KeypadPlus] = 18,
            [KeyCode.KeypadEnter] = 4,
            [KeyCode.KeypadEquals] = 36,
            [KeyCode.UpArrow] = 77,
            [KeyCode.DownArrow] = 78,
            [KeyCode.RightArrow] = 79,
            [KeyCode.LeftArrow] = 80,
            [KeyCode.Insert] = 81,
            [KeyCode.Home] = 82,
            [KeyCode.End] = 83,
            [KeyCode.PageUp] = 84,
            [KeyCode.PageDown] = 85,
            [KeyCode.F1] = 86,
            [KeyCode.F2] = 87,
            [KeyCode.F3] = 88,
            [KeyCode.F4] = 89,
            [KeyCode.F5] = 90,
            [KeyCode.F6] = 91,
            [KeyCode.F7] = 92,
            [KeyCode.F8] = 93,
            [KeyCode.F9] = 94,
            [KeyCode.F10] = 95,
            [KeyCode.F11] = 96,
            [KeyCode.F12] = 97,
            [KeyCode.F13] = 98,
            [KeyCode.F14] = 99,
            [KeyCode.F15] = 100,
            [KeyCode.Numlock] = 101,
            [KeyCode.CapsLock] = 102,
            [KeyCode.ScrollLock] = 103,
            [KeyCode.RightShift] = 104,
            [KeyCode.LeftShift] = 105,
            [KeyCode.RightControl] = 106,
            [KeyCode.LeftControl] = 107,
#if UNITY_STANDALONE_OSX
            [KeyCode.RightAlt] = 109,
            [KeyCode.LeftAlt] = 109,
#else
            [KeyCode.RightAlt] = 108,
            [KeyCode.LeftAlt] = 108,
#endif
            [KeyCode.RightCommand] = 110,
            [KeyCode.RightApple] = 110,
            [KeyCode.LeftCommand] = 111,
            [KeyCode.LeftApple] = 111,
            [KeyCode.LeftWindows] = 112,
            [KeyCode.RightWindows] = 112,
            [KeyCode.AltGr] = 113,
            [KeyCode.Help] = 114,
            [KeyCode.Print] = 115,
            [KeyCode.SysReq] = 116,
            [KeyCode.Break] = 117,
            [KeyCode.Menu] = 118,
        };

    }
}