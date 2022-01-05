namespace JuiceJam.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerHealthHeartView : MonoBehaviour
    {
        [SerializeField] private Image _heartImage = null;
        [SerializeField] private Sprite _heartOn = null;
        [SerializeField] private Sprite _heartOff = null;
        [SerializeField] private bool _hideWhenOff = false;
        [SerializeField] private Animator _heartLossAnimator = null;

        public void Toggle(bool on)
        {
            if (_heartImage.enabled == on)
                return;

            _heartImage.sprite = on ? _heartOn : _heartOff;
            _heartImage.enabled = on || !_hideWhenOff;

            if (!on)
                _heartLossAnimator.SetTrigger("Break");
        }
    }
}