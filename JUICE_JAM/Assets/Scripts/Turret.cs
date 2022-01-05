namespace JuiceJam
{
    using UnityEngine;

    public class Turret : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Bullet _bulletPrefab = null;
        [SerializeField] private Transform _bulletSpawnPosition = null;
        [SerializeField] private ParticleSystem[] _shootParticlesSystems = null;
        [SerializeField] private float _shootRate = 1f;
        [SerializeField] private float _initDelay = 0f;

        private float _shootTimer;

        public void Shoot()
        {
            Bullet bullet = Instantiate(_bulletPrefab, _bulletSpawnPosition.position, Quaternion.identity);
            bullet.Launch(-_bulletSpawnPosition.right * transform.localScale.x);

            for (int i = _shootParticlesSystems.Length - 1; i >= 0; --i)
                _shootParticlesSystems[i].Play();
        }

        private void Start()
        {
            _shootTimer = -_initDelay;
        }

        private void Update()
        {
            _shootTimer += Time.deltaTime;
            if (_shootTimer >= _shootRate)
            {
                _animator.SetTrigger("Shoot");
                _shootTimer = 0f;
            }
        }
    }
}