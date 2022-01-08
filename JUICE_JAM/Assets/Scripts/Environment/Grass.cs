namespace JuiceJam
{
    using UnityEngine;

    public class Grass : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private float _minDistanceToMove = 4f;

        private void OnBulletHit(Bullet.BulletHitEventArgs args)
        {
            if ((args.Position - transform.position).sqrMagnitude > _minDistanceToMove * _minDistanceToMove
                || args.Collision2D.gameObject.layer != LayerMask.NameToLayer("Ground"))
                return;

            _animator.SetTrigger(transform.position.x > args.Position.x ? "Right" : "Left");
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