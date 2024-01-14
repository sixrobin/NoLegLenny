namespace JuiceJam
{
    using RSLib.Maths;
    using UnityEngine;

    public class WindVolume : MonoBehaviour
    {
        [Header("REFS")]
        [SerializeField] private AudioSource _windAudioSource = null;
        [SerializeField] private UnityEngine.Audio.AudioMixer _mainMixer = null;

        [Header("REFERENCE POSITIONS")]
        [SerializeField] private Transform _normalizationTarget = null;
        [SerializeField] private Transform _windFadeInPositionA = null;
        [SerializeField] private Transform _windFadeInPositionB = null;
        [SerializeField] private Transform _windFadeOutPositionA = null;
        [SerializeField] private Transform _windFadeOutPositionB = null;

        [Header("FADE")]
        [SerializeField] private Curve _fadeInCurve = Curve.InCirc;
        [SerializeField] private Curve _fadeOutCurve = Curve.InCirc;

        private float _baseVolume;
        private bool _hasTurnedOffMusic = false;

        private void AdjustVolume()
        {
            float targetVolume;

            if (_normalizationTarget.position.y < _windFadeInPositionA.position.y || _normalizationTarget.position.y > _windFadeOutPositionB.position.y)
            {
                if (!_hasTurnedOffMusic && _normalizationTarget.position.y > _windFadeOutPositionB.position.y)
                {
                    _mainMixer.SetFloat("musicVolume", -80f);
                    _hasTurnedOffMusic = true;
                }

                targetVolume = 0f;
            }
            else if (_normalizationTarget.position.y > _windFadeInPositionB.position.y && _normalizationTarget.position.y < _windFadeOutPositionA.position.y)
            {
                targetVolume = _baseVolume;
                _hasTurnedOffMusic = false;
            }
            else
            {
                if (_normalizationTarget.position.y < _windFadeInPositionB.position.y)
                {
                    float percentage = RSLib.Maths.Maths.Normalize01(
                                       _normalizationTarget.position.y,
                                       _windFadeInPositionA.position.y,
                                       _windFadeInPositionB.position.y);

                    targetVolume = Mathf.Lerp(0f, _baseVolume, percentage.Ease(_fadeInCurve));
                }
                else
                {
                    // Fade out.
                    float percentage = RSLib.Maths.Maths.Normalize01(
                                       _normalizationTarget.position.y,
                                       _windFadeOutPositionA.position.y,
                                       _windFadeOutPositionB.position.y);

                    targetVolume = Mathf.LerpUnclamped(_baseVolume, 0f, percentage);
                    _mainMixer.SetFloat("musicVolume", Mathf.LerpUnclamped(-80f, RSLib.Audio.AudioManager.Instance.BaseMusicVolume, 1f - percentage.Ease(_fadeInCurve)));
                }

                _hasTurnedOffMusic = false;
            }

            _windAudioSource.volume = targetVolume * (1f - DitherFade.FadedPercentage);
        }

        private void Start()
        {
            _baseVolume = _windAudioSource.volume;
        }

        private void Update()
        {
            AdjustVolume();
        }
    }
}