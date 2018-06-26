using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Must have a UI Image with a size that covers the entire screen.  Source image doesn't need to be set to anything.
/// </summary>
[RequireComponent(typeof(Image))]
public class BlackScreen : MonoBehaviour {

    public static BlackScreen instance { get; private set; }
    
    public Image image { get; private set; }

    public enum State {
        CLEAR,
        FADING_OUT,
        BLACK,
        FADING_IN,
    }
    public State state { get; private set; }

    public void clear() {
        state = State.CLEAR;
        image.color = new Color(0, 0, 0, 0);
    }
    public void black() {
        state = State.BLACK;
        image.color = new Color(0, 0, 0, 1);
    }
    public void fadeOut(float duration) {
        if (duration <= .0001f) {
            black();
            return;
        }
        state = State.FADING_OUT;
        image.color = new Color(0, 0, 0, 0);
        time = 0;
        this.duration = duration;
    }
    public void fadeIn(float duration) {
        if (duration <= .0001f) {
            clear();
            return;
        }
        state = State.FADING_IN;
        image.color = new Color(0, 0, 0, 1);
        time = 0;
        this.duration = duration;
    }

    void Awake() {

        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // add to canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null) {
            transform.SetParent(canvas.transform, false);
        }

        image = GetComponent<Image>();

        clear();
	}
    
    void Update() {

        time += Time.deltaTime;

        switch (state) {
        case State.FADING_IN:
            image.color = Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), time / duration);
            if (time >= duration) {
                state = State.BLACK;
            }
            break;
        case State.FADING_OUT:
            image.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), time / duration);
            if (time >= duration) {
                state = State.CLEAR;
            }
            break;
        }

    }

    void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }

    float time = 0;
    float duration = 1;

}
