namespace JuiceJam.Settings
{
    using System.Xml.Linq;

    public class AxisReverse : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "AxisReverse";

        public AxisReverse() : base()
        {
        }

        public AxisReverse(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override void Init()
        {
            Value = false;
        }
    }
}