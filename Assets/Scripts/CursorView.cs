namespace JuiceJam
{
    using UnityEngine;

    public class CursorView : RSLib.Framework.Singleton<CursorView>
    {
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private SpriteRenderer[] _cursorsSpriteRenderers = null;

        private void OnConstrainCursorValueChanged(bool currentValue)
        {
            Cursor.lockState = currentValue ? CursorLockMode.Confined : CursorLockMode.None;
        }

        private void Start()
        {
            Settings.SettingsManager.ConstrainCursor.ValueChanged += OnConstrainCursorValueChanged;
            OnConstrainCursorValueChanged(Settings.SettingsManager.ConstrainCursor.Value);
        }

        private void Update()
        {
            Cursor.visible = false;

            for (int i = _cursorsSpriteRenderers.Length - 1; i >= 0; --i)
            {
                _cursorsSpriteRenderers[i].enabled = _playerController.LastControllerType == PlayerController.ControllerType.Mouse
                                                     || (UI.OptionsPanel.Instance.IsOpen && !UI.OptionsPanel.Instance.PausingCoroutineRunning);

            }

            if (_playerController.LastControllerType == PlayerController.ControllerType.Mouse || UI.OptionsPanel.Instance.IsOpen)
            {
                Vector3 mouseWorldPosition = MainCamera.Camera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0f;
                transform.position = mouseWorldPosition;
            }
        }

        private void OnDestroy()
        {
            Settings.SettingsManager.ConstrainCursor.ValueChanged -= OnConstrainCursorValueChanged;
        }
    }
}
