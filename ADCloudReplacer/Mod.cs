using ColossalFramework.Globalization;
using ICities;

namespace ADCloudReplacer
{
    public class Mod : IUserMod
    {
        public string Name => "AD Cloud Replacer";

        public string Description => "replace and enables the cloud used in After Dark(before weather update) " + Version;

        public string Version => "1.0.0";
        
        public static Mod Instance { get; } = new Mod();

        public void OnEnabled()
        {
            XMLUtils.Load<ModSettings>();
            if (LoadingManager.instance.m_loadingComplete) CustomWorkshopTag.Initialize();
            LoadingManager.instance.m_introLoaded += CustomWorkshopTag.Initialize;
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            new SettingsUI(helper);
        }
    }
}
