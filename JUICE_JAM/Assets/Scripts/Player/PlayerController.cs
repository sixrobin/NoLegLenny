namespace JuiceJam
{
    using UnityEngine;
    using RSLib.Extensions;

    public class PlayerController : MonoBehaviour, IDamageable, IExplodable, IRespawnable
    {
        [Header("VIEW")]
        [SerializeField] private PlayerView _playerView = null;

        [Header("PLAYER")]
        [SerializeField, Min(0f)] private float _shootImpulseForce = 8f;
        [SerializeField, Min(1f)] private float _fallMultiplier = 1.05f;
        [SerializeField, Range(0f, 1f)] private float _groundBrakePercentage = 0.9f;
        [SerializeField, Range(0f, 1f)] private float _checkpointBrakePercentage = 0.9f;
        [SerializeField, Min(0f)] private float _xVelocityMax = 5f;
        [SerializeField] private Vector2 _yVelocityMinMax = new Vector2(-3f, 10f);
        [SerializeField, Range(0f, 1f)] private float _movementKeptOnShootPercentage = 0.5f;
        [SerializeField] private LayerMask _groundMask = 0;

        [Header("WEAPON")]
        [SerializeField, Range(0f, 1f)] private float _fireAxisMinValue = 0.8f;
        [SerializeField] private Transform _weaponPivot = null;
        [SerializeField] private SpriteRenderer _weaponSpriteRenderer = null;
        [SerializeField] private GameObject _weaponView = null;

        [Header("BULLET")]
        [SerializeField] private Bullet _bulletPrefab = null;
        [SerializeField] private Transform _bulletSpawnPosition = null;

        [Header("HEALTH")]
        [SerializeField, Min(1)] private int _maxHealth = 3;
        [SerializeField] private RSLib.Dynamics.DynamicInt _health = null;
        [SerializeField, Min(0f)] private float _invulnerabilityWindowDuration = 1f;

        private Vector3 _initPosition;
        private bool _firstMovementInputDone;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private GameObject _droppedWeapon;

        private Vector2 _aimDirection;
        private Vector2 _shootImpulse;
        private float _weaponPivotXOffset;
        private float _lastFireAxisValue;
        private float _shootForceMultiplier = 1f;

        private Vector3 _lastMousePosition;

        public event System.Action FirstMovementInput;

        public enum ControllerType
        {
            Mouse,
            Joystick
        }

        public ControllerType LastControllerType { get; private set; }

        public bool CanBeDamaged => !IsDead && !IsInvulnerable;
        public bool DontDestroyDamageSource => false;

        public RaycastHit2D GroundHit { get; private set; }

        public bool IsInvulnerable { get; private set; }

        public bool IsDead => _health.Value == 0;

        public bool IsClouded { get; set; }

        public void TakeDamage(DamageData damageData)
        {
            if (IsInvulnerable || IsDead || DitherFade.IsFading)
                return;

            _health.Value = Mathf.Max(0, _health.Value - damageData.Amount);

            if (_health.Value == 0)
            {
                if (damageData.Source is Lava)
                {
                    _rigidbody2D.simulated = false;
                    _playerView.DisplayPlayer(false);
                    TimeManager.FreezeFrame(_playerView.DeathFreezeFrameDelay, _playerView.DeathFreezeFrameDuration);
                    CameraShake.SetTrauma(_playerView.DeathTrauma);
                }
                else
                {
                    DropWeapon();
                    _playerView.PlayDeathAnimation();
                    TimeManager.FreezeFrame(_playerView.DeathFreezeFrameDelay, _playerView.DeathFreezeFrameDuration);
                }

                GameController.Respawn();
            }
            else
            {
                StartCoroutine(InvulnerabilityWindowCoroutine());

                _playerView.PlayDamageAnimation(damageData);
                TimeManager.FreezeFrame(_playerView.DamageFreezeFrameDelay, _playerView.DamageFreezeFrameDuration);
            }
        }

        public void Explode(ExplosionData explosionData)
        {
            _rigidbody2D.AddForce(explosionData.ComputeRelativeDirection(transform.position) * explosionData.ComputeForceAtPosition(transform.position), ForceMode2D.Impulse);
        }

        public void Respawn()
        {
            transform.position = Checkpoint.LastCheckpoint?.RespawnPosition ?? _initPosition;

            _health.Value = _maxHealth;
            _firstMovementInputDone = false;

            _rigidbody2D.NullifyMovement();
            _rigidbody2D.simulated = true;

            _playerView.Respawn();
            _weaponView.SetActive(true);
            if (_droppedWeapon != null)
                Destroy(_droppedWeapon);
        }

        public void SetGravityScale(float value)
        {
            _rigidbody2D.gravityScale = value;
        }

        public void SetShootForceMultiplier(float value)
        {
            _shootForceMultiplier = value;
        }

        private void CheckGround()
        {
            GroundHit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, _groundMask);
        }

        private void Aim()
        {
            Vector2 joystickAimDirection = new Vector2(Input.GetAxis("AimHorizontal"), Input.GetAxis("AimVertical"));

            if (joystickAimDirection.magnitude > 0.01f)
            {
                _aimDirection = joystickAimDirection;
                if (_lastMousePosition == Input.mousePosition)
                    LastControllerType = ControllerType.Joystick;
            }
            else
            {
                if (LastControllerType != ControllerType.Joystick || Input.mousePosition != _lastMousePosition)
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mouseWorldPosition.z = 0f;
                    _aimDirection = (mouseWorldPosition - _weaponPivot.position).normalized;

                    LastControllerType = ControllerType.Mouse;
                }
            }

            if (Settings.SettingsManager.AxisReverse.Value)
                _aimDirection *= -1;

            _weaponPivot.transform.right = _aimDirection;

            _spriteRenderer.flipX = _aimDirection.x < 0f;
            _weaponSpriteRenderer.flipY = _aimDirection.x < 0f;
            _weaponPivot.transform.SetLocalPositionX(_weaponPivotXOffset * Mathf.Sign(_aimDirection.x));

            _lastMousePosition = Input.mousePosition;
        }

        private void Shoot()
        {
            float fireAxis = Input.GetAxis("FireAxis");

            if (Input.GetButtonDown("Fire")
                || (fireAxis > _fireAxisMinValue && _lastFireAxisValue < _fireAxisMinValue))
            {
                _shootImpulse = -_aimDirection * _shootImpulseForce * _shootForceMultiplier;
                SpawnBullet();

                if (!_firstMovementInputDone)
                {
                    FirstMovementInput?.Invoke();
                    _firstMovementInputDone = true;
                }

                _playerView.PlayShootAnimation(_shootImpulse);
                TimeManager.FreezeFrame(0, _playerView.ShootFreezeFrameDuration);
            }

            _lastFireAxisValue = fireAxis;
        }

        private void SpawnBullet()
        {
            if (IsClouded)
                return;

            //Collider2D[] nearColliders = Physics2D.OverlapCircleAll(_bulletSpawnPosition.position, 0.15f);
            //for (int i = nearColliders.Length - 1; i >= 0; --i)
            //    if (nearColliders[i].gameObject.layer == LayerMask.NameToLayer("Ground"))
            //        return;

            Bullet bullet = Instantiate(_bulletPrefab, _bulletSpawnPosition.position, Quaternion.identity);
            bullet.Launch(_weaponPivot.right, false);
        }

        private void Land()
        {
            _playerView.PlayLandAnimation(_rigidbody2D.velocity);
        }

        private void DropWeapon()
        {
            _droppedWeapon = Instantiate(_weaponView, _weaponView.transform.position, _weaponView.transform.rotation);
            _weaponView.SetActive(false);

            _droppedWeapon.GetComponent<Collider2D>().enabled = true;

            Rigidbody2D weaponRigidbody2D = _droppedWeapon.gameObject.AddComponent<Rigidbody2D>();
            weaponRigidbody2D.sharedMaterial = _droppedWeapon.GetComponent<Collider2D>().sharedMaterial;
            weaponRigidbody2D.AddForce(Random.insideUnitCircle.normalized * 10f, ForceMode2D.Impulse);
            weaponRigidbody2D.AddTorque(90f);
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

            _initPosition = transform.position;
            _weaponPivotXOffset = _weaponPivot.transform.localPosition.x;

            _health.Value = _maxHealth;
        }

        private void Update()
        {
            if (UI.OptionsPanel.Instance.IsOpen || UI.OptionsPanel.Instance.PausingCoroutineRunning)
                return;

            bool previousIsGrounded = GroundHit;
            CheckGround();

            // Need to check ground to brake velocity, even in death.
            if (IsDead)
                return;

            if (GroundHit && GroundHit != previousIsGrounded)
                Land();

            Aim();
            Shoot();
        }

        private void FixedUpdate()
        {
            if (_shootImpulse.magnitude > 0.05f)
            {
                Vector2 previousVelocity = _rigidbody2D.velocity;
                _rigidbody2D.velocity = _shootImpulse;
                if (previousVelocity.magnitude > 0f)
                    _rigidbody2D.velocity += previousVelocity * _movementKeptOnShootPercentage;

                if (_shootImpulse.y > 0f && GroundHit)
                    _playerView.PlayImpulseAnimation(_shootImpulse);

                _shootImpulse = Vector2.zero;
            }

            if (!GroundHit && _rigidbody2D.velocity.y < 0f)
                _rigidbody2D.velocity *= new Vector2(1f, _fallMultiplier);

            if (GroundHit)
            {
                float brakePercentage = GroundHit.collider.gameObject.layer == LayerMask.NameToLayer("Checkpoint")
                    ? _checkpointBrakePercentage
                    : _groundBrakePercentage;

                _rigidbody2D.velocity *= new Vector2(brakePercentage, 1f);
            }

            _rigidbody2D.velocity = new Vector2(
                Mathf.Clamp(_rigidbody2D.velocity.x, -_xVelocityMax, _xVelocityMax),
                Mathf.Clamp(_rigidbody2D.velocity.y, _yVelocityMinMax.x, _yVelocityMinMax.y));
        }
    }
}