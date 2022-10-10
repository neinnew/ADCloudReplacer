using ICities;

namespace ADCloudReplacer;

public class Loading : LoadingExtensionBase
{
    public static bool Loaded;

    public override void OnCreated(ILoading loading)
    {
        Loaded = true;
    }

    public override void OnLevelLoaded(LoadMode mode)
    {
        ADCloudReplacer.Initialize();
        Controller.Initialize();
        
        SettingsUI.OptionsEventHook();
        SettingsUI.Create();
        {
            SelectTab.Instance.RefreshCloudsList();
            PropertiesTab.Instance.ControlModeInitialize();
        }
        SettingsUI.Close();
    }
}