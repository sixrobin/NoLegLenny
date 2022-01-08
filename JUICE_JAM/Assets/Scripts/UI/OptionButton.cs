namespace JuiceJam.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class OptionButton : MonoBehaviour, ISelectHandler
    {
        [SerializeField] private RSLib.Framework.GUI.EnhancedButton _button = null;

        public void OnSelect(BaseEventData eventData)
        {
            _button.OnPointerEnter(null);
        }
    }
}