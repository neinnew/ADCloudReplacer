using ColossalFramework.UI;
using UnityEngine;
using ICities;

namespace ADCloudReplacer;

internal static class UIHelperExtension
{
    /// <summary>
    /// Create a slider with an input field.
    /// </summary>
    public static UISlider AddEditableSlider(this UIHelper helper, string text, float min, float max, float step, float defaultValue, OnValueChanged eventCallback)
    {

        var slider = helper.AddSlider(text, min, max, step, defaultValue, eventCallback) as UISlider;
        slider.width = slider.parent.parent.width - 200f;

        var thumb = slider.GetComponentInChildren<UISprite>();
            

        var panelBase = slider.parent.AddUIComponent<UIPanel>();
        panelBase.width = slider.parent.parent.width - 60f;
        panelBase.height = 12f;
        panelBase.autoLayout = true;
        panelBase.autoLayoutStart = LayoutStart.BottomRight;

        var panel = panelBase.AddUIComponent<UIPanel>();
        panel.autoFitChildrenHorizontally = true;
        panel.height = 40f;
        panel.autoLayout = true;
        panel.isInteractive = false;
        panel.autoLayoutPadding.left = 3;



        var textField = helper.AddTextfield(" ", defaultValue.ToString(), (_) => { }, (__) => { }) as UITextField;
        textField.eventTextSubmitted += OnTextSubmitted;
        textField.width = 100f;
        textField.color = new Color32(100, 100, 100, 255);
        textField.disabledBgSprite = "OptionsDropboxListbox";
        textField.eventEnterFocus += (component, eventParam) => { textField.color = new Color32(255, 255, 255, 255); };
        textField.eventLeaveFocus += (component, eventParam) => { textField.color = new Color32(100, 100, 100, 255); };


        var parent = textField.parent;
        parent.RemoveUIComponent(textField);
        parent.RemoveUIComponent(parent.GetComponentInChildren<UILabel>());
        panel.AttachUIComponent(textField.gameObject);
        UnityEngine.Object.Destroy(parent.GetComponentInChildren<UILabel>());
        parent.parent.RemoveUIComponent(parent);
        UnityEngine.Object.Destroy(parent);

        slider.eventValueChanged += OnValueChanged;
        slider.eventValueChanged += RestoreThumb;


        return slider;

        void OnValueChanged(UIComponent component, float value)
        {
            textField.text = value.ToString();
            thumb.isVisible = true;
            textField.color = new Color32(100, 100, 100, 255);
        }

        // If defaultValue is float.NaN, the handle does not appear even in later normal values OnValueChanged. So force thumb size for one time on OnValueChanged.
        void RestoreThumb(UIComponent component, float value)
        {
            if (float.IsNaN(value)) return;
            thumb.size = new Vector2(16f, 16f);
            slider.eventValueChanged -= RestoreThumb;
        }

        void OnTextSubmitted(UIComponent component, string text)
        {
            float value = float.Parse(text);

            if (min <= value && value <= max)
            {
                // Ensure eventValueChanged call even if the value is same
                if (slider.value == value) 
                {
                    OnValueChanged(slider, value);
                    eventCallback(value);
                }

                slider.value = value;
            }
            else
            {
                eventCallback(value);
                thumb.isVisible = false;
                textField.color = new Color32(100, 100, 255, 255);
            }
        }
    }

    public static UITabstrip AddToggleStrip(this UIHelper helper, string[] tabNames, int defaultSelected, PropertyChangedEventHandler<int> eventSelectedIndexChanged)
    {
        var panel = helper.self as UIPanel;
        var tabstrip = panel.AddUIComponent<UITabstrip>();

        foreach (string name in tabNames)
        {
            var button = tabstrip.AddTab(name);
            button.autoSize = true;
            button.textPadding = new RectOffset(8, 8, 8, 8);
            button.normalBgSprite = "ListItemHover";
            button.disabledBgSprite = "ListItemHover";
            button.focusedBgSprite = "ListItemHighlight";
            button.hoveredBgSprite = "ListItemHover";
            button.pressedBgSprite = "ListItemHighlight";
            button.hoveredColor = new Color32(200, 200, 200, 255);
        }

        tabstrip.eventSelectedIndexChanged += eventSelectedIndexChanged;
        tabstrip.startSelectedIndex = defaultSelected;

        return tabstrip;
    }
}