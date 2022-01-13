namespace JuiceJam.Settings
{
    using System.Xml.Linq;

    public abstract class Setting : ISetting
    {
        public Setting()
        {
            Init();
        }

        public Setting(XElement element)
        {
            Load(element);
        }

        public abstract string SaveElementName { get; }

        public abstract void Init();

        public abstract void Load(XElement element);
        public abstract XElement Save();

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