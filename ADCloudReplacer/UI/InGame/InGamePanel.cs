using System;
using ColossalFramework.UI;
using UnityEngine;

namespace ADCloudReplacer;

public enum PanelState
{
    Opened,
    Minimized,
    Closed
}

public sealed class InGamePanel : UIPanel
{
    private static InGamePanel _instance;
    private static PanelState _state;

    public override void Start()
    {
        name = "ADCloudReplacerPanel";
        size = new Vector2(764f, 710f);
        backgroundSprite = "AdvisorBubble";
        color = Color.gray;
        absolutePosition = float.IsNaN(InGameUIManager.PanelPositionX) || float.IsNaN(InGameUIManager.PanelPositionY)
            ? InitialPanelPosition()
            : new Vector3(InGameUIManager.PanelPositionX, InGameUIManager.PanelPositionY);
        
        const float topBarHeight = 40f;
        
        var dragHandle = AddUIComponent<UIDragHandle>();
        dragHandle.relativePosition = Vector3.zero;
        dragHandle.target = this;
        dragHandle.width = this.width;
        dragHandle.height = topBarHeight;
        dragHandle.eventMouseUp += (_, _) =>
        {
            InGameUIManager.PanelPositionX = this.absolutePosition.x;
            InGameUIManager.PanelPositionY = this.absolutePosition.y;
            XMLUtils.Save<ModSettings>();
        };

        var icon = AddUIComponent<UISprite>();
        icon.relativePosition = new Vector3(3f, 3f);
        icon.isInteractive = false;
        icon.atlas = TextureAtlasUtils.FindTextureAtlas(ModAtlases.ADCloudReplacerAtlas);
        icon.spriteName = ModSprites.ADCloudReplacerIcon;
        
        var nameLabel = AddUIComponent<UILabel>();
        nameLabel.text = Mod.Instance.Name;
        nameLabel.relativePosition = new Vector3(0f, 1f); // Optical adjustment (y:1f), Due to round of AdvisorBubble sprite
        nameLabel.autoSize = false;
        nameLabel.width = this.width;
        nameLabel.height = topBarHeight;
        nameLabel.textAlignment = UIHorizontalAlignment.Center;
        nameLabel.verticalAlignment = UIVerticalAlignment.Middle;
        nameLabel.isInteractive = false;
        
        var closeButton = AddUIComponent<UIButton>();
        closeButton.relativePosition = new Vector2(this.width - 37, 3);
        closeButton.normalBgSprite = "buttonclose";
        closeButton.hoveredBgSprite = "buttonclosehover";
        closeButton.pressedBgSprite = "buttonclosepressed";
        closeButton.eventClick += (_, _) => State = PanelState.Closed;
        
        var minimizeButton = AddUIComponent<UIButton>();
        minimizeButton.relativePosition = new Vector2(this.width - 70, 3);
        minimizeButton.atlas = TextureAtlasUtils.FindTextureAtlas(ModAtlases.MinimizeButtonAtlas);
        minimizeButton.normalBgSprite = ModSprites.MinimizeNormal;
        minimizeButton.hoveredBgSprite = ModSprites.MinimizeHovered;
        minimizeButton.pressedBgSprite = ModSprites.MinimizePressed;
        minimizeButton.eventClick += (_, _) => State = PanelState.Minimized;
        
        var optionsTabStrip = AddUIComponent<UITabstrip>();
        optionsTabStrip.relativePosition = new Vector3(0f, topBarHeight);
        optionsTabStrip.width = width;
        optionsTabStrip.height = 34f;

        UITabContainer tabContainer = AddUIComponent<UITabContainer>();
        tabContainer.relativePosition = new Vector2(0, 34f + topBarHeight);
        tabContainer.width = optionsTabStrip.width;
        tabContainer.height = this.height - optionsTabStrip.height - topBarHeight;
        optionsTabStrip.tabPages = tabContainer;
        
        optionsTabStrip.AddSettingTab<SelectTab>();
        optionsTabStrip.AddSettingTab<PropertiesTab>();
        
        Vector3 InitialPanelPosition()
        {
            // 44(InfoPanel) + 49(TSBar) + 25(ThumbnailBar)
            const float bottomBarHeight = 118f;
            const float padding = 15f;
            
            return GetUIView().GetScreenResolution() - this.size - new Vector2(padding, padding + bottomBarHeight);
        }
    }

    /// <summary>
    /// Manage the state of panel according to its state.
    /// </summary>
    public static PanelState State
    {
        get => _state;
        set
        {
            if (value == _state) 
                return;
            
            switch (value)
            {
                case PanelState.Opened:
                    
                    if (_instance == null)
                    { // If the panel does not exist, create it.
                        Create();
                    }
                    else
                    { // The panel exists (when previous state is minimized), make it visible. 
                        _instance.isVisible = true;
                    }
                    
                    // Focused sprites should be used regardless of button's state.
                    InGameUIManager.ModButton.normalBgSprite = InGameUIManager.ModButton.focusedBgSprite;
                    
                    break;
                
                case PanelState.Minimized:
                    
                    // Make panel invisible
                    Minimize();
                    
                    // Release the button focus
                    InGameUIManager.ModButton?.Unfocus();
                    
                    break;
                
                case PanelState.Closed:
                    
                    // Destroy the panel
                    Close();
                    
                    // Release the button focus
                    InGameUIManager.ModButton?.Unfocus();
                    
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            _state = value;
        }
    }
    
    private static void Create()
    {
        _instance ??= UIView.GetAView().AddUIComponent(typeof(InGamePanel)) as InGamePanel;
    }

    private static void Minimize()
    {
        if (_instance == null) return;
        
        _instance.isVisible = false;
    }

    private static void Close()
    {
        if (_instance == null) return;
        
        Destroy(_instance.gameObject);
        Destroy(_instance);
        _instance = null;
    }
}