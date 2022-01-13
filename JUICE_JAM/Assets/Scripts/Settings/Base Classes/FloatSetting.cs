namespace JuiceJam.Settings
{
    using RSLib.Extensions;

    public abstract class FloatSetting : Setting
    {
        public FloatSetting() : base()
        {
        }

        public delegate void ValueChangedEventHandler(float previousValue, float currentValue);
        public event ValueChangedEventHandler ValueChanged;

        private float _value;
        public virtual float Value
        {
            get => _value;
            set
            {
                float previousValue = _value;
                _value = UnityEngine.Mathf.Clamp(value, Range.Min, Range.Max);
                ValueChanged?.Invoke(previousValue, _value);
            }
        }

        public float ValueSqr => Value * Value;

        public abstract (float Min, float Max) Range { get; }
        public virtual float Default => Range.Max;

        public override void Init()
        {
            Value = Default;
        }

        public override void LoadFromPlayerPrefs()
        {
            if (UnityEngine.PlayerPrefs.HasKey(SaveElementName))
                Value = UnityEngine.PlayerPrefs.GetFloat(SaveElementName);
        }

        public override void SaveToPlayerPrefs()
        {
            UnityEngine.PlayerPrefs.SetFloat(SaveElementName, Value);
        }
    }
}