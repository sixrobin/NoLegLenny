namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CameraController : MonoBehaviour, IRespawnable
    {
        [SerializeField] private Transform _target = null;
        [SerializeField, Min(1f)] private float _lerpSpeed = 1f;
        [SerializeField] private Vector2 _maxOffset = new Vector2(0.6f, 3f);
        [SerializeField] private float _targetHeightOffset = 0f;

        private float _highestPositionReached;

        public void Respawn()
        {
            transform.position = _target.position.WithZ(transform.position.z);
        }

        private void Start()
        {
            Respawn();
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = _target.position;
            targetPosition.y += _targetHeightOffset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _lerpSpeed);

            if (Mathf.Abs(transform.position.x) > _maxOffset.x)
                transform.SetPositionX(_maxOffset.x * Mathf.Sign(transform.position.x));

            if (Mathf.Abs(_target.position.y - transform.position.y) > _maxOffset.y)
                transform.SetPositionY(_target.position.y - _maxOffset.y * Mathf.Sign(_target.position.y - transform.position.y));

            if (transform.position.y > _highestPositionReached)
                _highestPositionReached = transform.position.y;

            transform.SetPositionZ(-10f);
        }
    }
}