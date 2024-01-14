namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CameraShake : RSLib.Framework.Singleton<CameraShake>
    {
        [SerializeField, Min(0f)] private float _radius = 0.3f;
        [SerializeField, Min(0f)] private float _speed = 2f;

        private Vector2 _trauma;

        public void AddTrauma(float value)
        {
            _trauma = (_trauma + Vector2.one * value).ClampAll01();
        }

        public void AddTrauma(Vector2 value)
        {
            _trauma = (_trauma + value).ClampAll01();
        }

        public void AddTrauma(float x, float y)
        {
            _trauma = (_trauma + new Vector2(x, y)).ClampAll01();
        }

        public void SetTrauma(float value)
        {
            _trauma = Vector2.one * Mathf.Clamp01(value);
        }

        public void SetTrauma(Vector2 value)
        {
            _trauma = value.ClampAll01();
        }

        public void SetTrauma(float x, float y)
        {
            _trauma = new Vector2(Mathf.Clamp01(x), Mathf.Clamp01(y));
        }

        public Vector3 GetShakeRaw()
        {
            if (_trauma.sqrMagnitude == 0f)
                return Vector3.zero;

            _trauma = _trauma.AddAll(-Instance._speed * Time.deltaTime);
            _trauma = _trauma.ClampAll01();

            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            randomDirection *= _trauma;
            Vector3 shake = randomDirection * Instance._radius;
            shake *= Settings.SettingsManager.ShakeAmount.Value;

            return shake;
        }

        private void LateUpdate()
        {
            if (UI.OptionsPanel.Instance.IsOpen || UI.OptionsPanel.Instance.PausingCoroutineRunning)
                return;

            transform.position = GetShakeRaw();
        }
    }
}