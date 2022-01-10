namespace JuiceJam
{
    using UnityEngine;

    public class MainCamera : RSLib.Framework.Singleton<MainCamera>
    {
        private static Camera s_camera;

        public static Camera Camera
        {
            get
            {
                if (s_camera == null)
                    s_camera = Instance.GetComponent<Camera>();

                return s_camera;
            }
        }

        public static Vector3 Position => Instance.transform.position;
    }
}