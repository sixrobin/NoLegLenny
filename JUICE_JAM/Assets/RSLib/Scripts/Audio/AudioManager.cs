namespace RSLib.Audio
{
    using RSLib.Maths;
    using UnityEngine;
    using UnityEngine.Audio;

    public class AudioManager : Framework.Singleton<AudioManager>
    {
        [System.Serializable]
        private class SFXPlayer
        {
            [SerializeField, Min(2)] private int _audioSourcesCount = 2;
            [SerializeField] private AudioMixerGroup _mixerGroup = null;

            public int AudioSourcesCount => _audioSourcesCount;
            public AudioMixerGroup MixerGroup => _mixerGroup;
        }

        private class RuntimeSFXPlayer
        {
            private int _nextSourceIndex = 0;
            private AudioSource[] _sources;

            public RuntimeSFXPlayer(int sourcesCount, AudioMixerGroup mixerGroup, Transform sourcesContainer)
            {
                _sources = new AudioSource[sourcesCount];
                for (int i = 0; i < sourcesCount; ++i)
                    _sources[i] = CreateAudioSource($"SFX Source {i}", sourcesContainer, mixerGroup);
            }

            public AudioSource GetNextSource()
            {
                return _sources[_nextSourceIndex++ % _sources.Length];
            }
        }

        [SerializeField] private SFXPlayer[] _sfxPlayers = null;
        [SerializeField] private AudioMixer _mixer = null;
        [SerializeField] private AudioMixerGroup _musicMixerGroup = null;

        [Header("UI")]
        [SerializeField] public ClipProvider UIHoverClip = null;
        [SerializeField] public ClipProvider UIClickClip = null;

        private static System.Collections.Generic.Dictionary<AudioMixerGroup, RuntimeSFXPlayer> s_sfxPlayersDict;
        private static AudioSource[] s_musicSources;
        private static int _nextMusicIndex;

        private static System.Collections.IEnumerator _musicFadeCoroutine;

        public float BaseMusicVolume;

        /// <summary>
        /// Remaps a value from [0.0001f, 1f] to [-80f, 0f], to adjust a percentage to the decibels scaling.
        /// </summary>
        /// <param name="value">Value to convert from percentage to decibels scale.</param>
        /// <returns>Decibels scaled value.</returns>
        public static float RemapToDecibels(float value)
        {
            return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        }

        public static void PlayNextPlaylistSoundOnAudioSource(IClipProvider clipProvider, AudioSource audioSource)
        {
            AudioClipPlayDatas clipDatas = clipProvider.GetNextClipDatas();

            audioSource.clip = clipDatas.Clip;
            audioSource.volume = clipDatas.RandomVolume;
            audioSource.pitch = 1f + clipDatas.PitchVariation;
            audioSource.Play();
        }

        public static void PlayNextPlaylistSound(IClipProvider clipProvider)
        {
            AudioSource source = GetSFXSource(clipProvider);
            PlayNextPlaylistSoundOnAudioSource(clipProvider, source);
        }

        public static void PlayNextPlaylistSound(ClipProvider clipProvider)
        {
            PlayNextPlaylistSound(clipProvider as IClipProvider);
        }

        public static void PlayMusic(IClipProvider musicProvider, MusicTransitionsDatas transitionDatas)
        {
            if (_musicFadeCoroutine != null)
                Instance.StopCoroutine(_musicFadeCoroutine);

            if (transitionDatas.CrossFade)
                Instance.StartCoroutine(_musicFadeCoroutine = Instance.CrossFadeMusicCoroutine(musicProvider, transitionDatas));
            else
                Instance.StartCoroutine(_musicFadeCoroutine = Instance.FadeMusicCoroutine(musicProvider, transitionDatas));
        }

        public static void StopMusic()
        {
            AudioSource source = GetCurrentMusicSource();
            source.volume = 0f;
            source.Stop();
        }

        private System.Collections.IEnumerator CrossFadeMusicCoroutine(IClipProvider musicProvider, MusicTransitionsDatas transitionDatas)
        {
            AudioSource prev = GetCurrentMusicSource();
            AudioSource next = GetNextMusicSource();
            AudioClipPlayDatas clipDatas = musicProvider.GetNextClipDatas();

            float prevVol = prev.volume;
            float nextVol = clipDatas.RandomVolume;

            next.clip = clipDatas.Clip;
            next.pitch = 1f + clipDatas.PitchVariation;
            next.Play();

            for (float t = 0f; t <= 1f; t += Time.deltaTime / transitionDatas.Duration)
            {
                prev.volume = (1f - t).Ease(transitionDatas.CurveIn) * prevVol;
                next.volume = t.Ease(transitionDatas.CurveOut) * nextVol;
                yield return null;
            }

            next.volume = nextVol;
            prev.volume = 0f;
            prev.Stop();
        }

        private System.Collections.IEnumerator FadeMusicCoroutine(IClipProvider musicProvider, MusicTransitionsDatas transitionDatas)
        {
            AudioSource prev = GetCurrentMusicSource();
            AudioSource next = GetNextMusicSource();
            AudioClipPlayDatas clipDatas = musicProvider.GetNextClipDatas();

            float prevVol = prev.volume;
            float nextVol = clipDatas.RandomVolume;

            for (float t = prevVol; t >= 0f; t -= Time.deltaTime / transitionDatas.Duration * 2f)
            {
                prev.volume = t.Ease(transitionDatas.CurveIn);
                yield return null;
            }

            prev.volume = 0f;
            prev.Stop();

            next.clip = clipDatas.Clip;
            next.pitch = 1f + clipDatas.PitchVariation;
            next.Play();

            for (float t = 0f; t <= 1f; t += Time.deltaTime / transitionDatas.Duration * 2f)
            {
                next.volume = t.Ease(transitionDatas.CurveOut) * nextVol;
                yield return null;
            }

            next.volume = nextVol;
        }

        private static AudioSource GetSFXSource(IClipProvider clipProvider)
        {
            UnityEngine.Assertions.Assert.IsTrue(
                s_sfxPlayersDict.ContainsKey(clipProvider.MixerGroup),
                $"No AudioMixerGroup named {clipProvider.MixerGroup.name} has been registered in {Instance.GetType().Name}.");

            return s_sfxPlayersDict[clipProvider.MixerGroup].GetNextSource();
        }

        private static AudioSource GetCurrentMusicSource()
        {
            return s_musicSources[Helpers.Mod(_nextMusicIndex - 1, s_musicSources.Length)];
        }

        private static AudioSource GetNextMusicSource()
        {
            return s_musicSources[_nextMusicIndex++ % s_musicSources.Length];
        }

        private static void InitSFXSources()
        {
            s_sfxPlayersDict = new System.Collections.Generic.Dictionary<AudioMixerGroup, RuntimeSFXPlayer>();
            for (int i = 0; i < Instance._sfxPlayers.Length; ++i)
            {
                GameObject newPlayerParent = new GameObject($"SFX Group Player - {Instance._sfxPlayers[i].MixerGroup.name}");
                newPlayerParent.transform.SetParent(Instance.transform);
                s_sfxPlayersDict.Add(
                    Instance._sfxPlayers[i].MixerGroup,
                    new RuntimeSFXPlayer(Instance._sfxPlayers[i].AudioSourcesCount, Instance._sfxPlayers[i].MixerGroup, newPlayerParent.transform));
            }
        }

        private static void InitMusicSources()
        {
            s_musicSources = new AudioSource[2];
            GameObject newPlayerParent = new GameObject($"Music Group Player - {Instance._musicMixerGroup.name}");
            newPlayerParent.transform.SetParent(Instance.transform);

            for (int i = 0; i < 2; ++i)
            {
                s_musicSources[i] = CreateAudioSource($"Music Source {i}", newPlayerParent.transform, Instance._musicMixerGroup);
                s_musicSources[i].loop = true;
            }
        }

        private static AudioSource CreateAudioSource(string gameObjectName, Transform parent, AudioMixerGroup mixerGroup)
        {
            GameObject newSource = new GameObject(gameObjectName);
            newSource.transform.SetParent(parent);
            AudioSource source = newSource.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = mixerGroup;

            return source;
        }

        protected override void Awake()
        {
            base.Awake();
            InitSFXSources();
            InitMusicSources();

            _mixer.GetFloat("musicVolume", out BaseMusicVolume);
        }
    }
}