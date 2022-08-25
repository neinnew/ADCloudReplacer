using ColossalFramework.UI;
using UnityEngine;
using System;


namespace ADCloudReplacer
{
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
            Name = "Properties";
        }

        public override void Create(UIHelper helper)
        {
            Instance = this;
            
            UIHelper group1 = helper.AddGroup("Control Mode") as UIHelper;
            _controlModeStrip = group1.AddToggleStrip(Enum.GetNames(typeof(Controller.ControlModes)), (int)Controller.ControlMode, OnControlModeChanged);
            var group1Panel = group1.self as UIPanel;
            var buttonsPanel = group1Panel.AddUIComponent<UIPanel>();
            buttonsPanel.width = group1Panel.width - 30f;
            buttonsPanel.height = 0f;
            buttonsPanel.autoLayout = true;
            buttonsPanel.autoLayoutStart = LayoutStart.BottomRight;
            buttonsPanel.autoLayoutPadding.left = 3;
            _controlModeStrip.components[0].tooltip = "Load the vanilla values. changed values will not be saved.";
            _controlModeStrip.components[1].tooltip = "Changed value will be saved to config.";

            var resetToVanillaButton = group1.AddButton("Reset to Vanilla", () => { Controller.RevertVanilla(); SetSliderValues(); }) as UIButton;
            resetToVanillaButton.NewStyle();
            resetToVanillaButton.tooltip = "Load the vanilla values by the current environment";
            buttonsPanel.AttachUIComponent(resetToVanillaButton.gameObject);
            resetToVanillaButton.isEnabled = Loading.Loaded;

            UIHelper group2 = helper.AddGroup("Control") as UIHelper;
            _group2Panel = group2.self as UIPanel;

            _alphaSaturationSlider = group2.AddEditableSlider("Alpha Saturation", 0.01f, 5f, 0.01f, Controller.AlphaSaturation ?? float.NaN, (value) => { Controller.AlphaSaturation = value; });
            _attenuationSlider = group2.AddEditableSlider("Attenuation", 0, 1f, 0.001f, Controller.Attenuation ?? float.NaN, (value) => { Controller.Attenuation = value; });
            _stepSizeSlider = group2.AddEditableSlider("Step Size", 0, 0.01f, 0.00001f, Controller.StepSize ?? float.NaN, (value) => { Controller.StepSize = value; });
            _rotateSpeedSlider = group2.AddEditableSlider("Rotate Speed", 0, 10, 0.01f, Controller.RotateSpeed ?? float.NaN, (value) => { Controller.RotateSpeed = value; });
            _skyColorMultiplierSlider = group2.AddEditableSlider("Sky Color Multiplier", 0, 10, 0.001f, Controller.SkyColorMultiplier ?? float.NaN, (value) => { Controller.SkyColorMultiplier = value; });
            _sunColorMultiplierSlider = group2.AddEditableSlider("Sun Color Multiplier", 0, 10, 0.001f, Controller.SunColorMultiplier ?? float.NaN, (value) => { Controller.SunColorMultiplier = value; });

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

                    if (Loading.Loaded)
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
            XMLUtils.Save<ModSettings>();
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
            panel.tooltip = "In Vanilla mode, setting control available in game";
            panel.color = new Color32(255, 255, 255, 75);
        }
    }
}
