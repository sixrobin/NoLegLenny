namespace JuiceJam.UI
{
    public class ReverseYAxisButton : OptionButton
    {
        private void RefreshText()
        {
            _buttonText.text = $"Axis: {(Settings.SettingsManager.AxisReverse.Value ? "Reversed" : "Normal")}";
        }

        private void OnButtonClicked()
        {
            Settings.SettingsManager.AxisReverse.Value = !Settings.SettingsManager.AxisReverse.Value;
            RefreshText();
            RSLib.Audio.AudioManager.PlayNextPlaylistSound(RSLib.Audio.AudioManager.Instance.UIClickClip);
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