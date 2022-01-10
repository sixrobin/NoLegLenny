namespace JuiceJam
{
    using UnityEngine;

    public class SpaceMusic : MonoBehaviour
    {
        [SerializeField] private RSLib.Audio.ClipProvider _musicClip = null;
        [SerializeField] private RSLib.Audio.MusicTransitionsDatas _musicInDatas = null;
        [SerializeField] private float _musicStartDelay = 1f;
        [SerializeField] private UnityEngine.Audio.AudioMixer _mainMixer = null;

        public bool IsPlaying { get; private set; }

        public void PlayMusic()
        {
            IsPlaying = true;
            StartCoroutine(StartMusicCoroutine());
        }

        private void OnMoonFinalPositionReached()
        {
            _mainMixer.SetFloat("musicVolume", -80f);
        }

        private System.Collections.IEnumerator StartMusicCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_musicStartDelay);

            _mainMixer.SetFloat("musicVolume", RSLib.Audio.AudioManager.Instance.BaseMusicVolume);
            RSLib.Audio.AudioManager.PlayMusic(_musicClip, _musicInDatas);
        }

        private void Awake()
        {
            Moon.MoonFinalPositionReached += OnMoonFinalPositionReached;
        }

        private void OnDestroy()
        {
            Moon.MoonFinalPositionReached -= OnMoonFinalPositionReached;
        }
    }
}