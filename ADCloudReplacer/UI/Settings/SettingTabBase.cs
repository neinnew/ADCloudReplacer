namespace ADCloudReplacer;

public abstract class SettingTabBase
{
    /// <summary>
    /// Name of tab.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Create the contents of the tab page.
    /// </summary>
    /// <param name="helper">UIHelper for the tab panel</param>
    public abstract void Create(UIHelper helper);
}