namespace JuiceJam.UI
{
    using RSLib.Extensions;
    using UnityEngine;

    public class OptionsPanel : RSLib.Framework.Singleton<OptionsPanel>
    {
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton[] _buttons = null;
        [SerializeField] private Canvas _canvas = null;

        public static bool PausingCoroutineRunning { get; private set; }
        public static bool IsOpen { get; private set; }

        private void InitNavigation()
        {
            for (int i = 0; i < _buttons.Length; ++i)
            {
                _buttons[i].SetMode(UnityEngine.UI.Navigation.Mode.Explicit);
                _buttons[i].SetSelectOnDown(_buttons[RSLib.Helpers.Mod(i + 1, _buttons.Length)]);
                _buttons[i].SetSelectOnUp(_buttons[RSLib.Helpers.Mod(i - 1, _buttons.Length)]);
            }
        }

        private void Start()
        {
            InitNavigation();
            _canvas.enabled = false;
        }

        private void Update()
        {
            if (!PausingCoroutineRunning
                && !DitherFade.IsFading
                && Input.GetButtonDown("Pause"))
            {
                IsOpen = !IsOpen;
                StartCoroutine(TogglePauseCoroutine(IsOpen));
            }
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
            }
            else
            {
                _canvas.enabled = false;

                DitherFade.FadeOut(0.3f, RSLib.Maths.Curve.Linear);
                yield return new WaitUntil(() => !DitherFade.IsFading);
                yield return RSLib.Yield.SharedYields.WaitForSecondsRealtime(0.2f);

                TimeManager.SetTimeScale(1f);
            }

            PausingCoroutineRunning = false;
        }
    }
}