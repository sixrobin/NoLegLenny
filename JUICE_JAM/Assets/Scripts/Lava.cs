namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class Lava : MonoBehaviour
    {
        [SerializeField] private Transform _minHeightReference = null;
        [SerializeField, Min(0.1f)] private float _speed = 1f;
        [SerializeField, Min(1f)] private float _maxHeightOffset = 15f;

        private void Update()
        {
            transform.Translate(0f, _speed * Time.deltaTime, 0f, Space.World);
            if (_minHeightReference.position.y - transform.position.y > _maxHeightOffset)
                transform.SetPositionY(_minHeightReference.position.y - _maxHeightOffset);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out PlayerController playerController))
                playerController.TakeDamage(new DamageData() { Source = this, Amount = int.MaxValue });
        }
    }
}