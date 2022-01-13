namespace JuiceJam.Settings
{
    public abstract class BoolSetting : Setting
    {
        public BoolSetting() : base()
        {
        }

        public delegate void ValueChangedEventHandler(bool currentValue);
        public event ValueChangedEventHandler ValueChanged;

        private bool _value;
        public virtual bool Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged?.Invoke(_value);
            }
        }

        public override void LoadFromPlayerPrefs()
        {
            if (UnityEngine.PlayerPrefs.HasKey(SaveElementName))
                Value = UnityEngine.PlayerPrefs.GetInt(SaveElementName) == 1;
        }

        public override void SaveToPlayerPrefs()
        {
            UnityEngine.PlayerPrefs.SetInt(SaveElementName, Value ? 1 : 0);
        }
    }
}