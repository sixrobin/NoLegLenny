namespace JuiceJam.Settings
{
    public abstract class Setting : ISetting
    {
        public Setting()
        {
            Init();
        }

        public abstract string SaveElementName { get; }

        public abstract void Init();

        public abstract void LoadFromPlayerPrefs();
        public abstract void SaveToPlayerPrefs();

        public void DeleteFromPlayerPrefs()
        {
            UnityEngine.PlayerPrefs.DeleteKey(SaveElementName);
        }
    
        public virtual bool CanBeDisplayedToUser()
        {
            return true;
        }
    }
}