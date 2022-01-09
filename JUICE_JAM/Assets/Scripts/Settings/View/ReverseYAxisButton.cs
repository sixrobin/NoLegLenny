namespace JuiceJam.UI
{
    public class ReverseYAxisButton : OptionButton
    {
        private void RefreshText()
        {
            _buttonText.text = $"Y Axis: {(Settings.SettingsManager.YAxisReverse.Value ? "Reversed" : "Normal")}";
        }

        private void OnButtonClicked()
        {
            Settings.SettingsManager.YAxisReverse.Value = !Settings.SettingsManager.YAxisReverse.Value;
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