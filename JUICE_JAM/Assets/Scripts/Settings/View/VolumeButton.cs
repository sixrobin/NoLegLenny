namespace JuiceJam.UI
{
    public class VolumeButton : OptionButton
    {
        private void RefreshText()
        {
            _buttonText.text = $"Volume: {UnityEngine.Mathf.RoundToInt(Settings.SettingsManager.Volume.Value * 100)}%";
        }

        private void OnButtonClicked()
        {
            if (Settings.SettingsManager.Volume.Value == Settings.SettingsManager.Volume.Range.Max)
                Settings.SettingsManager.Volume.Value = Settings.SettingsManager.Volume.Range.Min;
            else
                Settings.SettingsManager.Volume.Value += 0.1f;

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