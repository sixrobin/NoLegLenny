namespace JuiceJam
{
    using UnityEngine;

    public class Checkpoint : MonoBehaviour, IRespawnable
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Transform _respawnPosition = null;
        [SerializeField] private RSLib.ImageEffects.SpriteBlink _spriteBlink = null;

        [SerializeField] private UnityEngine.Events.UnityEvent _onRegister = null;
        [SerializeField] private UnityEngine.Events.UnityEvent _onRespawn = null;

        [Header("ANIMATION")]
        [SerializeField, Min(0f)] private float _idleSpeedMultiplier = 1f;

        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _checkClip = null;

        public static Checkpoint LastCheckpoint { get; private set; }

        public bool IsPlayerOnCheckpoint { get; private set; }

        public Vector3 RespawnPosition => _respawnPosition.position;

        [ContextMenu("Register")]
        public void Register()
        {
            LastCheckpoint?.Unregister();
            LastCheckpoint = this;

            _spriteBlink.BlinkColor(1, () => _animator.SetTrigger("Raise"));
            RSLib.Audio.AudioManager.PlayNextPlaylistSound(_checkClip);

            _onRegister?.Invoke();
        }

        [ContextMenu("Unregister")]
        public void Unregister()
        {
            _animator.SetTrigger("Off");
        }

        public void Respawn()
        {
            if (LastCheckpoint == this)
                _onRespawn?.Invoke();
        }

        private void Awake()
        {
            LastCheckpoint = null;
            _animator.SetFloat("IdleSpeedMult", _idleSpeedMultiplier);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out _))
            {
                if (LastCheckpoint != this)
                    Register();

                IsPlayerOnCheckpoint = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (LastCheckpoint != this)
                return;

            if (collision.gameObject.TryGetComponent<PlayerController>(out _))
                IsPlayerOnCheckpoint = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _animator.SetFloat("IdleSpeedMult", _idleSpeedMultiplier);
        }
#endif
    }
}