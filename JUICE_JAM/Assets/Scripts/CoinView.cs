namespace JuiceJam
{
    using UnityEngine;

    public class CoinView : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;

        private void Start()
        {
            _animator.Play("Idle", 0, Random.Range(0f, 1f));
        }
    }
}