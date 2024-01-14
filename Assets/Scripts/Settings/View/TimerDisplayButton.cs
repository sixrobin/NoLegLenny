namespace JuiceJam.UI
{
    public class TimerDisplayButton : OptionButton
    {
        private void RefreshText()
        {
            _buttonText.text = $"Display Timer: {(Settings.SettingsManager.TimerDisplay.Value ? "On" : "Off")}";
        }

        private void OnButtonClicked()
        {
            Settings.SettingsManager.TimerDisplay.Value = !Settings.SettingsManager.TimerDisplay.Value;
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