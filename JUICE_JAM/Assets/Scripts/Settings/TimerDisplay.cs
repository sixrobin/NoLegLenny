namespace JuiceJam.Settings
{
    using System.Xml.Linq;

    public class TimerDisplay : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "TimerDisplay";

        public TimerDisplay() : base()
        {
        }

        public TimerDisplay(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override void Init()
        {
            Value = false;
        }
    }
}