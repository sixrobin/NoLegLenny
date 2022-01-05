namespace RSLib.Dynamics
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Dynamic Int", menuName = "RSLib/Dynamics/Int")]
    public class DynamicInt : ScriptableObject
    {
        public struct ValueChangedEventArgs
        {
            public int Previous;
            public int New;
        }

        [SerializeField]
        private int _value = 0;

        public event System.Action<ValueChangedEventArgs> ValueChanged;

        public int Value
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