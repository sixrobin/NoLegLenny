namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class Lava : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private Transform _minHeightReference = null;
        [SerializeField, Min(0.1f)] private float _speed = 1f;
        [SerializeField, Min(1f)] private float _maxHeightOffset = 15f;

        [Header("VFX")]
        [SerializeField] private GameObject _lavaKillEffects = null;

        private bool _isOn;

        private void OnFirstMovementInput()
        {
            _isOn = true;
        }

        private void Update()
        {
            if (!_isOn || _playerController.IsDead)
                return;

            transform.Translate(0f, _speed * Time.deltaTime, 0f, Space.World);
            if (_minHeightReference.position.y - transform.position.y > _maxHeightOffset)
                transform.SetPositionY(_minHeightReference.position.y - _maxHeightOffset);
        }

        private void Awake()
        {
            _isOn = true;

            if (_playerController != null)
            {
                _playerController.FirstMovementInput += OnFirstMovementInput;
                _isOn = false;
            }
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