using ColossalFramework.UI;
using UnityEngine;

namespace ADCloudReplacer;

public static class TabstripExtension
{
    /// <summary>
    /// Adds a tab to a SettingsUI tabstrip.
    /// </summary>
    public static void AddSettingTab<TTab>(this UITabstrip tabstrip) where TTab : SettingTabBase, new()
    {
        TTab tab = new TTab();

        var tabButton = tabstrip.AddTab(tab.Name);
        tabButton.normalBgSprite = "GenericTab";
        tabButton.disabledBgSprite = "GenericTabDisabled";
        tabButton.focusedBgSprite = "GenericTabFocused";
        tabButton.hoveredBgSprite = "GenericTabHovered";
        tabButton.pressedBgSprite = "GenericTabPressed";
        tabButton.color = new Color32(195, 195, 195, 255);
        tabButton.focusedTextColor = new Color32(16, 16, 16, 255);
        tabButton.textPadding = new RectOffset(15, 15, 10, 6);
        tabButton.autoSize = true;

        tabstrip.selectedIndex = tabstrip.tabCount - 1;
        var rootPanel = tabstrip.tabContainer.components[tabstrip.selectedIndex] as UIPanel;
        rootPanel.autoLayout = true;
        rootPanel.autoLayoutDirection = LayoutDirection.Horizontal;
        var panel = rootPanel.AddUIComponent<UIScrollablePanel>();
        panel.autoLayout = true;
        panel.autoLayoutDirection = LayoutDirection.Vertical;
        panel.size = new Vector2(rootPanel.size.x - 10f, rootPanel.size.y);
        panel.autoLayoutPadding.top = 10;
        panel.autoLayoutPadding.left = 10;

        var scrollbar = rootPanel.AddUIComponent<UIScrollbar>();
        scrollbar.width = 10f;
        scrollbar.height = rootPanel.height;
        scrollbar.orientation = UIOrientation.Vertical;
        scrollbar.pivot = UIPivotPoint.TopLeft;
        scrollbar.AlignTo(rootPanel, UIAlignAnchor.TopRight);
        scrollbar.minValue = 0;
        scrollbar.value = 0;
        scrollbar.incrementAmount = 30;
        scrollbar.autoHide = true;

        UISlicedSprite trackSprite = scrollbar.AddUIComponent<UISlicedSprite>();
        trackSprite.relativePosition = Vector2.zero;
        trackSprite.autoSize = true;
        trackSprite.size = trackSprite.parent.size;
        trackSprite.fillDirection = UIFillDirection.Vertical;
        trackSprite.spriteName = "ScrollbarTrack";
        scrollbar.trackObject = trackSprite;

        UISlicedSprite thumbSprite = trackSprite.AddUIComponent<UISlicedSprite>();
        thumbSprite.relativePosition = Vector2.zero;
        thumbSprite.fillDirection = UIFillDirection.Vertical;
        thumbSprite.autoSize = true;
        thumbSprite.width = thumbSprite.parent.width;
        thumbSprite.spriteName = "ScrollbarThumb";
        scrollbar.thumbObject = thumbSprite;

        scrollbar.eventValueChanged += (component, value) => { panel.scrollPosition = new Vector2(0, value); };
        panel.eventMouseWheel += (component, eventParam) => { scrollbar.value -= (int)eventParam.wheelDelta * scrollbar.incrementAmount; };
        panel.verticalScrollbar = scrollbar;

        tab.Create(new UIHelper(panel));
    }
}