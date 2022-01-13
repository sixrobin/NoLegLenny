﻿namespace JuiceJam.Settings
{
    using RSLib.Extensions;
    using System.Xml.Linq;

    public abstract class BoolSetting : Setting
    {
        public BoolSetting() : base()
        {
        }

        public BoolSetting(XElement element) : base(element)
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

        public override void Load(XElement element)
        {
            Value = element.ValueToBool();
        }

        public override XElement Save()
        {
            return new XElement(SaveElementName, Value);
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