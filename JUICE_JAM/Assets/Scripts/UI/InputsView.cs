namespace JuiceJam
{
    using UnityEngine;

    public class InputsView : MonoBehaviour
    {
        public static bool s_seenOnce;
        public static bool s_firstInputDoneOnce;

        [SerializeField] private PlayerController _playerController = null;

        private void OnPlayerFirstMovementInput()
        {
            s_firstInputDoneOnce = true;

            gameObject.SetActive(false);
        }

        private void OnDitherFadeBegan(bool fadeIn)
        {
            if (fadeIn)
                gameObject.SetActive(false);
        }

        private void OnDitherFadeOver(bool fadeIn)
        {
            Debug.Log("oui");
            if (!fadeIn)
                gameObject.SetActive(!s_firstInputDoneOnce);
        }

        private void UnregisterEvents()
        {
            _playerController.FirstMovementInput -= OnPlayerFirstMovementInput;
            DitherFade.FadeBegan -= OnDitherFadeBegan;
            DitherFade.FadeOver -= OnDitherFadeOver;
        }

        private void Start()
        {
            _playerController.FirstMovementInput += OnPlayerFirstMovementInput;
            DitherFade.FadeBegan += OnDitherFadeBegan;
            DitherFade.FadeOver += OnDitherFadeOver;

            if (s_seenOnce && s_firstInputDoneOnce)
                UnregisterEvents();

            gameObject.SetActive(false);
            s_seenOnce = true;
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }
    }
}