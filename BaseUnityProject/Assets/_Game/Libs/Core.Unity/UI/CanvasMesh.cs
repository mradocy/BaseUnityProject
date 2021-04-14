// Modified from https://gist.github.com/YclepticStudios/f2313ab08d2c81a31c94d5ed6b1e6eed
using UnityEngine;
using UnityEngine.UI;

namespace Core.Unity.UI {

    [ExecuteAlways]
    public class CanvasMesh : MaskableGraphic {

        #region Inspector Fields

        [SerializeField]
        private Mesh _mesh = null;

        [SerializeField]
        [Tooltip("If the mesh created should be auto-scaled to reach the bounds of the rect transform.  If not, mesh will be scaled to be like a quad.")]
        private bool _autoScaleMesh = false;

        #endregion

        /// <summary>
        /// Gets or sets the mesh that's rendered.
        /// </summary>
        public Mesh Mesh {
            get => _mesh;
            set => _mesh = value;
        }

        /// <summary>
        /// Callback function when a UI element needs to generate vertices.
        /// </summary>
        /// <param name="vh">VertexHelper utility.</param>
        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
            if (_mesh == null)
                return;

            // Get data from mesh
            Vector3[] verts = _mesh.vertices;
            Vector2[] uvs = _mesh.uv;
            if (uvs.Length < verts.Length) {
                uvs = new Vector2[verts.Length];
            }

            // Get mesh bounds parameters
            Vector2 meshMin, meshSize;
            if (_autoScaleMesh) {
                meshMin = _mesh.bounds.min;
                meshSize = _mesh.bounds.size;
            } else {
                // pretend bounds is a quad
                meshMin = new Vector2(-0.5f, -0.5f);
                meshSize = Vector2.one;
            }

            // Add scaled vertices
            for (int ii = 0; ii < verts.Length; ii++) {
                Vector2 v = verts[ii];
                v.x = (v.x - meshMin.x) / meshSize.x;
                v.y = (v.y - meshMin.y) / meshSize.y;
                v = Vector2.Scale(v - this.rectTransform.pivot, this.rectTransform.rect.size);
                vh.AddVert(v, this.color, uvs[ii]);
            }

            // Add triangles
            int[] tris = _mesh.triangles;
            for (int ii = 0; ii < tris.Length; ii += 3) {
                vh.AddTriangle(tris[ii], tris[ii + 1], tris[ii + 2]);
            }
        }
    }
}