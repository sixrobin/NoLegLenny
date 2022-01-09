namespace JuiceJam.UI
{
    public class ConstrainCursorButton : OptionButton
    {
        private void RefreshText()
        {
            _buttonText.text = $"Constrain Cursor: {(Settings.SettingsManager.ConstrainCursor.Value ? "On" : "Off")}";
        }

        private void OnButtonClicked()
        {
            Settings.SettingsManager.ConstrainCursor.Value = !Settings.SettingsManager.ConstrainCursor.Value;
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