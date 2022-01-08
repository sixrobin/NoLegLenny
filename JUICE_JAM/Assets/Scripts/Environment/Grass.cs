namespace JuiceJam
{
    using UnityEngine;

    public class Grass : MonoBehaviour, IExplodable
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private float _minDistanceToMove = 4f;

        public void Explode(ExplosionData explosionData)
        {
            OnNearImpact(explosionData.Source);
        }

        private void OnNearImpact(Vector3 position)
        {
            if ((position - transform.position).sqrMagnitude <= _minDistanceToMove * _minDistanceToMove)
                _animator.SetTrigger(transform.position.x > position.x ? "Right" : "Left");
        }

        private void OnBulletHit(Bullet.BulletHitEventArgs args)
        {
            if (args.Collision2D.gameObject.layer != LayerMask.NameToLayer("Ground"))
                return;

            OnNearImpact(args.Position);
        }

        private void Awake()
        {
            Bullet.BulletHit += OnBulletHit;
        }

        private void OnDestroy()
        {
            Bullet.BulletHit -= OnBulletHit;
        }
    }
}