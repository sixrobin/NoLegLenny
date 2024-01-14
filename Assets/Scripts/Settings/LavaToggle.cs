namespace JuiceJam.Settings
{
    public class LavaToggle : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "Lava";

        public LavaToggle() : base()
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override void Init()
        {
            Value = true;
        }
    }
}