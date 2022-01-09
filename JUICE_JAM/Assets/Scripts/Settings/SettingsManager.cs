namespace JuiceJam.Settings
{
    public class SettingsManager : RSLib.Framework.ConsoleProSingleton<SettingsManager>
    {
        public static ConstrainCursor ConstrainCursor { get; private set; }
        public static FullscreenMode FullscreenMode { get; private set; }
        public static PixelPerfect PixelPerfect { get; private set; }
        public static RunInBackground RunInBackground { get; private set; }
        public static YAxisReverse YAxisReverse { get; private set; }
        public static ShakeAmount ShakeAmount { get; private set; }

        public static void Init()
        {
            ConstrainCursor = new ConstrainCursor();
            FullscreenMode = new FullscreenMode();
            PixelPerfect = new PixelPerfect();
            RunInBackground = new RunInBackground();
            YAxisReverse = new YAxisReverse();
            ShakeAmount = new ShakeAmount();
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