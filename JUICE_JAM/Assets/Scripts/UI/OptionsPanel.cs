namespace JuiceJam.UI
{
    using RSLib.Extensions;
    using UnityEngine;

    public class OptionsPanel : RSLib.Framework.Singleton<OptionsPanel>
    {
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton[] _buttons = null;
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private float _resumeDelay = 0.4f;

        [Header("BUTTONS SPECS")]
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _resumeButton = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _resetButton = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _settingsButton = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _exitButton = null;

        [Header("SETTINGS")]
        [SerializeField] private Canvas _settingsCanvas = null;
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton[] _settings = null;

        public bool PausingCoroutineRunning { get; private set; }
        public bool IsOpen { get; private set; }

        private void OnResumeButtonClicked()
        {
            if (!IsOpen || PausingCoroutineRunning)
                return;

            TogglePause();
        }

        private void OnResetButtonClicked()
        {
            RSLib.Audio.AudioManager.StopMusic();
            GameController.ResetGame();
        }

        private void OnSettingsButtonClicked()
        {
            _canvas.enabled = false;
            _settingsCanvas.enabled = true;

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_settings[0].gameObject);
        }

        private void OnExitButtonClicked()
        {
            if (!IsOpen || PausingCoroutineRunning)
                return;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        private void InitNavigation()
        {
            for (int i = 0; i < _buttons.Length; ++i)
            {
                _buttons[i].SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                _buttons[i].SetSelectOnDown(_buttons[RSLib.Helpers.Mod(i + 1, _buttons.Length)]);
                _buttons[i].SetSelectOnUp(_buttons[RSLib.Helpers.Mod(i - 1, _buttons.Length)]);
            }

            for (int i = 0; i < _settings.Length; ++i)
            {
                _settings[i].SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                _settings[i].SetSelectOnDown(_settings[RSLib.Helpers.Mod(i + 1, _settings.Length)]);
                _settings[i].SetSelectOnUp(_settings[RSLib.Helpers.Mod(i - 1, _settings.Length)]);
            }
        }

        private void TogglePause()
        {
            IsOpen = !IsOpen;
            StartCoroutine(TogglePauseCoroutine(IsOpen));
        }

        private System.Collections.IEnumerator TogglePauseCoroutine(bool pausing)
        {
            PausingCoroutineRunning = true;

            if (pausing)
            {
                TimeManager.SetTimeScale(0f);

                DitherFade.FadeIn(0.3f, RSLib.Maths.Curve.Linear);
                yield return new WaitUntil(() => !DitherFade.IsFading);
                yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(0.2f);

                _canvas.enabled = true;
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_buttons[0].gameObject);
            }
            else
            {
                Settings.SettingsManager.SaveToPlayerPrefs();
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

                _canvas.enabled = false;
                _settingsCanvas.enabled = false;

                DitherFade.FadeOut(0.3f, RSLib.Maths.Curve.Linear);
                yield return new WaitUntil(() => !DitherFade.IsFading);
                yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(_resumeDelay);

                TimeManager.SetTimeScale(1f);
            }

            PausingCoroutineRunning = false;
        }

        private void Start()
        {
            _resumeButton.onClick.AddListener(OnResumeButtonClicked);
            _resetButton.onClick.AddListener(OnResetButtonClicked);
            _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            _exitButton.onClick.AddListener(OnExitButtonClicked);

            InitNavigation();

            _canvas.enabled = false;
        }

        private void Update()
        {
            if (PausingCoroutineRunning || DitherFade.IsFading)
                return;

            if (Input.GetButtonDown("Pause"))
            {
                TogglePause();
            }
            else if (IsOpen && (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetMouseButtonDown(1)))
            {
                if (_settingsCanvas.enabled)
                {
                    _settingsCanvas.enabled = false;
                    _canvas.enabled = true;

                    if (!Input.GetMouseButtonDown(1))
                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_settingsButton.gameObject);
                }
                else
                {
                    TogglePause();
                }
            }
        }

        private void OnDestroy()
        {
            _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            _resetButton.onClick.RemoveListener(OnResetButtonClicked);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            _exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }
}