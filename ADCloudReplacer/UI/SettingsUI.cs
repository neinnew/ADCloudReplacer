using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace ADCloudReplacer;

public class SettingsUI
{
    private UIScrollablePanel _mainPanel;
    private UIPanel _modNamedPanel;

    public SettingsUI(UIHelperBase helper)
    {
        _mainPanel = ((UIHelper)helper).self as UIScrollablePanel;
        _modNamedPanel = _mainPanel.parent as UIPanel;

        var tabStrip = _modNamedPanel.AddUIComponent<UITabstrip>();
        tabStrip.relativePosition = Vector2.zero;
        tabStrip.width = _modNamedPanel.width;
        tabStrip.height = 34f;

        var modNameLabelPanel = _modNamedPanel.AddUIComponent<UIPanel>();
        modNameLabelPanel.width = 300f;
        modNameLabelPanel.height = 34f;
        modNameLabelPanel.relativePosition = new Vector2(_modNamedPanel.width - modNameLabelPanel.width, 0f);
        modNameLabelPanel.autoLayout = true;
        modNameLabelPanel.autoLayoutStart = LayoutStart.TopRight;

        var modNameLabel = modNameLabelPanel.AddUIComponent<UILabel>();
        modNameLabel.textScale = 1.125f;
        modNameLabel.text = Mod.Instance.Name;
        modNameLabel.padding = new RectOffset(15, 15, 7, 7);
        modNameLabel.anchor = UIAnchorStyle.CenterVertical | UIAnchorStyle.Right;

        UITabContainer tabContainer = _modNamedPanel.AddUIComponent<UITabContainer>();
        tabContainer.relativePosition = new Vector2(0, 34f);
        tabContainer.width = tabStrip.width;
        tabContainer.height = _modNamedPanel.height - tabStrip.height;
        tabStrip.tabPages = tabContainer;

        tabStrip.AddSettingTab<SelectTab>();
        tabStrip.AddSettingTab<PropertiesTab>();
        tabStrip.AddSettingTab<SupportTab>();
    }
}