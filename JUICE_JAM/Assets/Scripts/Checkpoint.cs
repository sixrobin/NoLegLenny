namespace JuiceJam
{
    using UnityEngine;

    public class Checkpoint : MonoBehaviour, IRespawnable
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Transform _respawnPosition = null;
        [SerializeField] private RSLib.ImageEffects.SpriteBlink _spriteBlink = null;

        [SerializeField] private UnityEngine.Events.UnityEvent _onRespawn = null;

        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _checkClip = null;

        public static Checkpoint LastCheckpoint { get; private set; }

        public Vector3 RespawnPosition => _respawnPosition.position;

        [ContextMenu("Register")]
        public void Register()
        {
            LastCheckpoint = this;
            _spriteBlink.BlinkColor(1, () => _animator.SetTrigger("Raise"));
            RSLib.Audio.AudioManager.PlayNextPlaylistSound(_checkClip);
        }

        public void Respawn()
        {
            if (LastCheckpoint == this)
                _onRespawn?.Invoke();
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