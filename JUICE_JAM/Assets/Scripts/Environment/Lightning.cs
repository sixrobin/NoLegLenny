namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class Lightning : MonoBehaviour
    {
        [SerializeField] private Animator _lightningAnimator = null;
        [SerializeField] private Vector2 _strikesDelayMinMax = new Vector2(1f, 3f);
        [SerializeField, Min(0f)] private float _maxXOffset = 6.5f;
        [SerializeField, Range(0f, 1f)] private float _strikeTrauma = 0f;
        [SerializeField] private bool _startIsOn = false;

        private bool _isOn;
        private float _timer;
        private float _nextStrikeDelay;

        public void Toggle(bool on)
        {
            _isOn = on;
            if (_isOn)
                PrepareNextStrike();
        }

        public void OnStrikeFrame()
        {
            CameraShake.AddTrauma(_strikeTrauma);
        }

        private void Strike()
        {
            _lightningAnimator.transform.SetPositionX(Random.Range(-_maxXOffset, _maxXOffset));
            _lightningAnimator.SetTrigger("Strike");
        }

        private void PrepareNextStrike()
        {
            _timer = 0f;
            _nextStrikeDelay = Random.Range(_strikesDelayMinMax.x, _strikesDelayMinMax.y);
        }

        private void Start()
        {
            _isOn = _startIsOn;
        }

        private void Update()
        {
            if (!_isOn)
                return;

            _timer += Time.deltaTime;
            if (_timer >= _nextStrikeDelay)
            {
                Strike();
                PrepareNextStrike();
            }
        }
    }
}