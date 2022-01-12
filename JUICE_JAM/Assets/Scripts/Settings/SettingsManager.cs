namespace JuiceJam.Settings
{
    public class SettingsManager : RSLib.Framework.ConsoleProSingleton<SettingsManager>
    {
        public static ConstrainCursor ConstrainCursor { get; private set; }
        public static FullscreenMode FullscreenMode { get; private set; }
        public static PixelPerfect PixelPerfect { get; private set; }
        public static RunInBackground RunInBackground { get; private set; }
        public static AxisReverse AxisReverse { get; private set; }
        public static ShakeAmount ShakeAmount { get; private set; }
        public static LavaToggle LavaToggle { get; private set; }
        public static Volume Volume { get; private set; }
        public static TimerDisplay TimerDisplay { get; private set; }

        public static void Init()
        {
            ConstrainCursor = new();
            FullscreenMode = new();
            PixelPerfect = new();
            RunInBackground = new();
            AxisReverse = new();
            ShakeAmount = new();
            LavaToggle = new();
            Volume = new();
            TimerDisplay = new();
        }

        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;

            Init();
        }
    }
}