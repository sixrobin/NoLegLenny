namespace JuiceJam
{
    using UnityEngine;
    using RSLib.Extensions;

    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("VIEW")]
        [SerializeField] private PlayerView _playerView = null;

        [Header("PLAYER")]
        [SerializeField, Min(0f)] private float _shootImpulseForce = 8f;
        [SerializeField, Min(1f)] private float _fallMultiplier = 1.05f;
        [SerializeField, Range(0f, 1f)] private float _groundBrakePercentage = 0.9f;
        [SerializeField, Min(0f)] private float _xVelocityMax = 5f;
        [SerializeField] private Vector2 _yVelocityMinMax = new Vector2(-3f, 10f);
        [SerializeField, Range(0f, 1f)] private float _upwardMovementKeptOnShootPercentage = 0.5f;
        [SerializeField] private LayerMask _groundMask = 0;

        [Header("WEAPON")]
        [SerializeField] private Transform _weaponPivot = null;
        [SerializeField] private SpriteRenderer _weaponSpriteRenderer = null;

        [Header("BULLET")]
        [SerializeField] private Bullet _bulletPrefab = null;
        [SerializeField] private Transform _bulletSpawnPosition = null;

        [Header("HEALTH")]
        [SerializeField, Min(0f)] private float _invulnerabilityWindowDuration = 1f;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;

        private Vector2 _aimDirection;
        private Vector2 _shootImpulse;
        private float _weaponPivotXOffset;

        public bool IsGrounded { get; private set; }

        public bool IsInvulnerable { get; private set; }

        public void TakeDamage(DamageData damageData)
        {
            if (IsInvulnerable)
                return;

            Debug.Log("Health -= amount");
            StartCoroutine(InvulnerabilityWindowCoroutine());

            _playerView.PlayDamageAnimation(damageData);
        }

        private void CheckGround()
        {
            IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, _groundMask);
        }

        private void Aim()
        {
            Vector2 joystickAimDirection = new Vector2(Input.GetAxis("AimHorizontal"), Input.GetAxis("AimVertical"));

            if (joystickAimDirection.magnitude > 0.01f)
            {
                _aimDirection = joystickAimDirection;
            }
            else
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0f;
                _aimDirection = (mouseWorldPosition - _weaponPivot.position).normalized;
            }

            _weaponPivot.transform.right = _aimDirection;

            _spriteRenderer.flipX = _aimDirection.x < 0f;
            _weaponSpriteRenderer.flipY = _aimDirection.x < 0f;
            _weaponPivot.transform.SetLocalPositionX(_weaponPivotXOffset * Mathf.Sign(_aimDirection.x));
        }

        private void Shoot()
        {
            if (Input.GetButtonDown("Fire"))
            {
                _shootImpulse = -_aimDirection * _shootImpulseForce;
                SpawnBullet();

                _playerView.PlayShootAnimation(_shootImpulse);
                FreezeFrameManager.FreezeFrame(0, _playerView.ShootFreezeFrameDuration);
            }
        }

        private void SpawnBullet()
        {
            Collider2D[] nearColliders = Physics2D.OverlapCircleAll(_bulletSpawnPosition.position, 0.3f);
            for (int i = nearColliders.Length - 1; i >= 0; --i)
                if (nearColliders[i].gameObject.layer == LayerMask.NameToLayer("Ground"))
                    return;

            Bullet bullet = Instantiate(_bulletPrefab, _bulletSpawnPosition.position, Quaternion.identity);
            bullet.Launch(_weaponPivot.right);
        }

        private void Land()
        {
            _playerView.PlayLandAnimation(_rigidbody2D.velocity);
        }

        private System.Collections.IEnumerator InvulnerabilityWindowCoroutine()
        {
            IsInvulnerable = true;
            
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_invulnerabilityWindowDuration);

            IsInvulnerable = false;
        }

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            _weaponPivotXOffset = _weaponPivot.transform.localPosition.x;
        }

        private void Update()
        {
            bool previousIsGrounded = IsGrounded;
            CheckGround();

            if (IsGrounded && IsGrounded != previousIsGrounded)
                Land();

            Aim();
            Shoot();
        }

        private void FixedUpdate()
        {
            if (_shootImpulse.magnitude > 0.05f)
            {
                float previousY = _rigidbody2D.velocity.y;
                _rigidbody2D.velocity = _shootImpulse;
                if (previousY > 0f)
                    _rigidbody2D.velocity += new Vector2(1f, previousY * _upwardMovementKeptOnShootPercentage);

                if (_shootImpulse.y > 0f && IsGrounded)
                    _playerView.PlayImpulseAnimation(_shootImpulse);

                _shootImpulse = Vector2.zero;
            }

            if (!IsGrounded && _rigidbody2D.velocity.y < 0f)
                _rigidbody2D.velocity *= new Vector2(1f, _fallMultiplier);

            if (IsGrounded)
                _rigidbody2D.velocity *= new Vector2(_groundBrakePercentage, 1f);

            _rigidbody2D.velocity = new Vector2(
                Mathf.Clamp(_rigidbody2D.velocity.x, -_xVelocityMax, _xVelocityMax),
                Mathf.Clamp(_rigidbody2D.velocity.y, _yVelocityMinMax.x, _yVelocityMinMax.y));
        }
    }
}