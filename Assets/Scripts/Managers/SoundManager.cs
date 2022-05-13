using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : PersistentSingleton<SoundManager>
{
    struct SoundPlayer
    {
        public float startTime;
        public AudioSource source;
        public SoundPlayer(float startTime, AudioSource source) {
            this.startTime = startTime;
            this.source = source;
        }
    }

    [SerializeField]
    [Range(0, 1)]
    float _musicVolume = 1f, _effectVolume = 1f;
    public float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            foreach (SoundPlayer player in playingPool)
            {
                if (player.source.loop) player.source.volume = player.source.volume * value / _musicVolume;
            }
            _musicVolume = value;
        }
    }
    public float EffectVolume
    {
        get { return _effectVolume; }
        set
        {
            foreach (SoundPlayer player in playingPool)
            {
                if (!player.source.loop) player.source.volume = player.source.volume * value / _effectVolume;
            }
            _effectVolume = value;
        }
    }

    const int MAX_POOL_SIZE = 10;
    List<SoundPlayer> playingPool = new List<SoundPlayer>();
    Queue<AudioSource> sourcePool = new Queue<AudioSource>(MAX_POOL_SIZE);
    int m_PlayerID = 0;
    AudioListener gListener;
    AudioSource bgmSource;

    AudioSource SourceBGM
    {
        get {
            if (bgmSource == null) {
                GameObject go = new GameObject("bgmSource");
                go.transform.SetParent(transform, false);
                bgmSource = go.AddComponent<AudioSource>();
            }
            return bgmSource;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        name = "SoundManager";
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void Start()
    {
        CheckListener();
        PlayBGM("Audios/Bgm");
    }

    void OnSceneChanged(Scene s1, Scene s2)
    {
        CheckListener();
    }

    void CheckListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length == 0)
        {
            gListener = gameObject.AddComponent<AudioListener>();
        }
        else
        {
            foreach (AudioListener listener in listeners)
            {
                if (listener != gListener)
                {
                    if (gListener != null && listener != gListener)
                    {
                        gListener.enabled = false;
                        break;
                    }
                }
            }
        }
    }

    void ReleaseSoundPlayer(SoundPlayer sound)
    {
        if (sourcePool.Count >= MAX_POOL_SIZE)
        {
            Destroy(sound.source.gameObject);
        }
        else
        {
            sound.source.clip = null;
            sourcePool.Enqueue(sound.source);
        }
        playingPool.Remove(sound);
    }

    AudioSource GetSource()
    {
        AudioSource source;
        if (sourcePool.Count != 0)
        {
            source = sourcePool.Dequeue();
        }
        else
        {
            GameObject go = new GameObject("audioSource");
            go.transform.SetParent(transform);
            source = go.AddComponent<AudioSource>();
            source.spatialBlend = 0;
        }
        playingPool.Add(new SoundPlayer(Time.time, source));
        return source;
    }

    void StartReleaseChecker() {
        if (playingPool.Count == 1) {
            StartCoroutine("CheckAudioSourceRelease");
        }
    }

    IEnumerator CheckAudioSourceRelease() {
        while (playingPool.Count != 0) {
            yield return null;
            float curTime = Time.time;
            foreach (var sound in playingPool)
            {
                if (sound.startTime + sound.source.clip.length > curTime) {
                    ReleaseSoundPlayer(sound);
                }
            }
        }
    }

    public void PlayBGM(string audioUrl, float volume = 1, bool fade = true)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(audioUrl);
        PlayBGM(audioClip, volume);
    }

    public void PlayBGM(AudioClip clip, float volume = 1)
    {
        if (SourceBGM == null || SourceBGM.clip != clip)
        {
            SourceBGM.clip = clip;
            SourceBGM.volume = MusicVolume * volume;
            SourceBGM.Play();
        }
    }

    public int Play(string audioUrl, bool isOneShot, float volume = 1, float maxDistance = 10)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(audioUrl);
        return Play(audioClip, isOneShot, volume, maxDistance);
    }

    public int Play(AudioClip audioClip, bool isOneShot, float volume = 1, float maxDistance = 10)
    {
        int id = m_PlayerID++;
        AudioSource source = GetSource();
        source.maxDistance = maxDistance;
        source.volume = isOneShot ? EffectVolume * volume : MusicVolume * volume;
        if (isOneShot)
        {
            source.name = id.ToString();
            source.loop = false;
            source.PlayOneShot(audioClip);
            StartReleaseChecker();
        }
        else
        {
            source.name = id.ToString();
            source.clip = audioClip;
            source.Play();
            source.loop = true;
        }
        return id;
    }

}
