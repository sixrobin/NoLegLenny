namespace JuiceJam
{
    using RSLib.Maths;
    using UnityEngine;

    public class SpaceMusic : MonoBehaviour
    {
        [Header("GENERAL")]
        [SerializeField] private RSLib.Audio.ClipProvider _musicClip = null;
        [SerializeField] private RSLib.Audio.MusicTransitionsDatas _musicInDatas = null;
        [SerializeField] private float _musicStartDelay = 1f;
        [SerializeField] private UnityEngine.Audio.AudioMixer _mainMixer = null;

        [Header("REFERENCE POSITIONS")]
        [SerializeField] private Transform _normalizationTarget = null;
        [SerializeField] private Transform _fadeOutPositionA = null;
        [SerializeField] private Transform _fadeOutPositionB = null;

        [Header("FADE")]
        [SerializeField] private Curve _fadeOutCurve = Curve.InCirc;

        public bool IsPlaying { get; private set; }

        public void PlayMusic()
        {
            StartCoroutine(StartMusicCoroutine());
        }

        private void OnMoonFinalPositionReached()
        {
            RSLib.Audio.AudioManager.StopMusic();
        }

        private void AdjustVolume()
        {
            if (!IsPlaying)
                return;

            float targetVolume;

            if (_normalizationTarget.position.y < _fadeOutPositionA.position.y)
            {
                targetVolume = RSLib.Audio.AudioManager.Instance.BaseMusicVolume;
            }
            else
            {
                // Fade out.
                float percentage = _normalizationTarget.position.y.Normalize01(_fadeOutPositionA.position.y, _fadeOutPositionB.position.y);
                targetVolume = Mathf.LerpUnclamped(-80f, RSLib.Audio.AudioManager.Instance.BaseMusicVolume, 1f - percentage.Ease(_fadeOutCurve));
            }

            _mainMixer.SetFloat("musicVolume", targetVolume);
        }

        private System.Collections.IEnumerator StartMusicCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(_musicStartDelay);

            _mainMixer.SetFloat("musicVolume", RSLib.Audio.AudioManager.Instance.BaseMusicVolume);
            RSLib.Audio.AudioManager.PlayMusic(_musicClip, _musicInDatas);

            IsPlaying = true;
        }

        private void Awake()
        {
            Moon.MoonFinalPositionReached += OnMoonFinalPositionReached;
        }

        private void Update()
        {
            AdjustVolume();
        }

        private void OnDestroy()
        {
            Moon.MoonFinalPositionReached -= OnMoonFinalPositionReached;
        }
    }
}