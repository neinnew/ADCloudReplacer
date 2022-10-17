using System;
using ColossalFramework.UI;
using UnityEngine;

namespace ADCloudReplacer;

public sealed class ModButton : UIButton
{
    public override void Start()
    {
        name = Mod.Instance.Name;
        normalBgSprite = "OptionBase";
        disabledBgSprite = "OptionBaseDisabled";
        hoveredBgSprite = "OptionBaseHovered";
        pressedBgSprite = "OptionBasePressed";
        focusedBgSprite = "OptionBaseFocused";
        size = new Vector2(36f, 36f);
        playAudioEvents = true;

        var foregroundSprite = AddUIComponent<UISprite>();
        foregroundSprite.relativePosition = Vector3.zero;
        foregroundSprite.isInteractive = false;
        foregroundSprite.atlas = TextureAtlasUtils.FindTextureAtlas(ModAtlases.ADCloudReplacerAtlas);
        foregroundSprite.spriteName = ModSprites.ADCloudReplacerIcon;
        
        var freeCameraButton = GetUIView().FindUIComponent<UIComponent>("Freecamera");
        
        tooltip = Mod.Instance.Name;
        tooltipBox = freeCameraButton.tooltipBox;

        absolutePosition = float.IsNaN(InGameUIManager.ModButtonPositionX) || float.IsNaN(InGameUIManager.ModButtonPositionY)
            ? freeCameraButton.absolutePosition - new Vector3(120f, 0f)
            : new Vector3(InGameUIManager.ModButtonPositionX, InGameUIManager.ModButtonPositionY);

        InGamePanel.State = PanelState.Closed;

        eventClick += (_, _) => InGamePanel.State = InGamePanel.State switch
        {
            PanelState.Opened => PanelState.Closed,
            PanelState.Minimized => PanelState.Opened,
            PanelState.Closed => PanelState.Opened,
            _ => throw new ArgumentOutOfRangeException()
        };

        eventMouseMove += (_, param) =>
        {
            if (param.buttons.IsFlagSet(UIMouseButton.Right))
            {
                var ratio = UIView.GetAView().ratio;
                absolutePosition = new Vector3(absolutePosition.x + param.moveDelta.x * ratio, absolutePosition.y - param.moveDelta.y * ratio);
                
                InGameUIManager.ModButtonPositionX = absolutePosition.x;
                InGameUIManager.ModButtonPositionY = absolutePosition.y;
                XMLUtils.Save<ModSettings>();
            }
        };
    }

    public override void OnDestroy()
    {
        InGamePanel.State = PanelState.Closed;
        base.OnDestroy();
    }

    public new void Unfocus()
    {
        base.Unfocus();
        normalBgSprite = "OptionBase";
    }
}