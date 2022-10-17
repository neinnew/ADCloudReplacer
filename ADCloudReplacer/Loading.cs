using System;
using ICities;

namespace ADCloudReplacer;

public class Loading : LoadingExtensionBase
{
    public static bool Created;

    public override void OnCreated(ILoading loading)
    {
        Created = true;
    }

    public override void OnLevelLoaded(LoadMode mode)
    {
        ADCloudReplacer.Initialize();
        Controller.Initialize();

        SettingsUI.OptionsEventHook();
        SettingsUI.ActionsWhileCreated(delegate
        {
            SelectTab.Instance.RefreshCloudsList();
            PropertiesTab.Instance.ControlModeInitialize();
        });

        if (InGameUIManager.ShowModButton) InGameUIManager.Create();
    }
}