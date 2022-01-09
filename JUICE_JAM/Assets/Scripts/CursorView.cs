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

            Cursor.visible = false;
        }

        private void Update()
        {
            for (int i = _cursorsSpriteRenderers.Length - 1; i >= 0; --i)
                _cursorsSpriteRenderers[i].enabled = _playerController.LastControllerType == PlayerController.ControllerType.Mouse;

            if (_playerController.LastControllerType == PlayerController.ControllerType.Mouse)
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
