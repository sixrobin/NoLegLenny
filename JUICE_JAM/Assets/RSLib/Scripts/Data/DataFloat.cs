namespace RSLib
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data Float", menuName = "RSLib/Data/Float")]
    public class DataFloat : ScriptableObject
    {
        [SerializeField] private float _value = 0f;

        public float Value => _value;
    }
}