namespace JuiceJam
{
    using UnityEngine;
    using RSLib.Extensions;

    public class PlayerController : MonoBehaviour
    {
        [Header("PLAYER")]
        [SerializeField] private float _shootImpulseForce = 8f;
        [SerializeField] private float _fallMultiplier = 1.05f;
        [SerializeField] private float _groundBrakePercentage = 0.9f;
        [SerializeField] private float _xVelocityMax = 5f;
        [SerializeField] private Vector2 _yVelocityMinMax = new Vector2(-3f, 10f);
        [SerializeField, Range(0f, 1f)] private float _upwardMovementKeptOnShootPercentage = 0.5f;
        [SerializeField] private LayerMask _groundMask = 0;
        [SerializeField] private Animator _playerAnimator = null;

        [Header("WEAPON")]
        [SerializeField] private Transform _weaponPivot = null;
        [SerializeField] private SpriteRenderer _weaponSpriteRenderer = null;
        [SerializeField] private Animator _weaponAnimator = null;
        [SerializeField] private ParticleSystem[] _shootParticlesSystems = null;
        [SerializeField] private float _shootTrauma = 0.1f;

        [Header("BULLET")]
        [SerializeField] private Bullet _bulletPrefab = null;
        [SerializeField] private Transform _bulletSpawnPosition = null;

        [Header("VFX")]
        [SerializeField] private float _shootFreezeFrameDur = 0f;
        [SerializeField] private GameObject _impulsePuffPrefab = null;
        [SerializeField] private Transform _impulsePuffPivot = null;
        [SerializeField] private float _landSidePuffMinSpeed = 1f;
        [SerializeField] private GameObject _landPuffPrefab = null;
        [SerializeField] private GameObject _landSidePuffPrefab = null;
        [SerializeField] private Transform _landPuffPivot = null;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;

        private Vector2 _aimDirection;
        private Vector2 _shootImpulse;
        private float _weaponPivotXOffset;

        private bool _isGrounded;

        private void CheckGround()
        {
            _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, _groundMask);
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

                _weaponAnimator.SetTrigger("Shoot");
                _playerAnimator.SetTrigger("Shoot");

                for (int i = _shootParticlesSystems.Length - 1; i >= 0; --i)
                    _shootParticlesSystems[i].Play();

                if (_isGrounded)
                {
                    GameObject sidePuff = Instantiate(_landSidePuffPrefab, _landPuffPivot.position, _landSidePuffPrefab.transform.rotation);
                    sidePuff.GetComponent<SpriteRenderer>().flipX = _shootImpulse.x < 0f;
                }

                FreezeFrameManager.FreezeFrame(0, _shootFreezeFrameDur);
                CameraShake.SetTrauma(_shootTrauma);
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
            _playerAnimator.SetTrigger("Land");

            if (Mathf.Abs(_rigidbody2D.velocity.x) > _landSidePuffMinSpeed)
            {
                GameObject sidePuff = Instantiate(_landSidePuffPrefab, _landPuffPivot.position, _landSidePuffPrefab.transform.rotation);
                sidePuff.GetComponent<SpriteRenderer>().flipX = _rigidbody2D.velocity.x < 0f;
            }
            else
            {
                Instantiate(_landPuffPrefab, _landPuffPivot.position, _landPuffPrefab.transform.rotation);
            }
        }

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            _weaponPivotXOffset = _weaponPivot.transform.localPosition.x;
        }

        private void Update()
        {
            bool previousIsGrounded = _isGrounded;
            CheckGround();

            if (_isGrounded && _isGrounded != previousIsGrounded)
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

                if (_shootImpulse.y > 0f && _isGrounded)
                    Instantiate(_impulsePuffPrefab, _impulsePuffPivot.position, _impulsePuffPrefab.transform.rotation);

                _shootImpulse = Vector2.zero;
            }

            if (!_isGrounded && _rigidbody2D.velocity.y < 0f)
                _rigidbody2D.velocity *= new Vector2(1f, _fallMultiplier);

            if (_isGrounded)
                _rigidbody2D.velocity *= new Vector2(_groundBrakePercentage, 1f);

            _rigidbody2D.velocity = new Vector2(
                Mathf.Clamp(_rigidbody2D.velocity.x, -_xVelocityMax, _xVelocityMax),
                Mathf.Clamp(_rigidbody2D.velocity.y, _yVelocityMinMax.x, _yVelocityMinMax.y));
        }
    }
}