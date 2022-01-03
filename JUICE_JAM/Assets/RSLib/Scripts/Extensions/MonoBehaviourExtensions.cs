namespace RSLib
{
    using UnityEngine;

    public static class MonoBehaviourExtensions
    {
        #region DO AFTER

        /// <summary>
        /// Invokes a callback method after a delay.
        /// </summary>
        /// <param name="coroutineRunner">MonoBehaviour that will run the delay coroutine.</param>
        /// <param name="delay">Seconds to wait before callback.</param>
        /// <param name="callback">Callback behaviour.</param>
        public static void DoAfter(this MonoBehaviour coroutineRunner, float delay, System.Action callback)
        {
            if (callback == null)
                throw new System.Exception($"Coroutine callback cannot be null.");

            coroutineRunner.StartCoroutine(DoAfterCoroutine(delay, callback));
        }

        private static System.Collections.IEnumerator DoAfterCoroutine(float delay, System.Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }

        #endregion DO AFTER
    }
}