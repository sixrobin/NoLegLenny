namespace JuiceJam
{
    using UnityEngine;

    public class Checkpoint : MonoBehaviour, IRespawnable
    {
        private static readonly int RAISE_ANIMATOR_HASH = Animator.StringToHash("Raise");
        private static readonly int OFF_ANIMATOR_HASH = Animator.StringToHash("Off");
        private static readonly int IDLE_SPEED_MULT_ANIMATOR_HASH = Animator.StringToHash("IdleSpeedMult");
        
        [Header("REFERENCES")]
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Transform _respawnPosition = null;
        [SerializeField] private RSLib.ImageEffects.SpriteBlink _spriteBlink = null;

        [Header("EVENTS")]
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
            if (LastCheckpoint != null)
                LastCheckpoint.Unregister();
            
            LastCheckpoint = this;
            _onRegister?.Invoke();

            _spriteBlink.BlinkColor(1, () => _animator.SetTrigger(RAISE_ANIMATOR_HASH));
            RSLib.Audio.AudioManager.PlayNextPlaylistSound(_checkClip);
        }

        [ContextMenu("Unregister")]
        public void Unregister()
        {
            _animator.SetTrigger(OFF_ANIMATOR_HASH);
        }

        public void Respawn()
        {
            if (LastCheckpoint == this)
                _onRespawn?.Invoke();
        }

        private void Awake()
        {
            LastCheckpoint = null;
            _animator.SetFloat(IDLE_SPEED_MULT_ANIMATOR_HASH, _idleSpeedMultiplier);
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
            _animator.SetFloat(IDLE_SPEED_MULT_ANIMATOR_HASH, _idleSpeedMultiplier);
        }
#endif
    }
}