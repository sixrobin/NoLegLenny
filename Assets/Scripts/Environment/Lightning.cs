namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class Lightning : MonoBehaviour
    {
        private static readonly int STRIKE_ANIMATOR_HASH = Animator.StringToHash("Strike");

        [SerializeField] private Animator _lightningAnimator = null;
        [SerializeField] private SpriteRenderer _lightningSpriteRenderer = null;
        [SerializeField] private Vector2 _strikesDelayMinMax = new Vector2(1f, 3f);
        [SerializeField, Min(0f)] private float _maxXOffset = 6.5f;
        [SerializeField, Range(0f, 1f)] private float _strikeTrauma = 0f;
        [SerializeField] private bool _startIsOn = false;

        [Header("RAIN")]
        [SerializeField] private Rain _rain = null;

        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _lightningStrikeClip = null;

        private bool _isOn;
        private float _timer;
        private float _nextStrikeDelay;

        public void Toggle(bool on)
        {
            _isOn = on;

            if (_isOn)
                PrepareNextStrike();
            else if (_rain != null)
                _rain.Stop();
        }

        public void OnStrikeFrame()
        {
            CameraShake.Instance.AddTrauma(_strikeTrauma);
            RSLib.Audio.AudioManager.PlayNextPlaylistSound(_lightningStrikeClip);

            if (_rain != null)
                _rain.Play();
        }

        private void Strike()
        {
            _lightningAnimator.transform.SetPositionX(Random.Range(-_maxXOffset, _maxXOffset));
            _lightningAnimator.SetTrigger(STRIKE_ANIMATOR_HASH);
            _lightningSpriteRenderer.flipX = RSLib.Helpers.CoinFlip();
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