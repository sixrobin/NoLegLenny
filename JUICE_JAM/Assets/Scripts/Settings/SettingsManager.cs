namespace JuiceJam.Settings
{
    public class SettingsManager : RSLib.Framework.Singleton<SettingsManager>
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

        public static void SaveToPlayerPrefs()
        {
            ConstrainCursor.SaveToPlayerPrefs();
            FullscreenMode.SaveToPlayerPrefs();
            PixelPerfect.SaveToPlayerPrefs();
            RunInBackground.SaveToPlayerPrefs();
            AxisReverse.SaveToPlayerPrefs();
            ShakeAmount.SaveToPlayerPrefs();
            LavaToggle.SaveToPlayerPrefs();
            Volume.SaveToPlayerPrefs();
            TimerDisplay.SaveToPlayerPrefs();

            UnityEngine.PlayerPrefs.Save();
            Instance.Log("PlayerPrefs settings keys have been saved.", forceVerbose: true);
        }

        public static void LoadFromPlayerPrefs()
        {
            ConstrainCursor.LoadFromPlayerPrefs();
            FullscreenMode.LoadFromPlayerPrefs();
            PixelPerfect.LoadFromPlayerPrefs();
            RunInBackground.LoadFromPlayerPrefs();
            AxisReverse.LoadFromPlayerPrefs();
            ShakeAmount.LoadFromPlayerPrefs();
            LavaToggle.LoadFromPlayerPrefs();
            Volume.LoadFromPlayerPrefs();
            TimerDisplay.LoadFromPlayerPrefs();

            Instance.Log("PlayerPrefs settings keys have been loaded.", forceVerbose: true);
        }

        [UnityEngine.ContextMenu("Delete PlayerPrefs Keys")]
        public void DeletePlayerPrefsKeys()
        {
            ConstrainCursor.DeleteFromPlayerPrefs();
            FullscreenMode.DeleteFromPlayerPrefs();
            PixelPerfect.DeleteFromPlayerPrefs();
            RunInBackground.DeleteFromPlayerPrefs();
            AxisReverse.DeleteFromPlayerPrefs();
            ShakeAmount.DeleteFromPlayerPrefs();
            LavaToggle.DeleteFromPlayerPrefs();
            Volume.DeleteFromPlayerPrefs();
            TimerDisplay.DeleteFromPlayerPrefs();

            UnityEngine.PlayerPrefs.Save();
            Instance.Log("PlayerPrefs settings keys have been deleted.", forceVerbose: true);
        }

        [UnityEngine.ContextMenu("Delete All PlayerPrefs Keys")]
        public void DeleteAllPlayerPrefsKeys()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
            UnityEngine.PlayerPrefs.Save();

            Instance.Log("All PlayerPrefs keys have been deleted.", forceVerbose: true);
        }

        protected override void Awake()
        {
            base.Awake();
            if (!IsValid)
                return;

            Init();
            LoadFromPlayerPrefs();
        }
    }
}