namespace JuiceJam
{
    using System.Linq;
    using UnityEngine;

    public class GameController : RSLib.Framework.Singleton<GameController>
    {
        [SerializeField, Min(0f)] private float _respawnSequenceDelay = 1f;
        [SerializeField] private Color _debugColor = Color.red;
        [SerializeField] private UI.Score _score = null;
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private RSLib.Framework.DisabledString _totalCoinsCount = new();

        private static bool s_scoreDisplayed;
        private static bool s_timerGoing;

        private static System.Collections.Generic.IEnumerable<IRespawnable> s_respawnables;

        public static int CoinsTotal { get; private set; }
        public static int DeathsCount { get; private set; }
        public float Timer { get; private set; }

        [ContextMenu("1")]
        public void SetDeathCount1() => DeathsCount = 1;
        [ContextMenu("5")]
        public void SetDeathCount5() => DeathsCount = 5;
        [ContextMenu("10")]
        public void SetDeathCount10() => DeathsCount = 10;
        [ContextMenu("100")]
        public void SetDeathCount100() => DeathsCount = 100;

        public static void Respawn()
        {
            DeathsCount++;

            Instance.StartCoroutine(Instance.RespawnCoroutine());
        }

        public static void ResetGame()
        {
            TimeManager.SetTimeScale(1f);
            s_timerGoing = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        public static void DisplayStatistics()
        {
            Instance._score.DisplayScore(() => s_scoreDisplayed = true);
        }

        private System.Collections.IEnumerator RespawnCoroutine()
        {
            DitherFade.FadeIn(0.5f, RSLib.Maths.Curve.Linear, _respawnSequenceDelay);
            yield return new WaitUntil(() => !DitherFade.IsFading);

            foreach (IRespawnable respawnable in s_respawnables)
                respawnable.Respawn();

            DitherFade.FadeOut(0.5f, RSLib.Maths.Curve.Linear, 1f);
        }

        private void Start()
        {
            Moon.MoonFinalPositionReached += OnMoonFinalPositionReached;
            _playerController.FirstMovementInput += OnPlayerFirstMovementInput;

            s_respawnables = RSLib.Helpers.FindInstancesOfType<IRespawnable>();
            CoinsTotal = FindObjectsOfType<CoinView>().Length;
        }

        private void OnPlayerFirstMovementInput()
        {
            s_timerGoing = true;
        }

        private void OnMoonFinalPositionReached()
        {
            s_timerGoing = false;
        }

        private void Update()
        {
            if (s_timerGoing && !_playerController.IsDead && !DitherFade.IsFading)
                Timer += Time.unscaledDeltaTime;

            if (s_scoreDisplayed && Input.anyKeyDown)
            {
                s_scoreDisplayed = false;
                ResetGame();
            }
        }

        private void OnDestroy()
        {
            Moon.MoonFinalPositionReached -= OnMoonFinalPositionReached;
            _playerController.FirstMovementInput -= OnPlayerFirstMovementInput;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _debugColor;

            Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>()
                                                         .ToList()
                                                         .OrderBy(o => o.transform.position.y)
                                                         .ToArray();

            for (int i = checkpoints.Length - 1; i >= 0; --i)
            {
                Gizmos.DrawWireSphere(checkpoints[i].transform.position, 1f);
                if (i > 0)
                    Gizmos.DrawLine(checkpoints[i].transform.position, checkpoints[i - 1].transform.position);
            }

            _totalCoinsCount = new RSLib.Framework.DisabledString(FindObjectsOfType<CoinView>().Length.ToString());
        }
    }
}