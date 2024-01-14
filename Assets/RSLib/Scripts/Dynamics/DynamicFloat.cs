namespace RSLib.Dynamics
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Dynamic Float", menuName = "RSLib/Dynamics/Float")]
    public class DynamicFloat : ScriptableObject
    {
        public struct ValueChangedEventArgs
        {
            public float Previous;
            public float New;
        }

        [SerializeField]
        private float _value = 0f;

        public event System.Action<ValueChangedEventArgs> ValueChanged;

        public float Value
        {
            get => _value;
            set
            {
                ValueChangedEventArgs valueChangedEventArgs = new ValueChangedEventArgs()
                {
                    Previous = _value,
                    New = value
                };

                _value = value;

                ValueChanged?.Invoke(valueChangedEventArgs);
            }
        }
    }
}