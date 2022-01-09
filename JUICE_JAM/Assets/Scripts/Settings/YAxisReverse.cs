namespace JuiceJam.Settings
{
    using System.Xml.Linq;

    public class YAxisReverse : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "YAxisReverse";

        public YAxisReverse() : base()
        {
        }

        public YAxisReverse(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override void Init()
        {
            Value = false;
        }
    }
}