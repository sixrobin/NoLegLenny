namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class Lava : MonoBehaviour, IRespawnable
    {
        [Header("REFS")]
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private Transform _minHeightReference = null;

        [Header("MOVEMENT")]
        [SerializeField, Min(0.1f)] private float _speed = 1f;
        [SerializeField, Min(1f)] private float _maxHeightOffset = 15f;

        [Header("RESPAWN")]
        [SerializeField, Min(0f)] private float _respawnCheckpointOffset = 2f;

        [Header("VFX")]
        [SerializeField] private GameObject _lavaKillEffects = null;

        private bool _isOn;
        private float _initHeight;

        public void Respawn()
        {
            _isOn = false;
            transform.SetPositionY(Checkpoint.LastCheckpoint != null ? Checkpoint.LastCheckpoint.RespawnPosition.y - _respawnCheckpointOffset : _initHeight);
        }

        private void OnFirstMovementInput()
        {
            _isOn = true;
        }

        private void Awake()
        {
            _isOn = true;
            _initHeight = transform.position.y;

            if (_playerController != null)
            {
                _playerController.FirstMovementInput += OnFirstMovementInput;
                _isOn = false;
            }
        }

        private void Update()
        {
            if (!_isOn || _playerController.IsDead)
                return;

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
            }
        }

        private void OnDestroy()
        {
            if (_playerController != null)
                _playerController.FirstMovementInput -= OnFirstMovementInput;
        }
    }
}