namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class Moon : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Transform _player = null;
        [SerializeField] private Transform _playerReferenceA = null;
        [SerializeField] private Transform _playerReferenceB = null;
        [SerializeField] private Transform _moonPositionA = null;
        [SerializeField] private Transform _moonPositionB = null;

        [SerializeField] private float _landAnimationDelay = 1.5f;
        [SerializeField] private float _landedTrauma = 0.15f;

        private bool _reached;

        public static event System.Action MoonFinalPositionReached;

        public void OnLandedFrame()
        {
            CameraShake.SetTrauma(_landedTrauma);
        }

        public void OnLandAnimationOver()
        {
            DitherFade.FadeIn(0.5f, RSLib.Maths.Curve.Linear, callback: GameController.DisplayStatistics);
        }

        private System.Collections.IEnumerator PlayLandAnimationCoroutine()
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_landAnimationDelay);
            _animator.SetTrigger("Land");
        }

        private void Update()
        {
            if (_reached)
                return;

            float targetY = RSLib.Maths.Maths.Normalize(
                _player.position.y,
                _playerReferenceA.position.y,
                _playerReferenceB.position.y,
                _moonPositionA.position.y,
                _moonPositionB.position.y);

            targetY = Mathf.Clamp(targetY, _moonPositionB.position.y, _moonPositionA.position.y);

            if (targetY == _moonPositionB.position.y)
            {
                MoonFinalPositionReached?.Invoke();
                StartCoroutine(PlayLandAnimationCoroutine());
                _reached = true;
            }

            transform.SetPositionY(targetY);
        }
    }
}