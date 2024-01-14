namespace JuiceJam.UI
{
    using RSLib.Maths;
    using UnityEngine;

    public class Score : MonoBehaviour
    {
        [System.Serializable]
        public struct StatTweenData
        {
            public Vector2 DurationMinMax;
            public Curve Curve;
        }

        [Header("DISPLAY")]
        [SerializeField] private float _delay = 1f;
        [SerializeField] private float _delayBetweenValues = 1f;
        [SerializeField] private float _pressAnyKeyDelay = 2f;
        [SerializeField] private float _preTweenDelay = 0.3f;

        [Header("COINS")]
        [SerializeField] private RSLib.Dynamics.DynamicInt _coinsCollected = null;
        [SerializeField] private GameObject _coins = null;
        [SerializeField] private TMPro.TextMeshProUGUI _coinsCountText = null;
        [SerializeField] private Animator _coinsAnimator = null;
        [SerializeField] private Color _allCoinsColor = new();
        [SerializeField] private StatTweenData _coinsTweenData = new();

        [Header("DEATHS")]
        [SerializeField] private GameObject _deaths = null;
        [SerializeField] private TMPro.TextMeshProUGUI _deathsCountText = null;
        [SerializeField] private StatTweenData _deathsTweenData = new();
        
        [Header("TIME")]
        [SerializeField] private GameObject _time = null;
        [SerializeField] private TMPro.TextMeshProUGUI _timeText = null;
        [SerializeField] private StatTweenData _timeTweenData = new();

        [Header("PRESS ANY KEY")]
        [SerializeField] private GameObject _pressAnyKey = null;

        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _statAppearingClip = null;
        [SerializeField] private RSLib.Audio.ClipProvider _statTweenEndClip = null;
        [SerializeField] private RSLib.Audio.ClipProvider _allCoinsClip = null;

        public void DisplayScore(System.Action callback = null)
        {
            StartCoroutine(DisplayScoreCoroutine(callback));
        }

        private System.Collections.IEnumerator DisplayScoreCoroutine(System.Action callback = null)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_delay);

            // Coins.
            {
                _coinsCountText.text = $"0/{GameController.CoinsTotal}";
                _coins.SetActive(true);
                RSLib.Audio.AudioManager.PlayNextPlaylistSound(_statAppearingClip);
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_preTweenDelay);

                if (_coinsCollected.Value > 0)
                {
                    float coinsTweenDuration = Maths.Normalize(_coinsCollected.Value, 0, GameController.CoinsTotal, _coinsTweenData.DurationMinMax.x, _coinsTweenData.DurationMinMax.y);
                    for (float t = 0f; t <= 1f; t += Time.deltaTime / coinsTweenDuration)
                    {
                        float tweenValue = Mathf.Lerp(0f, _coinsCollected.Value, t.Ease(_coinsTweenData.Curve));
                        _coinsCountText.text = $"{Mathf.RoundToInt(tweenValue)}/{GameController.CoinsTotal}";
                        yield return null;
                    }

                    _coinsCountText.text = $"{_coinsCollected.Value}/{GameController.CoinsTotal}";

                    if (_coinsCollected.Value == GameController.CoinsTotal)
                    {
                        RSLib.Audio.AudioManager.PlayNextPlaylistSound(_allCoinsClip);
                        _coinsAnimator.SetTrigger("AllCoins");
                        _coinsCountText.color = _allCoinsColor;
                    }
                    else
                    {
                        RSLib.Audio.AudioManager.PlayNextPlaylistSound(_statTweenEndClip);
                    }
                }
            }

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_delayBetweenValues);

            // Deaths count.
            {
                _deathsCountText.text = $"0 death";
                _deaths.SetActive(true);
                RSLib.Audio.AudioManager.PlayNextPlaylistSound(_statAppearingClip);
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_preTweenDelay);

                if (GameController.DeathsCount > 0)
                {
                    float deathTweenDuration = Maths.Normalize(GameController.DeathsCount, 0, Mathf.Max(10, GameController.DeathsCount), _deathsTweenData.DurationMinMax.x, _deathsTweenData.DurationMinMax.y);
                    for (float t = 0f; t <= 1f; t += Time.deltaTime / deathTweenDuration)
                    {
                        float tweenValue = Mathf.Lerp(0f, GameController.DeathsCount, t.Ease(_deathsTweenData.Curve));
                        _deathsCountText.text = $"{Mathf.RoundToInt(tweenValue)} {(Mathf.RoundToInt(tweenValue) <= 1 ? "death" : "deaths")}";
                        yield return null;
                    }

                    RSLib.Audio.AudioManager.PlayNextPlaylistSound(_statTweenEndClip);
                    _deathsCountText.text = $"{GameController.DeathsCount} {(GameController.DeathsCount <= 1 ? "death" : "deaths")}";
                }
            }

            yield return RSLib.Yield.SharedYields.WaitForSeconds(_delayBetweenValues);

            // Timer.
            {
                _timeText.text = System.TimeSpan.FromSeconds(0).ToString("mm':'ss'.'fff");
                _time.SetActive(true);
                RSLib.Audio.AudioManager.PlayNextPlaylistSound(_statAppearingClip);
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_preTweenDelay);

                float timeTweenDuration = Maths.Normalize(GameController.Instance.Timer, 0f, Mathf.Max(180f, GameController.Instance.Timer), _timeTweenData.DurationMinMax.x, _timeTweenData.DurationMinMax.y);
                for (float t = 0f; t <= 1f; t += Time.deltaTime / timeTweenDuration)
                {
                    float tweenValue = Mathf.Lerp(0f, GameController.Instance.Timer, t.Ease(_timeTweenData.Curve));
                    _timeText.text = System.TimeSpan.FromSeconds(tweenValue).ToString("mm':'ss'.'fff");
                    yield return null;
                }

                RSLib.Audio.AudioManager.PlayNextPlaylistSound(_statTweenEndClip);
                _timeText.text = System.TimeSpan.FromSeconds(GameController.Instance.Timer).ToString("mm':'ss'.'fff");
            }

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