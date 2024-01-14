namespace JuiceJam.Settings
{
    public class ShakeAmount : FloatSetting
    {
        public const string SAVE_ELEMENT_NAME = "ShakeAmount";

        public ShakeAmount() : base()
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override (float Min, float Max) Range => (0f, 2f);
        public override float Default => 1f;
    }
}