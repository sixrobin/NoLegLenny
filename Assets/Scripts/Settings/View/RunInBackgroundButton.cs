namespace JuiceJam.UI
{
    public class RunInBackgroundButton : OptionButton
    {
        private void RefreshText()
        {
            _buttonText.text = $"Run in background: {Settings.SettingsManager.RunInBackground.Value}";
        }

        private void OnButtonClicked()
        {
            Settings.SettingsManager.RunInBackground.Value = !Settings.SettingsManager.RunInBackground.Value;
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