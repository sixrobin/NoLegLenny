namespace JuiceJam.Settings
{
    public abstract class IntSetting : Setting
    {
        public IntSetting() : base()
        {
        }

        public delegate void ValueChangedEventHandler(int previousValue, int currentValue);
        public event ValueChangedEventHandler ValueChanged;

        private int _value;
        public virtual int Value
        {
            get => _value;
            set
            {
                int previousValue = _value;
                _value = UnityEngine.Mathf.Clamp(value, Range.Min, Range.Max);
                ValueChanged?.Invoke(previousValue, _value);
            }
        }

        public abstract (int Min, int Max) Range { get; }

        public override void Init()
        {
            Value = Range.Max;
        }

        public override void LoadFromPlayerPrefs()
        {
            if (UnityEngine.PlayerPrefs.HasKey(SaveElementName))
                Value = UnityEngine.PlayerPrefs.GetInt(SaveElementName);
        }

        public override void SaveToPlayerPrefs()
        {
            UnityEngine.PlayerPrefs.SetInt(SaveElementName, Value);
        }
    }
}