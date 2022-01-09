namespace JuiceJam.UI
{
    public class ShakeAmountButton : OptionButton
    {
        private void RefreshText()
        {
            _buttonText.text = $"Shake Amount: {UnityEngine.Mathf.RoundToInt(Settings.SettingsManager.ShakeAmount.Value * 100)}%";
        }

        private void OnButtonClicked()
        {
            if (Settings.SettingsManager.ShakeAmount.Value == Settings.SettingsManager.ShakeAmount.Range.Max)
                Settings.SettingsManager.ShakeAmount.Value = Settings.SettingsManager.ShakeAmount.Range.Min;
            else
                Settings.SettingsManager.ShakeAmount.Value += 0.2f;

            RefreshText();
        }

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClicked);
            RefreshText();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _button.onClick.AddListener(OnButtonClicked);
        }
    }
}