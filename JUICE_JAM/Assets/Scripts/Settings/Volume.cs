namespace JuiceJam.Settings
{
    using System.Xml.Linq;

    public class Volume : FloatSetting
    {
        public const string SAVE_ELEMENT_NAME = "Volume";

        public Volume() : base()
        {
        }

        public Volume(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override (float Min, float Max) Range => (0f, 1f);
        public override float Default => 1f;
    }
}