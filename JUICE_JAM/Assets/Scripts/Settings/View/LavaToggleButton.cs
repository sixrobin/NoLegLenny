namespace JuiceJam.UI
{
    public class LavaToggleButton : OptionButton
    {
        private void RefreshText()
        {
            _buttonText.text = $"Lava: {(Settings.SettingsManager.LavaToggle.Value ? "On" : "Off")}";
        }

        private void OnButtonClicked()
        {
            Settings.SettingsManager.LavaToggle.Value = !Settings.SettingsManager.LavaToggle.Value;
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