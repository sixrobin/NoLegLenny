namespace RSLib
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data Int", menuName = "RSLib/Data/Int")]
    public class DataInt : ScriptableObject
    {
        [SerializeField] private int _value = 0;

        public int Value => _value;
    }
}