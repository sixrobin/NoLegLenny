namespace JuiceJam
{
    using RSLib.Extensions;
    using RSLib.Maths;
    using UnityEngine;

    public class FollowingEnemy : MonoBehaviour, IRespawnable, IDamageable, IExplodable
    {
        [Header("REFS")]
        [SerializeField] private Rigidbody2D _rigidbody2D = null;
        [SerializeField] private FollowingEnemyView _followingEnemyView = null;

        [Header("DETECTION")]
        [SerializeField] private float _detectionDistance = 7f;
        [SerializeField] private float _chargeDistance = 3f;
        [SerializeField] private float _stopFollowDistance = 9f;
        [SerializeField] private float _maxHeightFollowFromInitPosition = 10f;

        [Header("CHARGE")]
        [SerializeField] private float _chargeDelay = 0.5f;
        [SerializeField] private float _chargeLength = 4f;
        [SerializeField] private float _chargeDuration = 0.8f;
        [SerializeField] private AnimationCurve _chargeCurve = new();
        [SerializeField] private float _chargeCooldown = 0.5f;

        [Header("SPEED")]
        [SerializeField] private float _baseSpeed = 1f;

        private PlayerController _player;
        private Vector3 _startPosition;
        
        private bool _isFollowing;
        private bool _isCharging;
        private bool _chargeOnCooldown;

        public bool CanBeDamaged => true;
        public bool DontDestroyDamageSource => false;

        public void TakeDamage(DamageData damageData)
        {
            throw new System.NotImplementedException();
        }

        public void Explode(ExplosionData explosionData)
        {
            throw new System.NotImplementedException();
        }

        public void Respawn()
        {
            throw new System.NotImplementedException();
        }

        private bool IsPlayerInRange(float range)
        {
            return (_player.transform.position - transform.position).sqrMagnitude < range * range;
        }

        private void UpdateFollowState()
        {
            if (!_isFollowing)
            {
                if (IsPlayerInRange(_detectionDistance))
                    _isFollowing = true;
            }
            else
            {
                if (!IsPlayerInRange(_stopFollowDistance) || transform.position.y > _startPosition.y + _maxHeightFollowFromInitPosition)
                    _isFollowing = false;
            }
        }

        private System.Collections.IEnumerator ChargeCoroutine()
        {
            if (_isCharging)
                yield break;

            _isCharging = true;

            _rigidbody2D.NullifyMovement();

            Vector3 chargeDirection = (_player.transform.position - transform.position).normalized;

            _followingEnemyView?.SetAngryFace();
            _followingEnemyView?.LookAt(chargeDirection);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_chargeDelay);

            Vector3 chargeStartPosition = transform.position;
            Vector3 chargeEndPosition = transform.position + chargeDirection * _chargeLength;

            for (float t = 0f; t <= 1f; t += Time.deltaTime / _chargeDuration)
            {
                Vector3 position = Vector3.LerpUnclamped(chargeStartPosition, chargeEndPosition, _chargeCurve.Evaluate(t));
                _rigidbody2D.MovePosition(position);
                yield return null;
            }

            _rigidbody2D.MovePosition(chargeEndPosition);
            _isCharging = false;

            _followingEnemyView?.SetNormalFace();

            _chargeOnCooldown = true;
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_chargeCooldown);
            _chargeOnCooldown = false;
        }

        private void Awake()
        {
            _startPosition = transform.position;
            _player = FindObjectOfType<PlayerController>();
        }

        private void Update()
        {
            UpdateFollowState();

            if (_isFollowing)
            {
                if (_isCharging)
                    return;

                if (IsPlayerInRange(_chargeDistance) && !_chargeOnCooldown)
                    StartCoroutine(ChargeCoroutine());
                else
                    _rigidbody2D.velocity = (_player.transform.position - transform.position).normalized * _baseSpeed;
            }
            else
            {
                if ((_startPosition - transform.position).sqrMagnitude > 0.3f * 0.3f)
                    _rigidbody2D.velocity = (_startPosition - transform.position).normalized * _baseSpeed;
                else
                    _rigidbody2D.NullifyMovement();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _detectionDistance);
            Gizmos.color = Gizmos.color.WithA(0.5f);
            Gizmos.DrawWireSphere(transform.position, _stopFollowDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _chargeDistance);

            Gizmos.color = Color.red;
            Gizmos.color = Gizmos.color.WithA(0.5f);
            Gizmos.DrawLine((UnityEditor.EditorApplication.isPlaying ? _startPosition : transform.position).AddY(_maxHeightFollowFromInitPosition).AddX(-3f),
                            (UnityEditor.EditorApplication.isPlaying ? _startPosition : transform.position).AddY(_maxHeightFollowFromInitPosition).AddX(3f));
        }
#endif
    }
}