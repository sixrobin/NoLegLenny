namespace JuiceJam
{
    using UnityEngine;

    public class CoinCollector : MonoBehaviour
    {
        [SerializeField] private RSLib.Dynamics.DynamicInt _coinsCollected = null;
        [SerializeField] private GameObject[] _coinCollectedParticles = null;

        private void Start()
        {
            _coinsCollected.Value = 0;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            _coinsCollected.Value++;
            Destroy(collision.gameObject);

            for (int i = _coinCollectedParticles.Length - 1; i >= 0; --i)
                Instantiate(_coinCollectedParticles[i], collision.gameObject.transform.position, _coinCollectedParticles[i].transform.rotation);
        }
    }
}