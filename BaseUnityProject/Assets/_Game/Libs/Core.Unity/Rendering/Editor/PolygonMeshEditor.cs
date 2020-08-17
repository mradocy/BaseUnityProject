using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using UnityEditor;

namespace Core.Unity.Rendering {

    /// <summary>
    /// Editor for the <see cref="PolygonMesh"/> component.
    /// </summary>
    [CustomEditor(typeof(PolygonMesh))]
    public class PolygonMeshEditor : PolygonPointsEditor { }
}