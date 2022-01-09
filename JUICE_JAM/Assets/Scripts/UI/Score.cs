namespace JuiceJam.UI
{
    using UnityEngine;

    public class Score : MonoBehaviour
    {
        [Header("DISPLAY")]
        [SerializeField] private float _delay = 1f;
        [SerializeField] private float _delayBetweenValues = 1f;
        [SerializeField] private float _pressAnyKeyDelay = 2f;

        [Header("COINS")]
        [SerializeField] private RSLib.Dynamics.DynamicInt _coinsCollected = null;
        [SerializeField] private GameObject _coins = null;
        [SerializeField] private TMPro.TextMeshProUGUI _coinsCountText = null;

        [Header("DEATHS")]
        [SerializeField] private GameObject _deaths = null;
        [SerializeField] private TMPro.TextMeshProUGUI _deathsCountText = null;

        [Header("TIME")]
        [SerializeField] private GameObject _time = null;
        [SerializeField] private TMPro.TextMeshProUGUI _timeText = null;

        [Header("PRESS ANY KEY")]
        [SerializeField] private GameObject _pressAnyKey = null;

        public void DisplayScore(System.Action callback = null)
        {
            StartCoroutine(DisplayScoreCoroutine(callback));
        }

        private System.Collections.IEnumerator DisplayScoreCoroutine(System.Action callback = null)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_delay);

            _coinsCountText.text = $"{_coinsCollected.Value}/{GameController.CoinsTotal}";
            _coins.SetActive(true);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_delayBetweenValues);

            _deathsCountText.text = $"{GameController.DeathsCount} {(GameController.DeathsCount <= 1 ? "death" : "deaths")}";
            _deaths.SetActive(true);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_delayBetweenValues);

            // TODO.
            _timeText.text = $"X:XX:XX";
            _time.SetActive(true);

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_pressAnyKeyDelay);

            _pressAnyKey.SetActive(true);

            callback?.Invoke();
        }

        private void Awake()
        {
            _coins.SetActive(false);
            _deaths.SetActive(false);
            _time.SetActive(false);
            _pressAnyKey.SetActive(false);
        }
    }
}