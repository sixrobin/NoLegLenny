namespace JuiceJam
{
    using RSLib.Extensions;
    using RSLib.Maths;
    using UnityEngine;

    public class Lava : MonoBehaviour, IRespawnable
    {
        [Header("REFS")]
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private Collider2D _collider = null;
        [SerializeField] private Transform _minHeightReference = null;
        [SerializeField] private GameObject _lavaView = null;
        [SerializeField] private SpriteRenderer _lavaSpriteRenderer = null;

        [Header("MOVEMENT")]
        [SerializeField, Min(0.1f)] private float _speed = 1f;
        [SerializeField, Min(1f)] private float _maxHeightOffset = 15f;
        [SerializeField, Min(1f)] private float _moonReachedSpeed = 15f;

        [Header("RESPAWN")]
        [SerializeField, Min(0f)] private float _respawnCheckpointOffset = 2f;

        [Header("OPTION TOGGLE")]
        [SerializeField] private float _minDistanceToKillOnReenable = 1f;

        [Header("VFX")]
        [SerializeField] private GameObject _lavaKillEffects = null;

        [Header("AUDIO")]
        [SerializeField] private AudioSource _lavaIdleSource = null;
        [SerializeField] private Vector2 _lavaIdleVolumeRange = new Vector2(0f, 0.5f);
        [SerializeField] private Curve _lavaIdleVolumeCurve = Curve.Linear;
        [SerializeField] private RSLib.Audio.ClipProvider _lavaKillClip = null;

        private bool _isOn;
        private bool _isEnabled;
        private float _initHeight;
        private bool _moonReached;

        public void Respawn()
        {
            _isOn = false;
            _moonReached = false;
            _lavaView.SetActive(true);

            transform.SetPositionY(Checkpoint.LastCheckpoint != null ? Checkpoint.LastCheckpoint.RespawnPosition.y - _respawnCheckpointOffset : _initHeight);
        }

        private void OnFirstMovementInput()
        {
            _isOn = true;
        }

        private void OnLavaToggleValueChanged(bool currentValue)
        {
            _isEnabled = currentValue;
            gameObject.SetActive(_isEnabled);

            if (!currentValue)
                _collider.enabled = false;
        }

        private void OnMoonFinalPositionReached()
        {
            _moonReached = true;
            _lavaView.SetActive(false);
        }

        private void AdjustLavaVolume()
        {
            float percentage = RSLib.Maths.Maths.Normalize01(
                                                 transform.position.y,
                                                 _minHeightReference.position.y - _maxHeightOffset,
                                                 _minHeightReference.position.y);

            float volumeRanged = Mathf.LerpUnclamped(_lavaIdleVolumeRange.x, _lavaIdleVolumeRange.y, percentage.Ease(_lavaIdleVolumeCurve));
            _lavaIdleSource.volume = volumeRanged * (1f - DitherFade.FadedPercentage);
        }

        private void Start()
        {
            Settings.SettingsManager.LavaToggle.ValueChanged += OnLavaToggleValueChanged;
            Moon.MoonFinalPositionReached += OnMoonFinalPositionReached;

            _isOn = true;
            _initHeight = transform.position.y;

            if (_playerController != null)
            {
                _playerController.FirstMovementInput += OnFirstMovementInput;
                _isOn = false;
            }

            OnLavaToggleValueChanged(Settings.SettingsManager.LavaToggle.Value);
        }

        private void Update()
        {
            AdjustLavaVolume();

            if (!_isOn || _playerController.IsDead || _moonReached)
                return;

            // Handle lava being reenabled by game settings.
            if (!_collider.enabled && _isEnabled && !DitherFade.IsFading && !UI.OptionsPanel.Instance.IsOpen)
            {
                _collider.enabled = true;

                if (_playerController.transform.position.y - _minDistanceToKillOnReenable < transform.position.y)
                {
                    _playerController.TakeDamage(new DamageData()
                    {
                        Source = this,
                        Amount = int.MaxValue
                    });

                    _isOn = false;

                    GameObject killParticles = Instantiate(_lavaKillEffects, _playerController.transform.position, _lavaKillEffects.transform.rotation);
                    Destroy(killParticles, 3f);

                    if (_lavaSpriteRenderer.isVisible)
                        RSLib.Audio.AudioManager.PlayNextPlaylistSound(_lavaKillClip);
                }
            }

            if (Checkpoint.LastCheckpoint == null
                || !Checkpoint.LastCheckpoint.IsPlayerOnCheckpoint
                || transform.position.y < Checkpoint.LastCheckpoint.RespawnPosition.y - _respawnCheckpointOffset)
                transform.Translate(0f, _speed * Time.deltaTime, 0f, Space.World);

            if (_minHeightReference.position.y - transform.position.y > _maxHeightOffset)
                transform.SetPositionY(_minHeightReference.position.y - _maxHeightOffset);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out PlayerController playerController))
            {
                playerController.TakeDamage(new DamageData()
                {
                    Source = this,
                    Amount = int.MaxValue
                });

                _isOn = false;

                GameObject killParticles = Instantiate(_lavaKillEffects, playerController.transform.position, _lavaKillEffects.transform.rotation);
                Destroy(killParticles, 3f);

                if (_lavaSpriteRenderer.isVisible)
                    RSLib.Audio.AudioManager.PlayNextPlaylistSound(_lavaKillClip);
            }
            else if (collision.TryGetComponent(out FollowingEnemy enemy))
            {
                enemy.Kill();

                GameObject killParticles = Instantiate(_lavaKillEffects, enemy.transform.position, _lavaKillEffects.transform.rotation);
                Destroy(killParticles, 3f);

                if (_lavaSpriteRenderer.isVisible)
                    RSLib.Audio.AudioManager.PlayNextPlaylistSound(_lavaKillClip);
            }
        }

        private void OnDestroy()
        {
            Settings.SettingsManager.LavaToggle.ValueChanged -= OnLavaToggleValueChanged;
            Moon.MoonFinalPositionReached -= OnMoonFinalPositionReached;

            if (_playerController != null)
                _playerController.FirstMovementInput -= OnFirstMovementInput;
        }
    }
}