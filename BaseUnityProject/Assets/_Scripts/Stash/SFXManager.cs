using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour {

    #region Static - Instantiation
    
    /// <summary>
    /// Reference to the one SFXManager instance.
    /// </summary>
    public static SFXManager instance {
        get {
            return _instance;
        }
    }
    
    /// <summary>
    /// Instantiates an instance of SFXManager, if one doesn't already exist.
    /// </summary>
    public static void instantiate() {
        if (_instance != null) return;
        GameObject smGO = new GameObject();
        for (int i = 0; i < NUM_AUDIO_SOURCES; i++) {
            smGO.AddComponent<AudioSource>();
        }
        _instance = smGO.AddComponent<SFXManager>();
    }

    /// <summary>
    /// Number of AudioSource components to add to the instantiated gameObject containing SFXManager.  More audio sources, more sounds can be played at once.
    /// </summary>
    public const int NUM_AUDIO_SOURCES = 12;

    #endregion

    #region Static - Settings

    /// <summary>
    /// Master SFX volume level, as would be shown in an options menu.  All SFX volume levels are multiplied by this.
    /// </summary>
    public static float masterVolume {
        get {
            return InitializationSettings.sfxVolume;
        }
        set {
            InitializationSettings.sfxVolume = value;
        }
    }

    #endregion

    #region Playing SFX

    /// <summary>
    /// Another multiplier sounds are multiplied by.  This multiplier can be ignored by calling functions that say 'IgnoreVolumeScale'.
    /// </summary>
    public float volumeScale {
        get { return _volumeScale; }
        set { _volumeScale = value; }
    }

    /// <summary>
    /// Play a sound at the given volume.  The volume is multiplied by volumeScale.
    /// </summary>
    public void play(AudioClip clip, float volume = 1.0f) {
        playIgnoreVolumeScale(clip, volume * volumeScale);
    }

    /// <summary>
    /// Play a sound with a random pitch bend within the given range.  The volume is multiplied by volumeScale.
    /// </summary>
    /// <param name="pitchBendMagnitude">Magnitude range to multiply the pitch by.</param>
    public void playRandPitchBend(AudioClip clip, float pitchBendMagnitude = .05f, float volume = 1.0f) {
        playRandPitchBendIgnoreVolumeScale(clip, pitchBendMagnitude, volume * volumeScale);
    }

    /// <summary>
    /// Play a sound at the given volume.
    /// </summary>
    public void playIgnoreVolumeScale(AudioClip clip, float volume = 1.0f) {
        playF(clip, volume, 1);
    }

    /// <summary>
    /// Play a sound with a random pitch bend within the given range.
    /// </summary>
    /// <param name="pitchBendMagnitude">Magnitude range to multiply the pitch by.</param>
    public void playRandPitchBendIgnoreVolumeScale(AudioClip clip, float pitchBendMagnitude = .05f, float volume = 1.0f) {
        float pitch = 1 + (Random.value * 2 - 1) * pitchBendMagnitude;
        playF(clip, volume, pitch);
    }

    /// <summary>
    /// Stops a sound effect if it's currently playing.
    /// </summary>
    public void stop(AudioClip clip) {
        if (clip == null) return;
        foreach (AudioSource audS in sfxSources) {
            if (audS.clip == null) continue;
            if (audS.clip == clip) {
                audS.Stop();
                return;
            }
        }
    }

    /// <summary>
    /// Returns if the given sound effect is currently playing.
    /// </summary>
    public bool isPlaying(AudioClip clip) {
        if (clip == null) return false;
        foreach (AudioSource audS in sfxSources) {
            if (audS.clip == clip && audS.isPlaying) {
                return true;
            }
        }
        return false;
    }

    #endregion
    
    #region Private Helpers

    /// <summary>
    /// Private function that plays the given sfx clip with the given properties.  This is where masterVolume is applied.
    /// </summary>
    private void playF(AudioClip clip, float volume, float pitch) {
        if (clip == null) {
            Debug.LogError("ERROR: AudioClip is null");
            return;
        }
        volume *= masterVolume;
        AudioSource source = assignSFXSource(clip);
        source.volume = volume;
        source.pitch = pitch;
        source.loop = false;
        source.Play();
    }

    /// <summary>
    /// Finds an AudioSource from sfxSources to play the given clip on.  AudioSources that aren't currently playing anything are prioritized.
    /// </summary>
    /// <param name="clip">The clip to play.</param>
    private AudioSource assignSFXSource(AudioClip clip) {

        AudioSource source = sfxSources[0];
        foreach (AudioSource audS in sfxSources) {
            if (audS.clip == null || !audS.isPlaying) {
                source = audS;
                break;
            }
        }
        if (source.clip != null && !source.isPlaying) {
            source.Stop();
        }
        source.clip = clip;
        return source;
    }

    #endregion

    #region Unity Functions

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSources = GetComponents<AudioSource>();

    }

    void Update() {

    }

    void OnDestroy() {
        if (_instance == this) {
            _instance = null;
        }

        foreach (AudioSource audS in sfxSources) {
            audS.clip = null;
        }
        sfxSources = null;
    }

    #endregion

    #region Private Variables

    private static SFXManager _instance = null;

    private float _volumeScale = 1;
    
    AudioSource[] sfxSources;

    #endregion
    
}
