namespace JuiceJam
{
    using UnityEngine;

    public class GameController : RSLib.Framework.Singleton<GameController>
    {
        [SerializeField, Min(0f)] private float _respawnSequenceDelay = 1f;

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
    }
}