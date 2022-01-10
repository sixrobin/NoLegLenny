namespace JuiceJam
{
    using UnityEngine;

    public class MainCamera : RSLib.Framework.Singleton<MainCamera>
    {
        public static Vector3 Position => Instance.transform.position;
    }
}