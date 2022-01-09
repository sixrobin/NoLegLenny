namespace JuiceJam.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class OptionButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] protected RSLib.Framework.GUI.EnhancedButton _button = null;
        [SerializeField] private System.Collections.Generic.List<GameObject> _selectionArrows = null;
        [SerializeField] protected TMPro.TextMeshProUGUI _buttonText = null;

        private Color _baseColor;

        public static event System.Action<OptionButton> ButtonHovered;

        public void OnSelect()
        {
            OnSelect(null);
        }

        public void OnSelect(BaseEventData eventData)
        {
            _selectionArrows.ForEach(o => o.SetActive(true));
            _buttonText.color = Color.white;
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

        protected virtual void Awake()
        {
            _baseColor = _buttonText.color;

            _button.PointerEnter += OnPointerEnter;
            ButtonHovered += OnButtonHovered;
        }

        protected virtual void OnDestroy()
        {
            _button.PointerEnter -= OnPointerEnter;
            ButtonHovered -= OnButtonHovered;
        }
    }
}