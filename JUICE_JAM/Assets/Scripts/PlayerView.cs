namespace JuiceJam
{
    using UnityEngine;

    public class PlayerView : MonoBehaviour
    {
        [Header("PLAYER")]
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private Animator _playerAnimator = null;

        [Header("SHOOT")]
        [SerializeField] private Animator _weaponAnimator = null;
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

        [Header("HEALTH")]
        [SerializeField] private ParticleSystem[] _damageParticlesSystems = null;
        [SerializeField, Range(0f, 1f)] private float _damageTrauma = 0.2f;
        [SerializeField] private RSLib.ImageEffects.SpriteBlink[] _spritesToBlink = null;
        [SerializeField] private int _blinksCount = 3;

        public float ShootFreezeFrameDuration => _shootFreezeFrameDur;

        public void PlayImpulseAnimation(Vector2 shootDirection)
        {
            Instantiate(_impulsePuffPrefab, _impulsePuffPivot.position, _impulsePuffPrefab.transform.rotation);
        }

        public void PlayShootAnimation(Vector2 shootDirection)
        {
            _playerAnimator.SetTrigger("Shoot");
            _weaponAnimator.SetTrigger("Shoot");

            CameraShake.SetTrauma(_shootTrauma);

            for (int i = _shootParticlesSystems.Length - 1; i >= 0; --i)
                _shootParticlesSystems[i].Play();

            if (_playerController.IsGrounded)
            {
                GameObject sidePuff = Instantiate(_landSidePuffPrefab, _landPuffPivot.position, _landSidePuffPrefab.transform.rotation);
                sidePuff.GetComponent<SpriteRenderer>().flipX = shootDirection.x < 0f;
            }
        }

        public void PlayLandAnimation(Vector2 velocity)
        {
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

        public void PlayInvulnerabilityAnimation()
        {
            for (int j = _spritesToBlink.Length - 1; j >= 0; --j)
                _spritesToBlink[j].BlinkAlpha(_blinksCount);
        }
    }
}