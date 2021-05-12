using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using TMPro;
using UnityEngine.Events;

namespace Core.Unity.TextMeshPro {

    // reference for TMPro stuff:
    // http://digitalnativestudios.com/forum/index.php?topic=1170.0
    // https://github.com/mdechatech/CharTweener/blob/master/Assets/CharTween/Scripts/CharTweener.cs

    [RequireComponent(typeof(TMP_Text))]
    public class TMProTextHelper : MonoBehaviour {

        #region Constants

        /// <summary>
        /// The maximum number of characters to appear in a TMPro text component
        /// </summary>
        public const int MaxChars = 500;

        #endregion

        #region Events

        /// <summary>
        /// Event invoked when the text of the associated <see cref="TextComponent"/> changes, after the mesh has been updated and properties have been updated.
        /// </summary>
        public event UnityAction TextChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the TMPro text component this helper is for.
        /// </summary>
        public TMP_Text TextComponent {
            get {
                if (_textComponent == null) {
                    _textComponent = this.EnsureComponent<TMP_Text>();
                }
                return _textComponent;
            }
        }

        /// <summary>
        /// Gets or sets the text of the <see cref="TextComponent"/>.
        /// The parsed text is updated and <see cref="TextChanged"/> events are invoked immediately when setting text this way, setting the text of the component directly may take a frame to update.
        /// </summary>
        public string Text {
            get => _textComponent.text;
            set {
                if (value == _textComponent.text)
                    return;
                _textComponent.text = value;
                this.OnTextChangedInner();
            }
        }

        /// <summary>
        /// Gets the parsed text of the text component.
        /// </summary>
        public string ParsedText { get; private set; }

        /// <summary>
        /// Gets the length of <see cref="ParsedText"/>.
        /// </summary>
        public int ParsedTextLength { get; private set; }

