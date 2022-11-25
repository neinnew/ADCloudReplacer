using ColossalFramework.UI;
using System;
using ICities;
using nnCitiesShared.Translation;
using UnityEngine;

namespace ADCloudReplacer;

public static class SettingsUI
{
    private static GameObject _optionsGameObject;
    private static UIPanel _gameOptionsPanel;
    private static UIPanel _modNamedPanel;
    private static UIPanel _optionsPanel;

    private static UITabstrip _optionsTabStrip;

    static SettingsUI()
    {
        Translator.UpdateSettingsUI += LocaleChanged;
    }
    
    public static void Setup(UIHelperBase helper)
    {
        _modNamedPanel = (((UIHelper)helper).self as UIScrollablePanel)?.parent as UIPanel;
    }

    private static void AddTabs()
    {
        _optionsTabStrip.AddSettingTab<SelectTab>();
        _optionsTabStrip.AddSettingTab<PropertiesTab>();
        _optionsTabStrip.AddSettingTab<SupportTab>();
    }
    
    /// code to enable/disable options panel based on visibility from AlgernonCommons
    /// Partially modified. (Use helper.parent as base panel, not helper)
    /// https://github.com/algernon-A/AlgernonCommons/blob/master/UI/OptionsPanelManager.cs
    
    /// <summary>
    /// Attaches an event hook to options panel visibility, to enable/disable mod hokey when the panel is open.
    /// </summary>
    public static void OptionsEventHook()
    {
        // Get options panel instance.
        _gameOptionsPanel = UIView.library.Get<UIPanel>("OptionsPanel");

        if (_gameOptionsPanel == null)
        {
            Debug.LogError("couldn't find OptionsPanel");
        }
        else
        {
            // Simple event hook to create/destroy GameObject based on appropriate visibility.
            _gameOptionsPanel.eventVisibilityChanged += (c, isVisible) =>
            {
                // Create/destroy based on whether or not we're now visible.
                if (isVisible)
                {
                    Create();
                }
                else
                {
                    Close();
                }
            };

            // Recreate panel on system locale change.
            ColossalFramework.Globalization.LocaleManager.eventLocaleChanged += LocaleChanged;
        }
    }

    /// <summary>
    /// Refreshes the options panel (destroys and rebuilds) on a locale change when the options panel is open.
    /// </summary>
    public static void LocaleChanged()
    {
        if (_gameOptionsPanel != null && _gameOptionsPanel.isVisible)
        {
            Close();
            Create();
            
            _optionsTabStrip.startSelectedIndex = _optionsTabStrip.tabCount - 1;
        }
    }

    public static void ActionsWhileCreated(Action action)
    {
        Create();
        action();
        Close();
    }
    
    private static void AddModStrip()
    {
        _optionsTabStrip = _optionsPanel.AddUIComponent<UITabstrip>();
        _optionsTabStrip.relativePosition = Vector2.zero;
        _optionsTabStrip.width = _optionsPanel.width;
        _optionsTabStrip.height = 34f;

        var modNameLabelPanel = _optionsPanel.AddUIComponent<UIPanel>();
        modNameLabelPanel.width = 300f;
        modNameLabelPanel.height = 34f;
        modNameLabelPanel.isInteractive = false;
        modNameLabelPanel.relativePosition = new Vector2(_optionsPanel.width - modNameLabelPanel.width, 0f);
        modNameLabelPanel.autoLayout = true;
        modNameLabelPanel.autoLayoutStart = LayoutStart.TopRight;

        var modNameLabel = modNameLabelPanel.AddUIComponent<UILabel>();
        modNameLabel.isInteractive = false;
        modNameLabel.textScale = 1.125f;
        modNameLabel.text = Mod.Instance.Name;
        modNameLabel.padding = new RectOffset(15, 15, 7, 7);
        modNameLabel.anchor = UIAnchorStyle.CenterVertical | UIAnchorStyle.Right;

        UITabContainer tabContainer = _optionsPanel.AddUIComponent<UITabContainer>();
        tabContainer.relativePosition = new Vector2(0, 34f);
        tabContainer.width = _optionsTabStrip.width;
        tabContainer.height = _optionsPanel.height - _optionsTabStrip.height;
        _optionsTabStrip.tabPages = tabContainer;
    }
    
    /// <summary>
    /// Creates the panel object in-game and displays it.
    /// </summary>
    private static void Create()
    {
        try
        {
            // If no instance already set, create one.
            if (_optionsGameObject == null)
            {
                // Create parent GameObject.
                _optionsGameObject = new GameObject("OptionsPanel");
                _optionsGameObject.transform.parent = _modNamedPanel.transform;

                // Create a base panel attached to our game object, perfectly overlaying the game options panel.
                _optionsPanel = _optionsGameObject.AddComponent<UIPanel>();
                _optionsPanel.width = _modNamedPanel.width;
                _optionsPanel.height = _modNamedPanel.height;
                
                // Needed to ensure position is consistent if we regenerate after initial opening (e.g. on language change).
                _optionsPanel.relativePosition = Vector3.zero;
                
                AddModStrip();
                AddTabs();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    /// <summary>
    /// Closes the panel by destroying the object (removing any ongoing UI overhead).
    /// </summary>
    private static void Close()
    {
        // We're no longer visible - destroy our game object.
        if (_optionsGameObject != null)
        {
            UnityEngine.Object.Destroy(_optionsPanel.gameObject);
            UnityEngine.Object.Destroy(_optionsPanel);
            UnityEngine.Object.Destroy(_optionsGameObject);

            // Release references.
            _optionsPanel = null;
            _optionsGameObject = null;
        }
    }
}