using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour {
    
    public static CameraControl instance { get; private set; }
    public Camera cameraRef { get; private set; }

    /// <summary>
    /// Creates an instance of CameraControl.  Does nothing if instance is already created.
    /// </summary>
    public static void instantiate() {
        if (instance != null) return;

        GameObject ccGO = new GameObject();
        ccGO.AddComponent<CameraControl>();
        ccGO.AddComponent<CheckpointUser>();
        instance = ccGO.GetComponent<CameraControl>();
    }
    

    #region Position Properties

    /// <summary>
    /// Gets camera position, ignoring shake offsets.
    /// </summary>
    public Vector2 cameraPos {
        get {
            return cameraRef.transform.position - new Vector3(shakeOffset.x, shakeOffset.y, 0);
        }
        set {
            cameraRef.transform.position = new Vector3(value.x, value.y, cameraRef.transform.position.z) + new Vector3(shakeOffset.x, shakeOffset.y, 0);
        }
    }
    /// <summary>
    /// Gets orthographic size of the camera.
    /// </summary>
    public float cameraSize {
        get { return cameraRef.orthographicSize; }
        set {
            cameraRef.orthographicSize = Mathf.Max(.5f, value);
        }
    }
    /// <summary>
    /// Right bound of the camera screen, ignoring shake offsets.
    /// </summary>
    public float rightBound {
        get {
            return cameraPos.x + cameraRef.orthographicSize * Screen.width / Screen.height;
        }
    }
    /// <summary>
    /// Top bound of the camera screen, ignoring shake offsets.
    /// </summary>
    public float topBound {
        get {
            return cameraPos.y + cameraRef.orthographicSize;
        }
    }
    /// <summary>
    /// Left bound of the camera screen, ignoring shake offsets.
    /// </summary>
    public float leftBound {
        get {
            return cameraPos.x - cameraRef.orthographicSize * Screen.width / Screen.height;
        }
    }
    /// <summary>
    /// Bottom bound of the camera screen, ignoring shake offsets.
    /// </summary>
    public float bottomBound {
        get {
            return cameraPos.y - cameraRef.orthographicSize;
        }
    }
    /// <summary>
    /// Gets if the given point can be seen on screen, shake offsets.
    /// </summary>
    /// <param name="position">Point to test.</param>
    /// <param name="border">Optional value that expands the border for the test.</param>
    public bool pointInBounds(Vector2 position, float border = 0) {
        return leftBound - border <= position.x && position.x <= rightBound + border &&
            bottomBound - border <= position.y && position.y <= topBound + border;
    }

    /// <summary>
    /// Converts a point in world space to its position on the UI layer, using the provided RectTransform.
    /// </summary>
    /// <param name="canvasRectTransform">RectTransform of the Canvas the returned UI position is for.  Is likely the RectTransform of the parent container of the UI GameObject.</param>
    /// <param name="worldPoint">Point in the world space.</param>
    public Vector2 worldToUIPoint(RectTransform canvasRectTransform, Vector2 worldPoint) {
        Vector2 uv = cameraRef.WorldToViewportPoint(worldPoint);
        Rect canvasRect = canvasRectTransform.rect;
        return new Vector2(canvasRect.xMin + canvasRect.width * uv.x, canvasRect.yMin + canvasRect.height * uv.y);
    }
    
    #endregion

    #region Hit Pause

    /// <summary>
    /// Gets if the game is currently hit pausing
    /// </summary>
    public bool hitPaused {
        get {
            return PauseManager.state == PauseManager.State.HIT_PAUSE;
        }
    }

    /// <summary>
    /// Pauses the game for a brief moment.  Intended to be used to make hits feel more impactful.
    /// </summary>
    /// <param name="duration">Duration of the hit pause.</param>
    public void hitPause(float duration) {
        if (hitPaused) {
            // only apply if duration is longer than current hit pause
            if (hitPauseDuration - hitPauseTime > duration)
                return;
        }
        if (!PauseManager.paused) {
            // start hit pause
            PauseManager.beginState(PauseManager.State.HIT_PAUSE);
            hitPauseTime = 0;
            hitPauseDuration = duration;
        }
    }

    #endregion

    #region Shake Effects
    
    public const float SHAKE_WAVE_2_MAGNITUDE_MULTIPLIER = .2f;
    public const float SHAKE_WAVE_2_FREQUENCY_MULTIPLIER = 4.5f;
    
    /// <summary>
    /// Shakes the screen for a given duration.
    /// </summary>
    /// <param name="magnitude">Magnitude of the movement.  x is horizontal waves, y is vertical waves.  (.02f, .1f) are good values to start with.</param>
    /// <param name="frequency">Frequency of the movement.  x is for horizontal waves, y for vertical waves.  (13f, 10f) are good values to start with.</param>
    /// <param name="duration">How long the shake lasts.</param>
    public void shake(Vector2 magnitude, Vector2 frequency, float duration) {
        shakeNoLayer(magnitude, frequency, duration);
        // layered wave
        shakeNoLayer(magnitude * SHAKE_WAVE_2_MAGNITUDE_MULTIPLIER, frequency * SHAKE_WAVE_2_FREQUENCY_MULTIPLIER, duration);
    }

    /// <summary>
    /// Shakes the screen endlessly.  Stop by calling shakeStop().
    /// </summary>
    /// <param name="magnitude">Magnitude of the movement.  x is horizontal waves, y is vertical waves.  (.02f, .1f) are good values to start with.</param>
    /// <param name="frequency">Frequency of the movement.  x is for horizontal waves, y for vertical waves.  (13f, 10f) are good values to start with.</param>
    /// <param name="duration">How long the shake lasts.</param>
    public void shakeEndless(Vector2 magnitude, Vector2 frequency) {
        shake(magnitude, frequency, float.MaxValue);
    }

    /// <summary>
    /// Shakes the screen by adding only one shake wave.
    /// </summary>
    /// <param name="magnitude">Magnitude of the movement.  x is horizontal waves, y is vertical waves.  (.02f, .1f) are good values to start with.</param>
    /// <param name="frequency">Frequency of the movement.  x is for horizontal waves, y for vertical waves.  (13f, 10f) are good values to start with.</param>
    /// <param name="duration">How long the shake lasts.</param>
    public void shakeNoLayer(Vector2 magnitude, Vector2 frequency, float duration) {
        addShakeWave(magnitude, frequency, Mathf.Max(frequency.x, frequency.y) * Random.value, duration);
    }

    /// <summary>
    /// Stops all shaking
    /// </summary>
    public void shakeStop() {
        shakeWaves.Clear();
    }

    #endregion

    #region View Modes

    /// <summary>
    /// View modes automatically set the camera every frame.  Set viewModesEnabled to false to prevent this, allowing for manual control.
    /// </summary>
    public bool viewModesEnabled { get; set; }
    
    /// <summary>
    /// Current view mode.
    /// </summary>
    public ViewMode currentViewMode { get; private set; }
    /// <summary>
    /// Previous view mode.
    /// </summary>
    public ViewMode previousViewMode { get; private set; }
    /// <summary>
    /// The transition style that blends the previous view mode to the current view mode.
    /// </summary>
    public ViewModeTransition viewModeTransition { get; private set; }
    
    /// <summary>
    /// Set the view mode without a transition.
    /// </summary>
    /// <param name="viewMode">The view mode that will become the current view mode.</param>
    public void setViewMode(ViewMode viewMode) {
        setViewMode(viewMode, ViewModeTransition.NONE, 0);
    }

    /// <summary>
    /// Set the view mode.  The current view mode will become the previous view mode, and the given blending method will be used to transition the two modes over time.
    /// </summary>
    /// <param name="viewMode">The view mode that will become the current view mode.</param>
    /// <param name="transition">Style of transitioning from the previous to the current view mode.</param>
    /// <param name="transitionDuration">How long the transition will take.</param>
    public void setViewMode(ViewMode viewMode, ViewModeTransition transition, float transitionDuration) {
        previousViewMode = currentViewMode;
        preTransitionPosition = cameraPos;
        preTransitionSize = cameraSize;
        currentViewMode = viewMode;
        viewModeTransition = transition;
        viewModeBlendTime = 0;
        viewModeBlendDuration = transitionDuration;
        if (viewModeBlendDuration < .0001f) {
            viewModeTransition = ViewModeTransition.NONE;
        }
    }

    /// <summary>
    /// Enum defining methods of transitioning between the previous to the current view mode.
    /// </summary>
    public enum ViewModeTransition {
        NONE,
        EASE_QUAD_IN_OUT,
        X_DIRECT,
        Y_DIRECT,
        EASE_QUAD_OUT,
    }

    /// <summary>
    /// Struct describing where the camera should be pointed.
    /// If getPositionFunction is null, it will automatically be set to CameraControl.viewModeGetPositionDefault.
    /// </summary>
    public struct ViewMode {
        public ViewMode(Vector2 position, Rect bounds, float size = DEFAULT_CAMERA_SIZE, Transform followTransform = null, ViewModeGetPosition getPositionFunction = null) {
            this.position = position;
            this.bounds = bounds;
            this.size = size;
            this.followTransform = followTransform;
            positionFunction = getPositionFunction;
            if (positionFunction == null) {
                positionFunction = ViewModePositionFunctions.defaultPosition;
            }
        }
        /// <summary>
        /// Constructor that initializes bounds to essentially be infinitely big.
        /// If getPositionFunction is null, it will automatically be set to CameraControl.viewModeGetPositionDefault.
        /// </summary>
        public ViewMode(Vector2 position, float size = DEFAULT_CAMERA_SIZE, Transform followTransform = null, ViewModeGetPosition getPositionFunction = null) {
            this.position = position;
            bounds = new Rect(-400000, -400000, 800000, 800000);
            bounds.center += this.position;
            this.size = size;
            this.followTransform = followTransform;
            positionFunction = getPositionFunction;
            if (positionFunction == null) {
                positionFunction = ViewModePositionFunctions.defaultPosition;
            }
        }
        /// <summary>
        /// Default center position of the camera if followTransform is null. 
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// Reference to a Transform object to follow.  
        /// </summary>
        public Transform followTransform;
        /// <summary>
        /// Orthographic size of camera.
        /// </summary>
        public float size;
        /// <summary>
        /// Bounds contraining the view of the camera.
        /// </summary>
        public Rect bounds;
        /// <summary>
        /// Option of custom function for getting what the position of the camera should be.  This can be picked from static functions in CameraControl.ViewModePositionFunctions, or custom made following the ViewModeGetPosition delegate.
        /// </summary>
        public ViewModeGetPosition positionFunction;
        
        public bool equals(ViewMode viewMode) {
            return
                position == viewMode.position &&
                followTransform == viewMode.followTransform &&
                size == viewMode.size &&
                bounds == viewMode.bounds &&
                positionFunction == viewMode.positionFunction;
        }

        public override string ToString() {
            return "position: " + position.ToString() + " size: " + size + " bounds: " + bounds.ToString() +
                " followTransform: " + (followTransform == null ? "null" : followTransform.ToString()) +
                " positionFunction(): " + (positionFunction == null ? "null" : positionFunction.ToString());
        }

    }

    /// <summary>
    /// Camera size when creating a new default ViewMode.
    /// </summary>
    public const float DEFAULT_CAMERA_SIZE = 5;
    
    /// <summary>
    /// Delegate for optional custom functions for moving the camera.
    /// </summary>
    public delegate Vector2 ViewModeGetPosition(ViewMode viewMode);

    public class ViewModePositionFunctions {

        /// <summary>
        /// ID's for built in position functions.
        /// </summary>
        public enum ID {
            DEFAULT,
            LERP_PAN,
        }

        /// <summary>
        /// Gets a function from its ID.
        /// </summary>
        public static ViewModeGetPosition functionFromID(ID id) {
            switch (id) {
            case ID.LERP_PAN:
                return lerpPan;
            default:
                return defaultPosition;
            }
        }

        /// <summary>
        /// The default function for getting the intended camera position from a ViewMode.
        /// This function centers on viewMode's followTransform if available, and keeps the position contained in the viewMode's bounds.
        /// </summary>
        public static Vector2 defaultPosition(ViewMode viewMode) {
            Vector2 pos = viewMode.position;
            // match followTransform's position if provided.
            if (viewMode.followTransform != null) {
                pos = viewMode.followTransform.position;
            }
            // contain in bounds
            float left = viewMode.bounds.xMin + viewMode.size * Screen.width / Screen.height;
            float right = viewMode.bounds.xMax - viewMode.size * Screen.width / Screen.height;
            float bottom = viewMode.bounds.yMin + viewMode.size;
            float top = viewMode.bounds.yMax - viewMode.size;
            if (left > right) { // edge case, will happen if viewMode.bounds are too tight.
                pos.x = (left + right) / 2;
            } else {
                pos.x = Mathf.Clamp(pos.x, left, right);
            }
            if (bottom > top) { // edge case, will happen if viewMode.bounds are too tight.
                pos.y = (bottom + top) / 2;
            } else {
                pos.y = Mathf.Clamp(pos.y, bottom, top);
            }
            return pos;
        }

        /// <summary>
        /// Linearly interpolates the camera's position based on followTransform's position (if available) relative to the viewMode's bounds.
        /// </summary>
        public static Vector2 lerpPan(ViewMode viewMode) {
            Vector2 pos = viewMode.position;
            // match followTransform's position if provided.
            if (viewMode.followTransform != null) {
                pos = viewMode.followTransform.position;
            }

            float camLeft = viewMode.bounds.xMin + viewMode.size * Screen.width / Screen.height;
            float camRight = viewMode.bounds.xMax - viewMode.size * Screen.width / Screen.height;
            float camBottom = viewMode.bounds.yMin + viewMode.size;
            float camTop = viewMode.bounds.yMax - viewMode.size;
            if (camLeft > camRight) { // edge case, will happen if viewMode.bounds are too tight.
                pos.x = (camLeft + camRight) / 2;
            } else {
                pos.x = camLeft + (camRight - camLeft) * Mathf.Clamp((pos.x - viewMode.bounds.xMin) / viewMode.bounds.width, 0, 1);
            }
            if (camBottom > camTop) { // edge case, will happen if viewMode.bounds are too tight.
                pos.y = (camBottom + camTop) / 2;
            } else {
                pos.y = camBottom + (camTop - camBottom) * Mathf.Clamp((pos.y - viewMode.bounds.yMin) / viewMode.bounds.height, 0, 1);
            }
            return pos;
        }

    }

    

    #endregion

    #region Misc. Functions
    
    public override string ToString() {
        return "Position: " + cameraPos + " size: " + cameraSize;
    }

    #endregion

    #region Unity Functions

    void Awake() {
        if (instance != this) {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        cameraRef = Camera.main;
        if (cameraRef == null) {
            Debug.LogError("No Camera in scene");
            return;
        }
    }

    void Update() {

        hitPauseUpdate();

        if (viewModesEnabled) {
            viewModeUpdate();
        }

        debugUpdate();

    }

    void LateUpdate() {

        shakeUpdate();

    }
    
    void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
        shakeWaves.Clear();
    }

    #endregion
    
    #region Hit Pause Private

    void hitPauseUpdate() {

        if (hitPaused) {
            hitPauseTime += Time.unscaledDeltaTime;
            if (hitPauseTime >= hitPauseDuration) {
                // end hit pause
                PauseManager.resume();
            }
        }

    }

    float hitPauseTime = 0;
    float hitPauseDuration = 0;

    #endregion

    #region Shake Private
    
    struct ShakeWave {
        public Vector2 magnitude;
        public Vector2 frequency;
        public float timeOffset;
        public float startTime;
        public float duration;
    }

    void addShakeWave(Vector2 magnitude, Vector2 frequency, float timeOffset, float duration) {
        ShakeWave shakeProp = new ShakeWave();
        shakeProp.magnitude = magnitude;
        shakeProp.frequency = frequency;
        shakeProp.timeOffset = timeOffset;
        shakeProp.duration = duration;
        shakeProp.startTime = shakeTime;
        shakeWaves.Add(shakeProp);
    }

    void shakeUpdate() {

        // undo previous shake offset
        cameraRef.transform.position -= new Vector3(shakeOffset.x, shakeOffset.y, 0);
        shakeOffset.Set(0, 0);

        shakeTime += Time.deltaTime;
        // accumulate effects of shake waves
        for (int i = 0; i < shakeWaves.Count; i++) {
            float time = shakeTime - shakeWaves[i].startTime;
            float timeOffset = shakeWaves[i].timeOffset;
            float duration = shakeWaves[i].duration;
            Vector2 magnitude = Easing.quadOut(shakeWaves[i].magnitude, Vector2.zero, time, duration);
            shakeOffset.x += Mathf.Sin((time + timeOffset) * shakeWaves[i].frequency.x * Mathf.PI * 2) * magnitude.x;
            shakeOffset.y += Mathf.Sin((time + timeOffset) * shakeWaves[i].frequency.y * Mathf.PI * 2) * magnitude.y;
            
            if (time >= duration) {
                shakeWaves.RemoveAt(i);
                i--;
            }
        }

        // apply shake effect
        cameraRef.transform.position += new Vector3(shakeOffset.x, shakeOffset.y, 0);
    }

    List<ShakeWave> shakeWaves = new List<ShakeWave>();
    Vector2 shakeOffset = new Vector2();
    float shakeTime = 0;

    #endregion

    #region View Modes Private

    void viewModeUpdate() {

        viewModeBlendTime += Time.deltaTime;

        // get previous position and size
        //Vector2 previousPosition = previousViewMode.position;
        //if (previousViewMode.positionFunction != null) {
        //    previousPosition = previousViewMode.positionFunction(previousViewMode);
        //}
        //float previousSize = previousViewMode.size;
        //if (previousSize == 0)
        //    previousSize = DEFAULT_CAMERA_SIZE;
        Vector2 previousPosition = preTransitionPosition;
        float previousSize = preTransitionSize;

        // get current position and size
        Vector2 currentPosition = currentViewMode.position;
        if (currentViewMode.positionFunction == null) {
            Debug.LogError("Current view mode does not have its positionFunction function defined.");
        } else {
            currentPosition = currentViewMode.positionFunction(currentViewMode);
        }
        float currentSize = currentViewMode.size;
        if (currentSize == 0)
            currentSize = DEFAULT_CAMERA_SIZE;

        // transition from previous to current
        Vector2 position = currentPosition;
        float size = currentSize;
        if (viewModeBlendDuration < .0001f) // div by 0 failsafe
            viewModeTransition = ViewModeTransition.NONE;
        switch (viewModeTransition) {
        case ViewModeTransition.NONE:
            position = currentPosition;
            size = currentSize;
            break;
        case ViewModeTransition.EASE_QUAD_IN_OUT:
            position = Easing.quadInOut(previousPosition, currentPosition, viewModeBlendTime, viewModeBlendDuration);
            size = Easing.quadInOut(previousSize, currentSize, viewModeBlendTime, viewModeBlendDuration);
            break;
        case ViewModeTransition.EASE_QUAD_OUT:
            position = Easing.quadOut(previousPosition, currentPosition, viewModeBlendTime, viewModeBlendDuration);
            size = Easing.quadOut(previousSize, currentSize, viewModeBlendTime, viewModeBlendDuration);
            break;
        case ViewModeTransition.X_DIRECT:
            position.x = currentPosition.x;
            position.y = Easing.quadInOut(previousPosition.y, currentPosition.y, viewModeBlendTime, viewModeBlendDuration);
            size = Easing.quadInOut(previousSize, currentSize, viewModeBlendTime, viewModeBlendDuration);
            break;
        case ViewModeTransition.Y_DIRECT:
            position.x = Easing.quadInOut(previousPosition.x, currentPosition.x, viewModeBlendTime, viewModeBlendDuration);
            position.y = currentPosition.y;
            size = Easing.quadInOut(previousSize, currentSize, viewModeBlendTime, viewModeBlendDuration);
            break;
        }

        // set to camera
        cameraPos = position;
        cameraSize = size;

    }

    float viewModeBlendTime = 0;
    float viewModeBlendDuration = 0;
    Vector2 preTransitionPosition = new Vector2();
    float preTransitionSize = DEFAULT_CAMERA_SIZE;

    #endregion
    
    #region Checkpoint save/load

    ViewMode cpViewMode;
    Vector2 cpCameraPos = Vector2.zero;
    float cpCameraSize = DEFAULT_CAMERA_SIZE;
    void OnCheckpointSave() {
        cpViewMode = currentViewMode;
        cpCameraPos = cameraPos;
        cpCameraSize = cameraSize;
    }
    
    void OnCheckpointLoad() {
        setViewMode(cpViewMode);
        cameraPos = cpCameraPos;
        cameraSize = cpCameraSize;
        shakeStop();
    }

    #endregion

    #region Debug Movement Private

    float debugCameraDragSpeed = .01f;

    /// <summary>
    /// Can move camera around.
    /// </summary>
    void debugUpdate() {
        if (!UDeb.debug) return;
        
        bool canMoveCamera = true;
        if (canMoveCamera) {

            // dragging camera
            if (Input.GetMouseButtonDown(1)) {
                prevMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(1)) {
                Vector2 mousePosition = Input.mousePosition;

                // compare to previous mouse position
                Vector2 diff = mousePosition - prevMousePosition;

                viewModesEnabled = false; // disable view modes to let debug take control of camera
                cameraPos -= diff * debugCameraDragSpeed * cameraSize;

                // update previous mouse position
                prevMousePosition = mousePosition;
            }

            // zooming in/out
            float mouseScroll = Input.mouseScrollDelta.y;
            if (UDeb.keyPressed(KeyCode.PageUp)) {
                mouseScroll = 1;
            } else if (UDeb.keyPressed(KeyCode.PageDown)) {
                mouseScroll = -1;
            }
            if (mouseScroll != 0) {
                viewModesEnabled = false; // disable view modes to let debug take control of camera
                cameraSize -= mouseScroll * .1f * cameraSize;
            }

            UDeb.post(0, "position: " + cameraPos.ToString() + " size: " + cameraSize);

        }

    }
    Vector2 prevMousePosition = new Vector3();

    #endregion

    
}
