namespace JuiceJam
{
    using UnityEngine;

    public class ExplodingBarrel : MonoBehaviour, IDamageable
    {
        [Header("REFS")]
        [SerializeField] private Collider2D _collider2D = null;
        [SerializeField] private Animator _animator = null;

        [Header("EXPLOSION")]
        [SerializeField, Min(0f)] private float _explosionRadius = 3f;
        [SerializeField, Min(0f)] private float _explosionForce = 10f;
        [SerializeField, Min(0f)] private float _explosionForceMin = 3f;

        [Header("VFX")]
        [SerializeField] private ParticleSystem[] _explosionParticlesSystems = null;
        [SerializeField, Range(0f, 1f)] private float _destroyedTrauma = 0.4f;
        [SerializeField, Min(0f)] private float _freezeFrameDuration = 0f;
        [SerializeField, Min(0)] private int _freezeFrameDelay = 0;
        [SerializeField] private Transform _ripplePosition = null;

        public bool CanBeDamaged => true;
        public bool DontDestroyDamageSource => false;

        [ContextMenu("Explode")]
        public void Explode()
        {
            ExplosionData explosionData = new ExplosionData()
            {
                Force = _explosionForce,
                ForceMin = _explosionForceMin,
                Radius = _explosionRadius,
                Source = transform.position
            };

            Collider2D[] nearColliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
            for (int i = nearColliders.Length - 1; i >= 0; --i)
                if (nearColliders[i].TryGetComponent(out IExplodable explodable))
                    explodable.Explode(explosionData);

            FreezeFrameManager.FreezeFrame(_freezeFrameDelay, _freezeFrameDuration);
            CameraShake.AddTrauma(_destroyedTrauma);
            RSLib.ImageEffects.RippleEffect.RippleAtWorldPosition(_ripplePosition.position);

            for (int i = _explosionParticlesSystems.Length - 1; i >= 0; --i)
                _explosionParticlesSystems[i].Play();
        }

        public void TakeDamage(DamageData damageData)
        {
            _collider2D.enabled = false;
            _animator.SetTrigger("Explode");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}