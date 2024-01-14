namespace JuiceJam.Settings
{
    public interface ISetting
    {
        void Init();
        bool CanBeDisplayedToUser();

        void LoadFromPlayerPrefs();
        void SaveToPlayerPrefs();
        void DeleteFromPlayerPrefs();
    }
}