        /// <summary>
        /// Gets the number of lines displayed when all characters are visible.
        /// </summary>
        public int LineCount { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the vertex changes at the given index.  These changes can be set and they will be reflected in the text.
        /// List of vertex changes will be automatically sized for this, as long as the number of characters doesn't go above <see cref="MaxChars"/>.
        /// </summary>
        /// <param name="charIndex">Character to get the vertex changes for.</param>
        /// <returns>vertex changes</returns>
        public VertexChanges GetVertexChanges(int charIndex) {
            if (charIndex < 0) {
                Debug.LogError($"char index {charIndex} is invalid.");
                return null;
            }
            if (charIndex >= MaxChars) {
                Debug.LogError($"char index {charIndex} is more than maximum allowed characters {MaxChars}.");
                return null;
            }

            while (charIndex >= _vertexChanges.Count) {
                _vertexChanges.Add(new VertexChanges());
            }

            return _vertexChanges[charIndex];
        }

        /// <summary>
        /// Gets the color changes at the given index.  These changes can be set and they will be reflected in the text.
        /// List of color changes will be automatically sized for this, as long as the number of characters doesn't go above <see cref="MaxChars"/>.
        /// </summary>
        /// <param name="charIndex">Character to get the color changes for.</param>
        /// <returns>color changes</returns>
        public ColorChanges GetColorChanges(int charIndex) {
            if (charIndex < 0) {
                Debug.LogError($"char index {charIndex} is invalid.");
                return null;
            }
            if (charIndex >= MaxChars) {
                Debug.LogError($"char index {charIndex} is more than maximum allowed characters {MaxChars}.");
                return null;
            }

            while (charIndex >= _colorChanges.Count) {
                _colorChanges.Add(new ColorChanges());
            }

            return _colorChanges[charIndex];
        }

        /// <summary>
        /// Gets <see cref="TMP_LinkInfo"/> for all the link tags with the given id in the text.
        /// </summary>
        /// <param name="linkId">The attribute associated with the link tag to search for.</param>
        /// <param name="linkInfos">Array of <see cref="TMP_LinkInfo"/> to fill.</param>
        /// <returns>Number of tags found with the given id.</returns>
        public int GetLinkInfo(string linkId, TMP_LinkInfo[] linkInfos) {
            if (this.TextComponent.textInfo.linkInfo == null)
                return 0;
            if (linkInfos == null || linkInfos.Length == 0) {
                Debug.LogError("given linkInfos must have length");
                return 0;
            }

            int count = 0;
            int allLinksCount = this.TextComponent.textInfo.linkCount;
            for (int i = 0; i < allLinksCount; i++) {
                TMP_LinkInfo li = this.TextComponent.textInfo.linkInfo[i];
                if (li.GetLinkID() == linkId) {
                    if (count >= linkInfos.Length) {
                        Debug.LogError($"Found more linkInfos than the given array's length of {linkInfos.Length}");
                        return count;
                    }
                    linkInfos[count] = li;
                    count++;
                }
            }

            return count;
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Method invoked when the text of this text component changes.
        /// This would be the place to prepare char animations.
        /// </summary>
        protected virtual void OnTextChanged() { }

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// This would be the place to apply char animations.
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// Called by Unity when this object is destroyed.
        /// </summary>
        protected virtual void OnDerivedDestroy() { }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        protected void Awake() {
            _prevText = this.TextComponent.text;
            this.TextComponent.ForceMeshUpdate(true);
            this.RefreshCache();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(this.TextChangedEventReceiver);

            this.OnAwake();
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected void Update() {
            this.OnUpdate();

            this.ApplyVertexChanges();
            this.ApplyColorChanges();
        }

        /// <summary>
        /// Called by Unity when this object is destroyed.
        /// </summary>
        protected void OnDestroy() {
            this.OnDerivedDestroy();

            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(this.TextChangedEventReceiver);
            _textComponent = null;
            _vertexChanges.Clear();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Copies mesh info to the <see cref="_meshCache"/>.
        /// </summary>
        private void RefreshCache() {
            if (string.IsNullOrEmpty(this.TextComponent.text)) {
                this.TextComponent.textInfo.ClearMeshInfo(false); // if this line isn't here, the previous mesh data is copied instead of the new data representing no text
            }
            _meshCache = this.TextComponent.textInfo.CopyMeshInfoVertexData();
        }

        /// <summary>
        /// Applies vertex-related changes to the text characters specified in <see cref="_vertexChanges"/>.
        /// </summary>
        private void ApplyVertexChanges() {
            if (!this.TextComponent.enabled)
                return;

            // change vertices of each character
            TMP_TextInfo textInfo = this.TextComponent.textInfo;
            for (int i = 0; i < textInfo.characterCount; i++) {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                // don't bother if character isn't visible
                if (!charInfo.isVisible)
                    continue;

                // get changes to make to the character
                if (i >= _vertexChanges.Count)
                    break;
                VertexChanges vertexChanges = _vertexChanges[i];
                if (vertexChanges == null)
                    continue;

                // gets the vertices that will be changed
                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // get the original vertices before vertex changes were made
                Vector3[] sourceVertices = _meshCache[materialIndex].vertices;
                Vector3 sourceCenter = new Vector3();
                for (int v = 0; v < 4; v++) {
                    sourceCenter += sourceVertices[vertexIndex + v];
                }
                sourceCenter /= 4;

                // make changes
                Vector3 charOffset = new Vector3(vertexChanges.OffsetX, vertexChanges.OffsetY);
                for (int v = 0; v < 4; v++) {
                    Vector3 vertex = sourceVertices[vertexIndex + v];
                    // rotation
                    float vertexZ = vertex.z;
                    vertex = MathUtils.RotateAroundPoint(vertex, sourceCenter, vertexChanges.Rotation * Mathf.Deg2Rad);
                    vertex.z = vertexZ;
                    // offset
                    vertex += charOffset;

                    vertices[vertexIndex + v] = vertex;
                }
            }

            // update data
            this.TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }

        /// <summary>
        /// Applies color-related changes to the text characters specified in <see cref="_colorChanges"/>.
        /// </summary>
        private void ApplyColorChanges() {
            if (!this.TextComponent.enabled)
                return;

            // change colors of each character
            TMP_TextInfo textInfo = this.TextComponent.textInfo;
            for (int i = 0; i < textInfo.characterCount; i++) {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                // don't bother if character isn't visible
                if (!charInfo.isVisible)
                    continue;

                // get changes to make to the character
                if (i >= _colorChanges.Count)
                    break;
                ColorChanges colorChanges = _colorChanges[i];
                if (colorChanges == null)
                    continue;

                // gets the colors that will be changed
                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

                // get the original colors before vertex changes were made
                Color32[] sourceColors = _meshCache[materialIndex].colors32;

                // make changes
                Color32 topLeftColor = sourceColors[vertexIndex + 1];
                topLeftColor.a = (byte)Mathf.FloorToInt(topLeftColor.a * colorChanges.Alpha);
                colors[vertexIndex + 1] = topLeftColor;

                Color32 topRightColor = sourceColors[vertexIndex + 2];
                topRightColor.a = (byte)Mathf.FloorToInt(topRightColor.a * colorChanges.Alpha);
                colors[vertexIndex + 2] = topRightColor;

                Color32 bottomLeftColor = sourceColors[vertexIndex + 0];
                bottomLeftColor.a = (byte)Mathf.FloorToInt(bottomLeftColor.a * colorChanges.Alpha);
                colors[vertexIndex + 0] = bottomLeftColor;

                Color32 bottomRightColor = sourceColors[vertexIndex + 3];
                bottomRightColor.a = (byte)Mathf.FloorToInt(bottomRightColor.a * colorChanges.Alpha);
                colors[vertexIndex + 3] = bottomRightColor;
            }

            // update data
            this.TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        /// <summary>
        /// Method called when any TMPro text object changes.
        /// </summary>
        /// <param name="text">The text object changed.</param>
        private void TextChangedEventReceiver(object text) {
            if (!ReferenceEquals(text, this.TextComponent))
                return;

            this.OnTextChangedInner();
        } 

        private void OnTextChangedInner() {
            this.RefreshCache();

            if (this.TextComponent.text != _prevText) {
                _prevText = this.TextComponent.text;

                // get parsed text
                this.TextComponent.ForceMeshUpdate(true); // GetParsedText() will be a frame late unless this is called
                this.ParsedText = this.TextComponent.GetParsedText();
                this.ParsedTextLength = this.ParsedText == null ? 0 : this.ParsedText.Length;
                this.LineCount = this.TextComponent.textInfo.lineCount;

                this.OnTextChanged();

                this.TextChanged?.Invoke();
            }

            this.ApplyVertexChanges();
            this.ApplyColorChanges();
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Reference to the text component.
        /// </summary>
        private TMP_Text _textComponent = null;

        /// <summary>
        /// Previous value of <see cref="TextComponent.text"/>.
        /// </summary>
        private string _prevText = null;

        /// <summary>
        /// Mesh info copied from the text's current meshes, before changes were made.
        /// </summary>
        private TMP_MeshInfo[] _meshCache = null;

        /// <summary>
        /// Changes to apply to the vertices of the characters of this text.
        /// </summary>
        private List<VertexChanges> _vertexChanges = new List<VertexChanges>();

        /// <summary>
        /// Changes to apply to the colors of the characters of this text.
        /// </summary>
        private List<ColorChanges> _colorChanges = new List<ColorChanges>();

        #endregion
    }
}