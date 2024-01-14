namespace JuiceJam
{
    using UnityEngine;

    public class Music : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private RSLib.Audio.ClipProvider _musicClip = null;
        [SerializeField] private RSLib.Audio.MusicTransitionsDatas _musicInDatas = null;
        [SerializeField] private float _musicStartDelay = 1f;
        [SerializeField] private UnityEngine.Audio.AudioMixer _mainMixer = null;

        private void OnFirstMovementInput()
        {
            if (!RSLib.Audio.AudioManager.IsMusicPlaying)
                StartCoroutine(StartMusicCoroutine());
        }

        private System.Collections.IEnumerator StartMusicCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(_musicStartDelay);
            RSLib.Audio.AudioManager.PlayMusic(_musicClip, _musicInDatas);
        }

        private void Start()
        {
            _playerController.FirstMovementInput += OnFirstMovementInput;
            _mainMixer.SetFloat("musicVolume", RSLib.Audio.AudioManager.Instance.BaseMusicVolume);
        }

        private void OnDestroy()
        {
            _playerController.FirstMovementInput -= OnFirstMovementInput;
        }
    }
}