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

        [Header("VFX")]
        [SerializeField] private CollisionPrefabs[] _collisionPrefabs = null;

        public static event System.Action<BulletHitEventArgs> BulletHit;

        public void Launch(Vector3 direction)
        {
            transform.right = direction;
            _rigidbody2D.velocity = transform.right * _speed;
        }

        private void Update()
        {
            _bulletSpriteRenderer.transform.right = _rigidbody2D.velocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(new DamageData()
                {
                    Amount = _damage
                });
            }

            BulletHit?.Invoke(new BulletHitEventArgs(transform.position, collision));

            for (int i = _collisionPrefabs.Length -1; i >= 0; --i)
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

            Destroy(gameObject);
        }
    }
}