namespace JuiceJam.UI
{
    using UnityEngine;

    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private RSLib.Framework.SceneField _gameScene = null;
        [SerializeField] private float _maxSplashDuration = 15f;

        private float _maxSplashTimer;

        public void OnSplashAnimationOver()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_gameScene);
        }

        private void Update()
        {
            _maxSplashTimer += Time.unscaledDeltaTime;
            if (_maxSplashTimer >= _maxSplashDuration)
                OnSplashAnimationOver();
        }
    }
}