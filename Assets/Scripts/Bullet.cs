namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class Bullet : MonoBehaviour
    {
        [System.Serializable]
        private struct CollisionPrefabs
        {
            public LayerMask Layer;
            public GameObject[] BulletPositionPrefabs;
            public GameObject[] CollisionPositionPrefabs;
        }

        public struct BulletHitEventArgs
        {
            public BulletHitEventArgs(Vector3 position, Collision2D collision2D)
            {
                Position = position;
                Collision2D = collision2D;
            }

            public Vector3 Position;
            public Collision2D Collision2D;
        }

        [SerializeField] private Rigidbody2D _rigidbody2D = null;
        [SerializeField] private SpriteRenderer _bulletSpriteRenderer = null;
        [SerializeField] private float _speed = 10f;
        [SerializeField, Min(0)] private int _damage = 1;
        [SerializeField] private bool _goThroughIfNotDamageable = true;

        [Header("FEEDBACK")]
        [SerializeField] private CollisionPrefabs[] _collisionPrefabs = null;
        [SerializeField] private RSLib.Audio.ClipProvider _hitClip = null;

        private Vector2 _direction;
        private bool _keepDirection;

        public static event System.Action<BulletHitEventArgs> BulletHit;

        public object Source { get; private set; }

        public void Launch(Vector3 direction, bool keepDirection, object source)
        {
            _direction = direction.normalized;
            _keepDirection = keepDirection;
            Source = source;

            transform.right = _direction;
            _rigidbody2D.velocity = transform.right * _speed;
        }

        private void Update()
        {
            _bulletSpriteRenderer.transform.right = _rigidbody2D.velocity;
        }

        private void FixedUpdate()
        {
            if (_keepDirection)
            {
                transform.right = _direction;
                _rigidbody2D.velocity = transform.right * _speed;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!collider.TryGetComponent(out FollowingEnemy enemy))
                return;

            if (enemy.CanBeDamaged)
            {
                enemy.TakeDamage(new DamageData()
                {
                    Source = this,
                    Amount = _damage,
                    HitPoint = transform.position,
                    HitDirection = _direction
                });
            }

            for (int i = _collisionPrefabs.Length - 1; i >= 0; --i)
            {
                if (!_collisionPrefabs[i].Layer.HasLayer(collider.gameObject.layer))
                    continue;

                for (int j = _collisionPrefabs[i].BulletPositionPrefabs.Length - 1; j >= 0; --j)
                    Instantiate(_collisionPrefabs[i].BulletPositionPrefabs[j], transform.position, _collisionPrefabs[i].BulletPositionPrefabs[j].transform.rotation);

                for (int j = _collisionPrefabs[i].CollisionPositionPrefabs.Length - 1; j >= 0; --j)
                    Instantiate(_collisionPrefabs[i].CollisionPositionPrefabs[j], transform.position, _collisionPrefabs[i].CollisionPositionPrefabs[j].transform.rotation);
            }

            if (_bulletSpriteRenderer.isVisible)
                RSLib.Audio.AudioManager.PlayNextPlaylistSound(_hitClip);

            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            bool dontDestroy = false;

            BulletHit?.Invoke(new BulletHitEventArgs(transform.position, collision));

            if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                if (damageable == Source)
                    return;

                if (damageable.CanBeDamaged)
                {
                    damageable.TakeDamage(new DamageData()
                    {
                        Source = this,
                        Amount = _damage,
                        HitPoint = collision.contacts[0].point,
                        HitDirection = _direction
                    });
                }

                dontDestroy = damageable.DontDestroyDamageSource || !damageable.CanBeDamaged && _goThroughIfNotDamageable;
            }

            if (dontDestroy)
                return;

            for (int i = _collisionPrefabs.Length - 1; i >= 0; --i)
            {
                if (!_collisionPrefabs[i].Layer.HasLayer(collision.gameObject.layer))
                {
                    continue;
                }

                for (int j = _collisionPrefabs[i].BulletPositionPrefabs.Length - 1; j >= 0; --j)
                    Instantiate(_collisionPrefabs[i].BulletPositionPrefabs[j], transform.position, _collisionPrefabs[i].BulletPositionPrefabs[j].transform.rotation);

                for (int j = _collisionPrefabs[i].CollisionPositionPrefabs.Length - 1; j >= 0; --j)
                    Instantiate(_collisionPrefabs[i].CollisionPositionPrefabs[j], collision.contacts[0].point, _collisionPrefabs[i].CollisionPositionPrefabs[j].transform.rotation);
            }

            if (_bulletSpriteRenderer.isVisible)
                RSLib.Audio.AudioManager.PlayNextPlaylistSound(_hitClip);

            Destroy(gameObject);
        }
    }
}