namespace JuiceJam.Settings
{
    public class AxisReverse : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "AxisReverse";

        public AxisReverse() : base()
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override void Init()
        {
            Value = false;
        }
    }
}