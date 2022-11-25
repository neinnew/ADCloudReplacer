using System.Reflection;
using ICities;
using nnCitiesShared.Translation;

using static nnCitiesShared.Translation.Usage;
using k = nnCitiesShared.Translation.KeyStrings;

namespace ADCloudReplacer;

public class Mod : IUserMod
{
    public string Name => "AD Cloud Replacer";

    public string Description => T[k.MOD_DESC, $"{Version}"];

    public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        
    public static Mod Instance { get; } = new Mod();

    public void OnEnabled()
    {
        XMLUtils.Load<ModSettings>();
        
        if (LoadingManager.instance.m_loadingComplete)
        {
            CustomWorkshopTag.Initialize();
            SettingsUI.OptionsEventHook();
            Translator.Initialize();
        }
        else
        {
            LoadingManager.instance.m_introLoaded += CustomWorkshopTag.Initialize;
            LoadingManager.instance.m_introLoaded += SettingsUI.OptionsEventHook;
            LoadingManager.instance.m_introLoaded += Translator.Initialize;
        }
    }

    public void OnSettingsUI(UIHelperBase helper)
    {
        SettingsUI.Setup(helper);
    }
}