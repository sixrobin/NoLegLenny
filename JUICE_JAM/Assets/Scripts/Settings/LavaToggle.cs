namespace JuiceJam.Settings
{
    using System.Xml.Linq;

    public class LavaToggle : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "PixelPerfect";

        public LavaToggle() : base()
        {
        }

        public LavaToggle(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override void Init()
        {
            Value = true;
        }
    }
}