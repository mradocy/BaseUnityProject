using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Core.Unity.Attributes;

namespace Core.Unity.Rendering {

    /// <summary>
    /// Makes an arbitrary mesh from a polygon.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class PolygonMesh : PolygonPoints {

        #region Inspector Fields

        [SerializeField, LongLabel]
        [Tooltip("If the mesh should update every frame while the application is playing.  If not, it must be manually updated with UpdateMesh().")]
        private bool _alwaysUpdateWhenPlaying = true;

        #endregion

        #region Methods

        /// <summary>
        /// Updates both meshes based on the points.
        /// Should be called after making changes to the points.
        /// </summary>
        public void UpdateMeshes() {

            this.InitMesh();

            Vector2[] points = LocalPoints;
            if (points == null || points.Length < 3) {
                // no points, clear mesh
                _vertices.Clear();
                _uvs.Clear();
                _triangles.Clear();
            } else {
                // resize vertices and uvs
                Resize(_vertices, points.Length);
                Resize(_uvs, _vertices.Count);

                // get bounds
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minY = float.MaxValue;
                float maxY = float.MinValue;
                for (int i = 0; i < points.Length; i++) {
                    Vector2 pt = points[i];
                    minX = Mathf.Min(minX, pt.x);
                    maxX = Mathf.Max(maxX, pt.x);
                    minY = Mathf.Min(minY, pt.y);
                    maxY = Mathf.Max(maxY, pt.y);
                }

                // get v3 vertices and uvs
                for (int i = 0; i < points.Length; i++) {
                    Vector2 pt = points[i];
                    _vertices[i] = pt;
                    _uvs[i] = new Vector2(
                        (pt.x - minX) / (maxX - minX),
                        (pt.y - minY) / (maxY - minY));
                }

                // get triangles
                _triangles.Clear();
                Triangulator triangulator = new Triangulator(points);
                int[] indices = triangulator.Triangulate();
                _triangles.AddRange(indices);
            }

            if (_mesh != null) {
                _mesh.Clear();
                _mesh.SetVertices(_vertices);
                _mesh.SetUVs(0, _uvs);
                _mesh.SetTriangles(_triangles, 0);
                _mesh.RecalculateBounds();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the mesh if it has not been initialized yet.
        /// </summary>
        private void InitMesh() {
            if (_mesh != null)
                return;
            if (_meshFilter == null) {
                _meshFilter = this.GetComponent<MeshFilter>();
            }
            if (_meshFilter == null)
                return;

            _mesh = new Mesh() {
                name = "Mesh",
            };
            _mesh.MarkDynamic();
            _meshFilter.mesh = _mesh;
        }

        /// <summary>
        /// Resizes the given list
        /// </summary>
        private static void Resize<T>(List<T> list, int size, T defaultValue = default(T)) {
            int count = list.Count;

            if (size < count) {
                list.RemoveRange(size, count - size);
            } else if (size > count) {
                if (size > list.Capacity) // Optimization
                    list.Capacity = size;

                list.AddRange(System.Linq.Enumerable.Repeat(defaultValue, size - count));
            }
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        protected void Awake() {
            if (!Application.isPlaying)
                return;

            _meshFilter = this.EnsureComponent<MeshFilter>();
            this.InitMesh();
            this.UpdateMeshes();
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected void Update() {

            // update meshes
            if (!Application.isPlaying || _alwaysUpdateWhenPlaying) {
                this.UpdateMeshes();
            }
        }

        #endregion

        #region Private Fields

        private MeshFilter _meshFilter = null;
        private Mesh _mesh = null;
        private List<Vector3> _vertices = new List<Vector3>();
        private List<Vector2> _uvs = new List<Vector2>();
        private List<int> _triangles = new List<int>();

        #endregion
    }
}