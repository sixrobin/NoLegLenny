namespace JuiceJam.UI
{
    using UnityEngine;

    public class CollectedCoinsText : MonoBehaviour
    {
        [SerializeField] private RSLib.Dynamics.DynamicInt _collectedCoins = null;
        [SerializeField] private TMPro.TextMeshProUGUI _coinsText = null;

        private void OnCoinsCollectedValueChanged(RSLib.Dynamics.DynamicInt.ValueChangedEventArgs args)
        {
            _coinsText.text = args.New.ToString();
        }

        private void Awake()
        {
            _collectedCoins.ValueChanged += OnCoinsCollectedValueChanged;
            _coinsText.text = "0";
        }

        private void OnDestroy()
        {
            _collectedCoins.ValueChanged -= OnCoinsCollectedValueChanged;
        }
    }
}