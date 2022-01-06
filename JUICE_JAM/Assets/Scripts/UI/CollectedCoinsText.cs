namespace JuiceJam.UI
{
    using UnityEngine;

    public class CollectedCoinsText : MonoBehaviour
    {
        [SerializeField] private RSLib.Dynamics.DynamicInt _collectedCoins = null;
        [SerializeField] private TMPro.TextMeshProUGUI _coinsText = null;
        [SerializeField] private UnityEngine.UI.Image _coinImage = null;

        private void OnDitherFadeBegan(bool fadeIn)
        {
            _coinImage.enabled = false;
            _coinsText.enabled = false;
        }

        private void OnDitherFadeOver(bool fadeIn)
        {
            if (!fadeIn)
            {
                _coinImage.enabled = true;
                _coinsText.enabled = true;
            }
        }

        private void OnCoinsCollectedValueChanged(RSLib.Dynamics.DynamicInt.ValueChangedEventArgs args)
        {
            _coinsText.text = args.New.ToString();
        }

        private void Awake()
        {
            _collectedCoins.ValueChanged += OnCoinsCollectedValueChanged;
            DitherFade.FadeBegan += OnDitherFadeBegan;
            DitherFade.FadeOver += OnDitherFadeOver;

            _coinsText.text = "0";
        }

        private void OnDestroy()
        {
            _collectedCoins.ValueChanged -= OnCoinsCollectedValueChanged;
            DitherFade.FadeBegan -= OnDitherFadeBegan;
            DitherFade.FadeOver -= OnDitherFadeOver;
        }
    }
}