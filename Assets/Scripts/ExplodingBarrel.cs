namespace JuiceJam
{
    using UnityEngine;

    public class ExplodingBarrel : MonoBehaviour, IDamageable, IExplodable, IRespawnable
    {
        [Header("REFS")]
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private Collider2D _collider2D = null;
        [SerializeField] private Animator _animator = null;

        [Header("EXPLOSION")]
        [SerializeField, Min(0f)] private float _explosionRadius = 3f;
        [SerializeField, Min(0f)] private float _barrelChainRadius = 2.5f;
        [SerializeField, Min(0f)] private float _explosionForce = 10f;
        [SerializeField, Min(0f)] private float _explosionForceMin = 3f;

        [Header("VFX")]
        [SerializeField] private ParticleSystem[] _explosionParticlesSystems = null;
        [SerializeField, Range(0f, 1f)] private float _destroyedTrauma = 0.4f;
        [SerializeField, Min(0f)] private float _freezeFrameDuration = 0f;
        [SerializeField, Min(0)] private int _freezeFrameDelay = 0;
        [SerializeField] private Transform _ripplePosition = null;
        [SerializeField] private float _barrelChainDelay = 0.2f;
        [SerializeField] private bool _hideAfterExplosion = false;

        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _explosionClip = null;

        public bool CanBeDamaged => true;
        public bool DontDestroyDamageSource => false;

        public void TakeDamage(DamageData damageData)
        {
            _collider2D.enabled = false;
            _animator.SetTrigger("Explode");
        }

        public void Explode(ExplosionData explosionData)
        {
            if (!_collider2D.enabled)
                return;

            StartCoroutine(ChainBarrelExplosion());
        }

        public void Respawn()
        {
            StopAllCoroutines();

            _spriteRenderer.enabled = true;
            if (!_collider2D.enabled)
            {
                _collider2D.enabled = true;
                _animator.SetTrigger("Respawn");
            }
        }

        [ContextMenu("Explode")]
        public void OnExplosionFrame()
        {
            ExplosionData explosionData = new ExplosionData()
            {
                Force = _explosionForce,
                ForceMin = _explosionForceMin,
                Radius = _explosionRadius,
                Source = transform.position
            };

            ExplosionData barrelChainExplosionData = new ExplosionData()
            {
                Force = _explosionForce,
                ForceMin = _explosionForceMin,
                Radius = _barrelChainRadius,
                Source = transform.position
            };

            Collider2D[] nearColliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);

            for (int i = nearColliders.Length - 1; i >= 0; --i)
            {
                if (nearColliders[i].TryGetComponent(out IExplodable explodable))
                {
                    if (explodable is ExplodingBarrel)
                    {
                        if ((nearColliders[i].transform.position - transform.position).sqrMagnitude < _barrelChainRadius * _barrelChainRadius)
                            explodable.Explode(barrelChainExplosionData);
                    }
                    else
                    {
                        explodable.Explode(explosionData);
                    }
                }
            }

            TimeManager.FreezeFrame(_freezeFrameDelay, _freezeFrameDuration);
            CameraShake.AddTrauma(_destroyedTrauma);
            RSLib.ImageEffects.RippleEffect.RippleAtWorldPosition(_ripplePosition.position);

            for (int i = _explosionParticlesSystems.Length - 1; i >= 0; --i)
                _explosionParticlesSystems[i].Play();

            RSLib.Audio.AudioManager.PlayNextPlaylistSound(_explosionClip);
        }

        public void OnExplodedFrame()
        {
            _spriteRenderer.enabled = !_hideAfterExplosion;
        }

        private System.Collections.IEnumerator ChainBarrelExplosion()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_barrelChainDelay);
            TakeDamage(new DamageData() { Source = this });
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, 0.75f);
            Gizmos.DrawWireSphere(transform.position, _barrelChainRadius);
        }
    }
}