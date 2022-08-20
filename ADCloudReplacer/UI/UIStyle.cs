using ColossalFramework.UI;
using UnityEngine;

namespace ADCloudReplacer
{
    public static class UIStyle
    {
        public static UIButton NewStyle(this UIButton button)
        {
            button.textScale = 0.9f;
            button.normalBgSprite = "ButtonWhite";
            button.color = new Color32(100, 128, 150, 255);
            button.focusedColor = new Color32(100, 128, 150, 255);
            button.hoveredColor = new Color32(94, 195, 255, 255);
            button.pressedColor = new Color32(212, 237, 255, 255);
            button.disabledColor = new Color32(51, 65, 77, 255);
            button.hoveredTextColor = new Color32(255, 255, 255, 255);
            button.disabledTextColor = new Color32(100, 100, 100, 255);
            return button;
        }
    }
}
