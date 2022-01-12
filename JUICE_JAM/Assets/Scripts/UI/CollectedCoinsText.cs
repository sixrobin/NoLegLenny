namespace JuiceJam.UI
{
    using UnityEngine;

    public class CollectedCoinsText : MonoBehaviour
    {
        [SerializeField] private RSLib.Dynamics.DynamicInt _collectedCoins = null;
        [SerializeField] private System.Collections.Generic.List<TMPro.TextMeshProUGUI> _coinsTexts = null;
        [SerializeField] private UnityEngine.UI.Image _coinImage = null;

        private bool _moonReached;

        private void OnDitherFadeBegan(bool fadeIn)
        {
            _coinImage.enabled = false;
            _coinsTexts.ForEach(o => o.enabled = false);
        }

        private void OnDitherFadeOver(bool fadeIn)
        {
            if (!fadeIn && !_moonReached)
            {
                _coinImage.enabled = true;
                _coinsTexts.ForEach(o => o.enabled = true);
            }
        }

        private void OnCoinsCollectedValueChanged(RSLib.Dynamics.DynamicInt.ValueChangedEventArgs args)
        {
            _coinsTexts.ForEach(o => o.text = args.New.ToString());
        }

        private void OnMoonFinalPositionReached()
        {
            _moonReached = true;
            _coinImage.enabled = false;
            _coinsTexts.ForEach(o => o.enabled = false);
        }

        private void Awake()
        {
            _collectedCoins.ValueChanged += OnCoinsCollectedValueChanged;
            DitherFade.FadeBegan += OnDitherFadeBegan;
            DitherFade.FadeOver += OnDitherFadeOver;
            Moon.MoonFinalPositionReached += OnMoonFinalPositionReached;

            OnCoinsCollectedValueChanged(new RSLib.Dynamics.DynamicInt.ValueChangedEventArgs() { New = 0 });
        }

        private void OnDestroy()
        {
            _collectedCoins.ValueChanged -= OnCoinsCollectedValueChanged;
            DitherFade.FadeBegan -= OnDitherFadeBegan;
            DitherFade.FadeOver -= OnDitherFadeOver;
            Moon.MoonFinalPositionReached -= OnMoonFinalPositionReached;
        }
    }
}