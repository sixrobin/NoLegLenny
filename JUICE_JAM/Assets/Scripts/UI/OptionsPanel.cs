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
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _exitButton = null;

        public static bool PausingCoroutineRunning { get; private set; }
        public static bool IsOpen { get; private set; }

        private void OnResumeButtonClicked()
        {
            if (!IsOpen || PausingCoroutineRunning)
                return;

            TogglePause();
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
                _canvas.enabled = false;

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
            _exitButton.onClick.AddListener(OnExitButtonClicked);

            InitNavigation();
            _canvas.enabled = false;
        }

        private void Update()
        {
            if (!PausingCoroutineRunning
                && !DitherFade.IsFading
                && Input.GetButtonDown("Pause"))
            {
                TogglePause();
            }
        }

        private void OnDestroy()
        {
            _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            _exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }
}