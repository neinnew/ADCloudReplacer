using ColossalFramework.UI;
using UnityEngine;

using static ADCloudReplacer.Translation.Translator;
using k = ADCloudReplacer.Translation.KeyStrings;

namespace ADCloudReplacer;

public class PropertiesTab : SettingTabBase
{
        
    UISlider _alphaSaturationSlider;
    UISlider _attenuationSlider;
    UISlider _stepSizeSlider;
    UISlider _rotateSpeedSlider;
    UISlider _skyColorMultiplierSlider;
    UISlider _sunColorMultiplierSlider;
    UITabstrip _controlModeStrip;
    UIPanel _group2Panel;

    public static PropertiesTab Instance;

    public PropertiesTab()
    {
        Name = T(k.TAB_NAME_PROPERTIES);
    }

    public override void Create(UIHelper helper)
    {
        Instance = this;
            
        UIHelper group1 = helper.AddGroup(T(k.CONTROL_MODE)) as UIHelper;
        _controlModeStrip = group1.AddToggleStrip(new string[] { T(k.MODE_VANILLA), T(k.MODE_CUSTOM) }, (int)Controller.ControlMode, OnControlModeChanged);
        _controlModeStrip.eventSelectedIndexChanged += (_, _) => XMLUtils.Save<ModSettings>();

        var group1Panel = group1.self as UIPanel;
        var buttonsPanel = group1Panel.AddUIComponent<UIPanel>();
        buttonsPanel.width = group1Panel.width - 30f;
        buttonsPanel.height = 0f;
        buttonsPanel.autoLayout = true;
        buttonsPanel.autoLayoutStart = LayoutStart.BottomRight;
        buttonsPanel.autoLayoutPadding.left = 3;
        _controlModeStrip.components[0].tooltip = T(k.TIP_VANILLA_MODE);
        _controlModeStrip.components[1].tooltip = T(k.TIP_CUSTOM_MODE);

        var resetToVanillaButton = group1.AddButton(T(k.RESET_TO_VANILLA_BTN), () => { Controller.RevertVanilla(); SetSliderValues(); }) as UIButton;
        resetToVanillaButton.NewStyle();
        resetToVanillaButton.tooltip = T(k.TIP_RESET_TO_VANILLA_BTN);
        buttonsPanel.AttachUIComponent(resetToVanillaButton.gameObject);
        resetToVanillaButton.isEnabled = Loading.Created;

        UIHelper group2 = helper.AddGroup(T(k.GROUP_CONTROL)) as UIHelper;
        _group2Panel = group2.self as UIPanel;

        _alphaSaturationSlider = group2.AddEditableSlider(T(k.ALPHA_SATURATION), 0.01f, 5f, 0.01f, Controller.AlphaSaturation ?? float.NaN, (value) => { Controller.AlphaSaturation = value; });
        _attenuationSlider = group2.AddEditableSlider(T(k.ATTENUATION), 0, 1f, 0.001f, Controller.Attenuation ?? float.NaN, (value) => { Controller.Attenuation = value; });
        _stepSizeSlider = group2.AddEditableSlider(T(k.STEP_SIZE), 0, 0.01f, 0.00001f, Controller.StepSize ?? float.NaN, (value) => { Controller.StepSize = value; });
        _rotateSpeedSlider = group2.AddEditableSlider(T(k.ROTATE_SPPED), 0, 10, 0.01f, Controller.RotateSpeed ?? float.NaN, (value) => { Controller.RotateSpeed = value; });
        _skyColorMultiplierSlider = group2.AddEditableSlider(T(k.SKY_COLOR_MULTIPLIER), 0, 10, 0.001f, Controller.SkyColorMultiplier ?? float.NaN, (value) => { Controller.SkyColorMultiplier = value; });
        _sunColorMultiplierSlider = group2.AddEditableSlider(T(k.SUN_COLOR_MULTIPLIER), 0, 10, 0.001f, Controller.SunColorMultiplier ?? float.NaN, (value) => { Controller.SunColorMultiplier = value; });

        ControlModeInitialize();
            
        foreach (var uic in _group2Panel.components)
        {
            var slider = uic.GetComponentInChildren<UISlider>();
            slider.disabledColor = slider.color;
        }
    }

    public void ControlModeInitialize()
    {
        OnControlModeChanged(_controlModeStrip, (int)Controller.ControlMode);
    }
        
    void OnControlModeChanged(UIComponent component, int index)
    {
        if (_group2Panel == null) return;

        switch ((Controller.ControlModes)index)
        {
            case Controller.ControlModes.Vanilla:

                Controller.ControlMode = Controller.ControlModes.Vanilla;
                RemoveCustomConfigSaveEvent();

                if (Loading.Created)
                {
                    EnableControlPanel(_group2Panel);
                    Controller.RevertVanilla();
                    SetSliderValues();
                }
                else
                {
                    DisableControlPanel(_group2Panel);
                }

                break;

            case Controller.ControlModes.Custom:

                Controller.ControlMode = Controller.ControlModes.Custom;
                EnableControlPanel(_group2Panel);
                LoadCustomConfig();
                AddCustomConfigSaveEvent();

                break;
                
            default:
                break;
        };
    }

    void SetSliderValues()
    {
        _alphaSaturationSlider.value = Controller.AlphaSaturation ?? float.NaN;
        _attenuationSlider.value = Controller.Attenuation ?? float.NaN;
        _stepSizeSlider.value = Controller.StepSize ?? float.NaN;
        _rotateSpeedSlider.value = Controller.RotateSpeed ?? float.NaN;
        _skyColorMultiplierSlider.value = Controller.SkyColorMultiplier ?? float.NaN;
        _sunColorMultiplierSlider.value = Controller.SunColorMultiplier ?? float.NaN;
    }

    void LoadCustomConfig()
    {
        XMLUtils.Load<GlobalPropertiesConfig>();
        SetSliderValues();
    }

    void AddCustomConfigSaveEvent()
    {
        // Prevent events from being double subscribed
        RemoveCustomConfigSaveEvent();
            
        _alphaSaturationSlider.eventValueChanged += SaveCustomConfig;
        _attenuationSlider.eventValueChanged += SaveCustomConfig;
        _stepSizeSlider.eventValueChanged += SaveCustomConfig;
        _rotateSpeedSlider.eventValueChanged += SaveCustomConfig;
        _skyColorMultiplierSlider.eventValueChanged += SaveCustomConfig;
        _sunColorMultiplierSlider.eventValueChanged += SaveCustomConfig;
    }

    void RemoveCustomConfigSaveEvent()
    {
        _alphaSaturationSlider.eventValueChanged -= SaveCustomConfig;
        _attenuationSlider.eventValueChanged -= SaveCustomConfig;
        _stepSizeSlider.eventValueChanged -= SaveCustomConfig;
        _rotateSpeedSlider.eventValueChanged -= SaveCustomConfig;
        _skyColorMultiplierSlider.eventValueChanged -= SaveCustomConfig;
        _sunColorMultiplierSlider.eventValueChanged -= SaveCustomConfig;
    }

    void SaveCustomConfig(UIComponent component, float value)
    {
        XMLUtils.Save<GlobalPropertiesConfig>();
    }

    void EnableControlPanel(UIPanel panel)
    {
        panel.Enable();
        panel.tooltip = null;
        panel.color = Color.white;
    }

    void DisableControlPanel(UIPanel panel)
    {
        panel.Disable();
        panel.tooltip = T(k.TIP_DISABLED_CONTROL_PANEL);
        panel.color = new Color32(255, 255, 255, 75);
    }
}