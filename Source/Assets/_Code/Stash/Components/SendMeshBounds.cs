using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
/// <summary>
/// Sets properties of shaders of materials of the attached mesh every frame.  Shaders do not have to use every property.
/// Properties:
///     _MeshWidth - width of the mesh
///     _MeshHeight - height of the mesh
///     _MeshPxWidth - width of the mesh in pixels
///     _MeshPxHeight - height of the mesh in pixels
/// </summary>
public class SendMeshBounds : MonoBehaviour {

    public float pixelsPerUnit = 100;

    public void setProperties() {

        if (meshRenderer == null) {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                return;
        }
        float meshWidth = 0;
        float meshHeight = 0;
        if (meshFilter == null) {
            meshFilter = GetComponent<MeshFilter>();
        }
        if (meshFilter != null) {
            Mesh mesh;
#if UNITY_EDITOR
            mesh = meshFilter.sharedMesh;
#else
            mesh = meshFilter.mesh;
#endif
            if (mesh != null) {
                meshWidth = mesh.bounds.size.x;
                meshHeight = mesh.bounds.size.y;
            }
        }

        Material[] materials;
#if UNITY_EDITOR
        materials = meshRenderer.sharedMaterials;
#else
        materials = meshRenderer.materials;
#endif

        foreach (Material material in materials) {
            if (material.HasProperty("_MeshWidth")) {
                material.SetFloat("_MeshWidth", meshWidth);
            }
            if (material.HasProperty("_MeshHeight")) {
                material.SetFloat("_MeshHeight", meshHeight);
            }
            if (material.HasProperty("_MeshPxWidth")) {
                material.SetFloat("_MeshPxWidth", meshWidth * pixelsPerUnit);
            }
            if (material.HasProperty("_MeshPxHeight")) {
                material.SetFloat("_MeshPxHeight", meshHeight * pixelsPerUnit);
            }
        }

    }

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
	}
    
    void Update() {
        
        if (enabled) {

#if UNITY_EDITOR
            if (!Application.isPlaying)
                setProperties();
#else
            setProperties();
#endif      

        }
    }

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

}
