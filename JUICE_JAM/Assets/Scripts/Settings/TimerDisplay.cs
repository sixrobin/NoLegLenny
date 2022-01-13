namespace JuiceJam.Settings
{
    public class TimerDisplay : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "TimerDisplay";

        public TimerDisplay() : base()
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override void Init()
        {
            Value = false;
        }
    }
}