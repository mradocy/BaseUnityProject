using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.Rendering {

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(PolygonCollider2D))]
    [ExecuteInEditMode]
    public class MeshMatchesPolygonCollider2D : MonoBehaviour {

        [SerializeField]
        [Tooltip("If the mesh should update every frame while the application is playing.  If not, it must be manually updated with UpdateMesh().")]
        private bool _alwaysUpdateWhenPlaying = true;

        public MeshFilter MeshFilter { get; private set; }

        public PolygonCollider2D Pc2d { get; private set; }

        public void UpdateMesh() {
            this.InitMesh();

            if (this.Pc2d.points == null || this.Pc2d.points.Length < 3) {
                // no points, clear mesh
                _vertices.Clear();
                _uvs.Clear();
                _triangles.Clear();
            } else {
                // get points from pc2d
                _points.Clear();
                for (int i=0; i < this.Pc2d.points.Length; i++) {
                    _points.Add(this.Pc2d.points[i] + this.Pc2d.offset);
                }

                // resize vertices and uvs
                Resize(_vertices, _points.Count);
                Resize(_uvs, _points.Count);

                // get bounds
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minY = float.MaxValue;
                float maxY = float.MinValue;
                for (int i=0; i < _points.Count; i++) {
                    Vector2 pt = _points[i];
                    minX = Mathf.Min(minX, pt.x);
                    maxX = Mathf.Max(maxX, pt.x);
                    minY = Mathf.Min(minY, pt.y);
                    maxY = Mathf.Max(maxY, pt.y);
                }

                // get v3 vertices and uvs
                for (int i = 0; i < _points.Count; i++) {
                    Vector2 pt = _points[i];
                    _vertices[i] = pt;
                    _uvs[i] = new Vector2(
                        (pt.x - minX) / (maxX - minX),
                        (pt.y - minY) / (maxY - minY));
                }

                // get triangles
                _triangles.Clear();
                Triangulator triangulator = new Triangulator(_points);
                int[] indices = triangulator.Triangulate();
                _triangles.AddRange(indices);
            }

            _mesh.Clear();
            _mesh.SetVertices(_vertices);
            _mesh.SetUVs(0, _uvs);
            _mesh.SetTriangles(_triangles, 0);
            _mesh.RecalculateBounds();
        }

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            this.MeshFilter = this.EnsureComponent<MeshFilter>();
            this.Pc2d = this.EnsureComponent<PolygonCollider2D>();

            this.InitMesh();
            this.UpdateMesh();
        }

        /// <summary>
        /// Called by Unity on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start() {
            if (Application.isPlaying) {
                this.UpdateMesh();
            }
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() {
            if (!Application.isPlaying || _alwaysUpdateWhenPlaying) {
                this.UpdateMesh();
            }
        }

        /// <summary>
        /// Initializes the mesh, if it has not been initialized yet.
        /// </summary>
        private void InitMesh() {
            if (_mesh != null)
                return;

            MeshFilter meshFilter = this.EnsureComponent<MeshFilter>();

            _mesh = new Mesh() {
                name = "Mesh",
            };
            _mesh.MarkDynamic();
            meshFilter.mesh = _mesh;
        }

        /// <summary>
        /// Resizes a list to contain the given number of elements.
        /// </summary>
        /// <typeparam name="T">Type of element in the list.</typeparam>
        /// <param name="list">This list to resize.</param>
        /// <param name="size">The new size (count) of the list.</param>
        /// <param name="defaultValue">The element to add when making the list bigger.</param>
        private static void Resize<T>(List<T> list, int size, T defaultValue = default) {
            int count = list.Count;

            if (size < count) {
                list.RemoveRange(size, count - size);
            } else if (size > count) {
                if (size > list.Capacity) // Optimization
                    list.Capacity = size;

                while (list.Count < size) {
                    list.Add(defaultValue);
                }
            }
        }

        private List<Vector2> _points = new List<Vector2>();

        private Mesh _mesh = null;
        private List<Vector3> _vertices = new List<Vector3>();
        private List<Vector2> _uvs = new List<Vector2>();
        private List<int> _triangles = new List<int>();
    }
}