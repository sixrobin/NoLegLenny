namespace JuiceJam.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class OptionButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _button = null;
        [SerializeField] private System.Collections.Generic.List<GameObject> _selectionArrows = null;
        [SerializeField] private TMPro.TextMeshProUGUI _buttonText = null;
        [SerializeField] private Color _selectedColor = Color.white;

        private Color _baseColor;

        public static event System.Action<OptionButton> ButtonHovered;

        public void OnSelect()
        {
            OnSelect(null);
        }

        public void OnSelect(BaseEventData eventData)
        {
            _selectionArrows.ForEach(o => o.SetActive(true));
            _buttonText.color = _selectedColor;
        }

        public void OnDeselect()
        {
            OnDeselect(null);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _selectionArrows.ForEach(o => o.SetActive(false));
            _buttonText.color = _baseColor;
        }

        private void OnPointerEnter(RSLib.Framework.GUI.EnhancedButton source)
        {
            ButtonHovered?.Invoke(this);
        }

        private void OnButtonHovered(OptionButton button)
        {
            if (button != this)
                OnDeselect();
        }

        private void Awake()
        {
            _baseColor = _buttonText.color;

            _button.PointerEnter += OnPointerEnter;
            ButtonHovered += OnButtonHovered;
        }

        private void OnDestroy()
        {
            _button.PointerEnter -= OnPointerEnter;
            ButtonHovered -= OnButtonHovered;
        }
    }
}