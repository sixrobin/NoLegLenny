namespace JuiceJam
{
    using UnityEngine;

    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Transform _respawnPosition = null;
        [SerializeField] private RSLib.ImageEffects.SpriteBlink _spriteBlink = null;

        public static Checkpoint LastCheckpoint { get; private set; }

        public Vector3 RespawnPosition => _respawnPosition.position;

        [ContextMenu("Register")]
        public void Register()
        {
            LastCheckpoint = this;
            _spriteBlink.BlinkColor(1, () => _animator.SetTrigger("Raise"));
        }

        private void Awake()
        {
            LastCheckpoint = null;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (LastCheckpoint == this)
                return;

            if (collision.gameObject.TryGetComponent<PlayerController>(out _))
                Register();
        }
    }
}