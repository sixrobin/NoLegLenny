namespace JuiceJam
{
    using RSLib.Maths;
    using UnityEngine;

    public class WindVolume : MonoBehaviour
    {
        [Header("AUDIO SOURCE")]
        [SerializeField] private AudioSource _windAudioSource = null;

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

        private void AdjustVolume()
        {
            if (_normalizationTarget.position.y < _windFadeInPositionA.position.y || _normalizationTarget.position.y > _windFadeOutPositionB.position.y)
            {
                _windAudioSource.volume = 0f;
            }
            else if (_normalizationTarget.position.y > _windFadeInPositionB.position.y && _normalizationTarget.position.y < _windFadeOutPositionA.position.y)
            {
                _windAudioSource.volume = _baseVolume;
            }
            else
            {
                float targetVolume;

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
                    targetVolume = RSLib.Maths.Maths.Normalize(
                                   _normalizationTarget.position.y,
                                   _windFadeOutPositionA.position.y,
                                   _windFadeOutPositionB.position.y,
                                   _baseVolume,
                                   0f);
                }

                _windAudioSource.volume = targetVolume;
            }
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