namespace JuiceJam.UI
{
    public class PixelPerfectButton : OptionButton
    {
        private void RefreshText()
        {
            _buttonText.text = $"Pixel Perfect: {(Settings.SettingsManager.PixelPerfect.Value ? "On" : "Off")}";
        }

        private void OnButtonClicked()
        {
            Settings.SettingsManager.PixelPerfect.Value = !Settings.SettingsManager.PixelPerfect.Value;
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