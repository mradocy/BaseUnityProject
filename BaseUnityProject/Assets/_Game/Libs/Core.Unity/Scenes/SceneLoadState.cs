using UnityEngine.SceneManagement;

namespace Core.Unity.Scenes {

    /// <summary>
    /// The state of a <see cref="Scene"/>'s load progress.  Used by <see cref="AsyncSceneManager"/>.
    /// </summary>
    public enum SceneLoadState {
        NotLoaded,
        Loading,
        Loaded,
        Unloading,
    }

}