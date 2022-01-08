namespace JuiceJam
{
    using UnityEngine;

    public class GameController : RSLib.Framework.Singleton<GameController>
    {
        [SerializeField, Min(0f)] private float _respawnSequenceDelay = 1f;
        [SerializeField] private Color _debugColor = Color.red;

        private static System.Collections.Generic.IEnumerable<IRespawnable> s_respawnables;

        public static void Respawn()
        {
            Instance.StartCoroutine(Instance.RespawnCoroutine());
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
            s_respawnables = RSLib.Helpers.FindInstancesOfType<IRespawnable>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _debugColor;

            Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
            for (int i = checkpoints.Length - 1; i >= 0; --i)
            {
                Gizmos.DrawWireSphere(checkpoints[i].transform.position, 1f);
                if (i > 0)
                    Gizmos.DrawLine(checkpoints[i].transform.position, checkpoints[i - 1].transform.position);
            }
        }
    }
}