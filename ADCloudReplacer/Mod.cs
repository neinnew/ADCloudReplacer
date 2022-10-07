using System.Reflection;
using ICities;

using static ADCloudReplacer.Translation.Translator;
using static ADCloudReplacer.Translation.KeyStrings;

namespace ADCloudReplacer;

public class Mod : IUserMod
{
    public string Name => "AD Cloud Replacer";

    public string Description => T(k.MOD_DESC) + Version;

    public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        
    public static Mod Instance { get; } = new Mod();

    public void OnEnabled()
    {
        XMLUtils.Load<ModSettings>();
        Translation.Translator.Initialize();
        
        if (LoadingManager.instance.m_loadingComplete)
        {
            CustomWorkshopTag.Initialize();
            SettingsUI.OptionsEventHook();
        }
        else
        {
            LoadingManager.instance.m_introLoaded += CustomWorkshopTag.Initialize;
            LoadingManager.instance.m_introLoaded += SettingsUI.OptionsEventHook;
        }
    }

    public void OnSettingsUI(UIHelperBase helper)
    {
        Translation.Translator.OnSettingsUI();
        SettingsUI.Setup(helper);
    }
}