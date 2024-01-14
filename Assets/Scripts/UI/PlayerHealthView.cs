namespace JuiceJam.UI
{
    using UnityEngine;

    public class PlayerHealthView : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private RSLib.Dynamics.DynamicInt _playerHealth = null;
        [SerializeField] private PlayerHealthHeartView[] _hearts = null;
        
        private bool _moonReached;

        private void OnPlayerHealthValueChanged(RSLib.Dynamics.DynamicInt.ValueChangedEventArgs args)
        {
            for (int i = 0; i < _hearts.Length; ++i)
                _hearts[i].Toggle(i < args.New);
        }

        private void OnDitherFadeBegan(bool fadeIn)
        {
            _canvas.enabled = false;
        }

        private void OnDitherFadeOver(bool fadeIn)
        {
            if (!fadeIn && !_moonReached)
                _canvas.enabled = true;
        }

        private void OnMoonFinalPositionReached()
        {
            _moonReached = true;
            _canvas.enabled = false;
        }

        private void Awake()
        {
            _playerHealth.ValueChanged += OnPlayerHealthValueChanged;
            DitherFade.FadeBegan += OnDitherFadeBegan;
            DitherFade.FadeOver += OnDitherFadeOver;
            Moon.MoonFinalPositionReached += OnMoonFinalPositionReached;
        }

        private void OnDestroy()
        {
            _playerHealth.ValueChanged -= OnPlayerHealthValueChanged;
            DitherFade.FadeBegan -= OnDitherFadeBegan;
            DitherFade.FadeOver -= OnDitherFadeOver;
            Moon.MoonFinalPositionReached -= OnMoonFinalPositionReached;
        }
    }
}