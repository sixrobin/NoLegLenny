namespace JuiceJam.UI
{
    using UnityEngine;

    public class Timer : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _timerText = null;
        [SerializeField] private string _timerFormat = "mm':'ss'.'fff";

        private bool _moonReached;

        private void OnMoonFinalPositionReached()
        {
            _moonReached = true;
        }

        private void Awake()
        {
            Moon.MoonFinalPositionReached += OnMoonFinalPositionReached;
        }

        private void Update()
        {
            if (_moonReached
                || !Settings.SettingsManager.TimerDisplay.Value
                || DitherFade.IsFading
                || OptionsPanel.Instance.IsOpen)
            {
                _timerText.enabled = false;
            }
            else
            {
                _timerText.enabled = true;

                System.TimeSpan timerSpan = System.TimeSpan.FromSeconds(GameController.Instance.Timer);
                _timerText.text = timerSpan.ToString(_timerFormat);
            }
        }

        private void OnDestroy()
        {
            Moon.MoonFinalPositionReached -= OnMoonFinalPositionReached;
        }
    }
}