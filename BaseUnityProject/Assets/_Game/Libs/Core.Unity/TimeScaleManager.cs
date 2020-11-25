using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Handles setting <see cref="Time.timeScale"/> from different sources.
    /// Tokens can be created and destroyed, each specifying a time scale.
    /// The time scale that's applied is the token with the highest priority.
    /// </summary>
    public static class TimeScaleManager {

        /// <summary>
        /// Creates a new token with the given priority.  Returns the id of the token.
        /// Ids will always be positive.
        /// </summary>
        /// <param name="priority">Priority</param>
        public static int CreateToken(int priority, float timeScale) {
            TokenClass tokenClass;
            if (_recycledTokens.Count > 0) {
                tokenClass = _recycledTokens.Pop();
            } else {
                tokenClass = new TokenClass();
            }

            tokenClass.Id = _lastTokenId++;
            tokenClass.Priority = priority;
            tokenClass.TimeScale = timeScale;
            _tokens.Add(tokenClass);

            UpdateTimeScale(true);

            return tokenClass.Id;
        }

        /// <summary>
        /// Gets the time scale of the given token.
        /// </summary>
        public static float GetTokenTimeScale(int tokenId) {
            TokenClass tokenClass = GetTokenClass(tokenId);
            if (tokenClass == null) {
                Debug.LogError(string.Format(_tokenNotFoundError, tokenId));
                return 1;
            }

            return tokenClass.TimeScale;
        }

        /// <summary>
        /// Sets the time scale of the given token.
        /// </summary>
        public static void SetTokenTimeScale(int tokenId, float timeScale) {
            TokenClass tokenClass = GetTokenClass(tokenId);
            if (tokenClass == null) {
                Debug.LogError(string.Format(_tokenNotFoundError, tokenId));
                return;
            }

            if (tokenClass.TimeScale == timeScale)
                return;

            tokenClass.TimeScale = timeScale;
            UpdateTimeScale(false);
        }

        /// <summary>
        /// Gets the priority of the given token.
        /// </summary>
        public static int GetTokenPriority(int tokenId) {
            TokenClass tokenClass = GetTokenClass(tokenId);
            if (tokenClass == null) {
                Debug.LogError(string.Format(_tokenNotFoundError, tokenId));
                return 0;
            }

            return tokenClass.Priority;
        }

        /// <summary>
        /// Sets the priority of the given token.
        /// </summary>
        public static void SetTokenPriority(int tokenId, int priority) {
            TokenClass tokenClass = GetTokenClass(tokenId);
            if (tokenClass == null) {
                Debug.LogError(string.Format(_tokenNotFoundError, tokenId));
                return;
            }

            if (tokenClass.Priority == priority)
                return;

            tokenClass.Priority = priority;
            UpdateTimeScale(true);
        }

        /// <summary>
        /// Destroys the token with the given id.
        /// The id can then no longer be used to describe a token.
        /// </summary>
        /// <returns>-1, used to represent a null or invalid token.</returns>
        public static int DestroyToken(int tokenId) {
            TokenClass tokenClass = GetTokenClass(tokenId);
            if (tokenClass == null) {
                // no error if not found
                return -1;
            }

            _tokens.Remove(tokenClass);
            _recycledTokens.Push(tokenClass);
            UpdateTimeScale(false);
            return -1;
        }

        #region Private

        private static TokenClass GetTokenClass(int tokenId) {
            foreach (TokenClass tokenClass in _tokens) {
                if (tokenClass.Id == tokenId)
                    return tokenClass;
            }

            return null;
        }

        private static void UpdateTimeScale(bool sortFirst) {
            if (_tokens.Count <= 0) {
                Time.timeScale = 1;
                return;
            }

            if (sortFirst) {
                _tokens.Sort();
            }
            Time.timeScale = _tokens[_tokens.Count - 1].TimeScale;
        }

        private const string _tokenNotFoundError = "No token with id {0} exists";

        private class TokenClass : System.IComparable<TokenClass> {
            public int Id;
            public int Priority = 0;
            public float TimeScale = 1;

            public int CompareTo(TokenClass other) {
                return Priority - other.Priority;
            }
        }

        private static int _lastTokenId = 1;
        private static List<TokenClass> _tokens = new List<TokenClass>();
        private static Stack<TokenClass> _recycledTokens = new Stack<TokenClass>();

        #endregion
    }
}