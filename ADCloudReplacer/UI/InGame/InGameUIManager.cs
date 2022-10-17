using ColossalFramework.UI;

namespace ADCloudReplacer;

public static class InGameUIManager
{
    public static ModButton ModButton;
    
    #region Store

    [BackingForSerialize(nameof(ModSettings.ShowModButton))]
    public static bool ShowModButton
    {
        get => _showModButton;
        set
        {
            if (value)
            {
                if (Loading.Created)
                {
                    Create();
                }
            }
            else
            {
                Destroy();
            }
            _showModButton = value;
        }
    }
    private static bool _showModButton = true;

    [BackingForSerialize(nameof(ModSettings.ModButtonPositionX))]
    public static float ModButtonPositionX = float.NaN;
    
    [BackingForSerialize(nameof(ModSettings.ModButtonPositionY))]
    public static float ModButtonPositionY = float.NaN;
    
    [BackingForSerialize(nameof(ModSettings.PanelPositionX))]
    public static float PanelPositionX = float.NaN;
    
    [BackingForSerialize(nameof(ModSettings.PanelPositionY))]
    public static float PanelPositionY = float.NaN;

    #endregion

    public static void Create()
    {
        ModButton ??= UIView.GetAView().AddUIComponent(typeof(ModButton)) as ModButton;
    }

    public static void Destroy()
    {
        if (ModButton == null) return;
        
        UnityEngine.Object.Destroy(ModButton.gameObject);
        UnityEngine.Object.Destroy(ModButton);
        ModButton = null;
    }
}