namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class CameraShake : RSLib.Framework.Singleton<CameraShake>
    {
        [SerializeField, Min(0f)] private float _radius = 0.3f;
        [SerializeField, Min(0f)] private float _speed = 2f;

        private static Vector2 s_trauma;

        public static void AddTrauma(float value)
        {
            s_trauma = (s_trauma + Vector2.one * value).ClampAll01();
        }

        public static void AddTrauma(Vector2 value)
        {
            s_trauma = (s_trauma + value).ClampAll01();
        }

        public static void AddTrauma(float x, float y)
        {
            s_trauma = (s_trauma + new Vector2(x, y)).ClampAll01();
        }

        public static void SetTrauma(float value)
        {
            s_trauma = Vector2.one * Mathf.Clamp01(value);
        }

        public static void SetTrauma(Vector2 value)
        {
            s_trauma = value.ClampAll01();
        }

        public static void SetTrauma(float x, float y)
        {
            s_trauma = new Vector2(Mathf.Clamp01(x), Mathf.Clamp01(y));
        }

        public static Vector3 GetShakeRaw()
        {
            if (s_trauma.sqrMagnitude == 0f/*|| Manager.FreezeFrameManager.IsFroze*/)
                return Vector3.zero;

            s_trauma = s_trauma.AddAll(-Instance._speed * Time.deltaTime);
            s_trauma = s_trauma.ClampAll01();

            Vector2 rndDir = Random.insideUnitCircle.normalized;
            rndDir *= s_trauma;

            return rndDir * Instance._radius;
        }

        private void LateUpdate()
        {
            if (UI.OptionsPanel.IsOpen || UI.OptionsPanel.PausingCoroutineRunning)
                return;

            transform.position = GetShakeRaw();
        }
    }
}