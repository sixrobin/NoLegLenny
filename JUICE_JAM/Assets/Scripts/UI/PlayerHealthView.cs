namespace JuiceJam.UI
{
    using UnityEngine;

    public class PlayerHealthView : MonoBehaviour
    {
        [SerializeField] private RSLib.Dynamics.DynamicInt _playerHealth = null;
        [SerializeField] private PlayerHealthHeartView[] _hearts = null;

        private void OnPlayerHealthValueChanged(RSLib.Dynamics.DynamicInt.ValueChangedEventArgs args)
        {
            for (int i = 0; i < _hearts.Length; ++i)
                _hearts[i].Toggle(i < args.New);
        }

        private void Awake()
        {
            _playerHealth.ValueChanged += OnPlayerHealthValueChanged;
        }

        private void OnDestroy()
        {
            _playerHealth.ValueChanged -= OnPlayerHealthValueChanged;
        }
    }
}