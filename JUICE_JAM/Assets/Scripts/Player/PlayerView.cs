namespace JuiceJam
{
    using UnityEngine;

    public class PlayerView : MonoBehaviour
    {
        [Header("PLAYER")]
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private Animator _playerAnimator = null;
        [SerializeField] private SpriteRenderer _playerSpriteRenderer = null;

        [Header("SHOOT")]
        [SerializeField] private Animator _weaponAnimator = null;
        [SerializeField] private SpriteRenderer _weaponSpriteRenderer = null;
        [SerializeField] private ParticleSystem[] _shootParticlesSystems = null;
        [SerializeField, Min(0f)] private float _shootFreezeFrameDur = 0f;
        [SerializeField, Range(0f, 1f)] private float _shootTrauma = 0.1f;

        [Header("VFX")]
        [SerializeField] private GameObject _impulsePuffPrefab = null;
        [SerializeField] private Transform _impulsePuffPivot = null;
        [SerializeField] private float _landSidePuffMinSpeed = 1f;
        [SerializeField] private GameObject _landPuffPrefab = null;
        [SerializeField] private GameObject _landSidePuffPrefab = null;
        [SerializeField] private Transform _landPuffPivot = null;

        [Header("DAMAGE")]
        [SerializeField] private ParticleSystem[] _damageParticlesSystems = null;
        [SerializeField, Range(0f, 1f)] private float _damageTrauma = 0.2f;
        [SerializeField] private RSLib.ImageEffects.SpriteBlink[] _spritesToBlink = null;
        [SerializeField] private int _blinksCount = 3;
        [SerializeField, Min(0f)] private float _damageFreezeFrameDur = 0f;
        [SerializeField, Min(0)] private int _damageFreezeFrameDelay = 0;

        [Header("DEATH")]
        [SerializeField] private ParticleSystem[] _deathParticlesSystems = null;
        [SerializeField, Range(0f, 1f)] private float _deathTrauma = 0.2f;
        [SerializeField, Min(0f)] private float _deathFreezeFrameDur = 0f;
        [SerializeField, Min(0)] private int _deathFreezeFrameDelay = 0;

        public float ShootFreezeFrameDuration => _shootFreezeFrameDur;
        public float DamageFreezeFrameDuration => _damageFreezeFrameDur;
        public int DamageFreezeFrameDelay => _damageFreezeFrameDelay;
        public float DeathFreezeFrameDuration => _deathFreezeFrameDur;
        public int DeathFreezeFrameDelay => _deathFreezeFrameDelay;
        public float DeathTrauma => _deathTrauma;

        public void Respawn()
        {
            DisplayPlayer(true);
            _playerAnimator.SetTrigger("Respawn");
        }

        public void DisplayPlayer(bool show)
        {
            _playerSpriteRenderer.enabled = show;
            _weaponSpriteRenderer.enabled = show;
        }

        public void PlayImpulseAnimation(Vector2 shootDirection)
        {
            Instantiate(_impulsePuffPrefab, _impulsePuffPivot.position, _impulsePuffPrefab.transform.rotation);
        }

        public void PlayShootAnimation(Vector2 shootDirection)
        {
            _playerAnimator.SetTrigger("Shoot");
            _weaponAnimator.SetTrigger("Shoot");

            CameraShake.SetTrauma(_shootTrauma);

            if (!_playerController.IsClouded)
            {
                for (int i = _shootParticlesSystems.Length - 1; i >= 0; --i)
                    _shootParticlesSystems[i].Play();

                if (_playerController.GroundHit)
                {
                    GameObject sidePuff = Instantiate(_landSidePuffPrefab, _landPuffPivot.position, _landSidePuffPrefab.transform.rotation);
                    sidePuff.GetComponent<SpriteRenderer>().flipX = shootDirection.x < 0f;
                }
            }
        }

        public void PlayLandAnimation(Vector2 velocity)
        {
            if (_playerController.IsDead)
                return;

            _playerAnimator.SetTrigger("Land");

            if (Mathf.Abs(velocity.x) > _landSidePuffMinSpeed)
            {
                GameObject sidePuff = Instantiate(_landSidePuffPrefab, _landPuffPivot.position, _landSidePuffPrefab.transform.rotation);
                sidePuff.GetComponent<SpriteRenderer>().flipX = velocity.x < 0f;
            }
            else
            {
                Instantiate(_landPuffPrefab, _landPuffPivot.position, _landPuffPrefab.transform.rotation);
            }
        }

        public void PlayDamageAnimation(DamageData damageData)
        {
            CameraShake.SetTrauma(_damageTrauma);

            for (int i = _damageParticlesSystems.Length - 1; i >= 0; --i)
                _damageParticlesSystems[i].Play();

            for (int i = _spritesToBlink.Length - 1; i >= 0; --i)
                _spritesToBlink[i].BlinkColor(1, PlayInvulnerabilityAnimation);
        }

        public void PlayDeathAnimation()
        {
            _playerAnimator.ResetTrigger("Respawn");
            _playerAnimator.SetTrigger("Death");

            CameraShake.SetTrauma(_deathTrauma);

            for (int i = _deathParticlesSystems.Length - 1; i >= 0; --i)
                _deathParticlesSystems[i].Play();

            for (int i = _spritesToBlink.Length - 1; i >= 0; --i)
                _spritesToBlink[i].BlinkColor(1);
        }

        public void PlayInvulnerabilityAnimation()
        {
            for (int j = _spritesToBlink.Length - 1; j >= 0; --j)
                _spritesToBlink[j].BlinkAlpha(_blinksCount);
        }

        private void Update()
        {
            _playerAnimator.SetBool("Airborne", !_playerController.GroundHit);
        }
    }
}