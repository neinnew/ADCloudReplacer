using System;
using System.Linq;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using nnCitiesShared.Translation;

using static nnCitiesShared.Translation.Usage;
using k = nnCitiesShared.Translation.KeyStrings;

namespace ADCloudReplacer;

public class SupportTab : SettingTabBase
{
    public SupportTab()
    {
        Name = T[k.TAB_NAME_SUPPORT];
    }

    public override void Create(UIHelper helper)
    {
        var localizationGroup = helper.AddGroup(T[k.GROUP_LOCALIZATION]) as UIHelper;

        int length = Translator.Instance.LanguageCodes.Length + 1;
        var languageListForDropdown = new string[length];
        languageListForDropdown[0] = $"{T[k.USE_GAME_LANGUAGE]} : {LocaleManager.instance.language}";
        for (int i = 1; i < length; i++)
        {
            string nativeName = Translator.Instance.LanguageNativeNames[i - 1];
            string languageCode = Translator.Instance.LanguageCodes[i - 1];
            string displayName = T[$"LANGUAGE_NAME:{languageCode}"];
            
            languageListForDropdown[i] = $"{nativeName} ({displayName}) | {languageCode}";
        }
        
        var languageDropdown = localizationGroup.AddDropdown(T[k.SELECT_LANGUAGE], 
            options: languageListForDropdown, 
            defaultSelection: Translator.UseGameLanguage ? 0 : Array.FindIndex(Translator.Instance.LanguageCodes, code => code == Translator.Language) + 1, 
            eventCallback: i =>
            {
                if (i == 0)
                {
                    Translator.UseGameLanguage = true;
                    Translator.Language = Translator.GameLanguage;
                }
                else
                {
                    Translator.UseGameLanguage = false;
                    Translator.Language = Translator.Instance.LanguageCodes[i - 1];
                }
                Translator.Update();
                XMLUtils.Save<ModSettings>(); 
            }) as UIDropDown;
        languageDropdown.width = 300f;

        var inGameUIGroup = helper.AddGroup(T[k.GROUP_INGAMEUI]) as UIHelper;
        inGameUIGroup.AddCheckbox(T[k.SHOW_MOD_BUTTON], InGameUIManager.ShowModButton, sel => { InGameUIManager.ShowModButton = sel; XMLUtils.Save<ModSettings>(); });
        
        inGameUIGroup.AddSpace(10);
        
        var inGameUIGroupPanel = inGameUIGroup.self as UIPanel;
        
        var modButtonPositionLabel = inGameUIGroupPanel.AddUIComponent<UILabel>();
        modButtonPositionLabel.text = T[k.MOD_BUTTON_POSITION];
        var modButtonPositionPanel = inGameUIGroupPanel.AddUIComponent<UIPanel>();
        modButtonPositionPanel.autoFitChildrenHorizontally = true;
        modButtonPositionPanel.autoFitChildrenVertically = true;
        modButtonPositionPanel.autoLayout = true;
        modButtonPositionPanel.autoLayoutDirection = LayoutDirection.Horizontal;
        modButtonPositionPanel.autoLayoutPadding.left = 15;

        var modButtonPositionXTextField = inGameUIGroup.AddTextfield("X:", float.IsNaN(InGameUIManager.ModButtonPositionX) ? T[k.NOT_SET] : InGameUIManager.ModButtonPositionX.ToString(), _ => { }, _ => { }) as UITextField;
        modButtonPositionXTextField.width = 125;
        var modButtonPositionYTextField = inGameUIGroup.AddTextfield("Y:", float.IsNaN(InGameUIManager.ModButtonPositionY) ? T[k.NOT_SET] : InGameUIManager.ModButtonPositionY.ToString(), _ => { }, _ => { }) as UITextField;
        modButtonPositionYTextField.width = 125;

        AttachTextFields(modButtonPositionPanel, modButtonPositionXTextField, modButtonPositionYTextField);

        var modButtonPositionApplyButton = (inGameUIGroup.AddButton(T[k.APPLY], () =>
        {
            InGameUIManager.ModButtonPositionX = float.Parse(modButtonPositionXTextField.text);
            InGameUIManager.ModButtonPositionY = float.Parse(modButtonPositionYTextField.text);
            InGameUIManager.Destroy();
            InGameUIManager.ShowModButton = InGameUIManager.ShowModButton;
            XMLUtils.Save<ModSettings>();
        }) as UIButton).NewStyle();
        modButtonPositionPanel.AttachUIComponent(modButtonPositionApplyButton.gameObject);
        
        var modButtonPositionResetButton = (inGameUIGroup.AddButton(T[k.RESET], () =>
        {
            InGameUIManager.ModButtonPositionX = float.NaN;
            InGameUIManager.ModButtonPositionY = float.NaN;
            InGameUIManager.Destroy();
            InGameUIManager.ShowModButton = InGameUIManager.ShowModButton;
            modButtonPositionXTextField.text = T[k.NOT_SET];
            modButtonPositionYTextField.text = T[k.NOT_SET];
            XMLUtils.Save<ModSettings>();
        }) as UIButton).NewStyle();
        modButtonPositionPanel.AttachUIComponent(modButtonPositionResetButton.gameObject);
        
        
        var panelPositionLabel = inGameUIGroupPanel.AddUIComponent<UILabel>();
        panelPositionLabel.text = T[k.PANEL_POSITION];
        var panelPositionPanel = inGameUIGroupPanel.AddUIComponent<UIPanel>();
        panelPositionPanel.autoFitChildrenHorizontally = true;
        panelPositionPanel.autoFitChildrenVertically = true;
        panelPositionPanel.autoLayout = true;
        panelPositionPanel.autoLayoutDirection = LayoutDirection.Horizontal;
        panelPositionPanel.autoLayoutPadding.left = 15;
        
        var panelPositionXTextField = inGameUIGroup.AddTextfield("X:", float.IsNaN(InGameUIManager.PanelPositionX) ? T[k.NOT_SET] : InGameUIManager.PanelPositionX.ToString(), _ => { }, _ => { }) as UITextField;
        panelPositionXTextField.width = 125;
        var panelPositionYTextField = inGameUIGroup.AddTextfield("Y:", float.IsNaN(InGameUIManager.PanelPositionY) ? T[k.NOT_SET] : InGameUIManager.PanelPositionY.ToString(), _ => { }, _ => { }) as UITextField;
        panelPositionYTextField.width = 125;
        
        AttachTextFields(panelPositionPanel, panelPositionXTextField, panelPositionYTextField);
        
        var panelPositionApplyButton = (inGameUIGroup.AddButton(T[k.APPLY], () =>
        {
            InGameUIManager.PanelPositionX = float.Parse(panelPositionXTextField.text);
            InGameUIManager.PanelPositionY = float.Parse(panelPositionYTextField.text);
            InGameUIManager.Destroy();
            InGameUIManager.ShowModButton = InGameUIManager.ShowModButton;
            XMLUtils.Save<ModSettings>();
        }) as UIButton).NewStyle();
        panelPositionPanel.AttachUIComponent(panelPositionApplyButton.gameObject);
        
        var panelPositionResetButton = (inGameUIGroup.AddButton(T[k.RESET], () =>
        {
            InGameUIManager.PanelPositionX = float.NaN;
            InGameUIManager.PanelPositionY = float.NaN;
            InGameUIManager.Destroy();
            InGameUIManager.ShowModButton = InGameUIManager.ShowModButton;
            panelPositionXTextField.text = T[k.NOT_SET];
            panelPositionYTextField.text = T[k.NOT_SET];
            XMLUtils.Save<ModSettings>();
        }) as UIButton).NewStyle();
        panelPositionPanel.AttachUIComponent(panelPositionResetButton.gameObject);

        #region Ensuring

        modButtonPositionXTextField.eventTextChanged += (_, _) => CheckApplicableModButtonPositionValue();
        modButtonPositionYTextField.eventTextChanged += (_, _) => CheckApplicableModButtonPositionValue();
        panelPositionXTextField.eventTextChanged += (_, _) => CheckApplicablePanelPositionValue();
        panelPositionYTextField.eventTextChanged += (_, _) => CheckApplicablePanelPositionValue();
        CheckApplicableModButtonPositionValue();
        CheckApplicablePanelPositionValue();

        #endregion
        
        void AttachTextFields(UIPanel target, params UITextField[] textFields)
        {
            foreach (var textField in textFields)
            {
                var parent = textField.parent;
                var label = parent.GetComponentInChildren<UILabel>();
                parent.RemoveUIComponent(textField);
                parent.RemoveUIComponent(label);
                UnityEngine.Object.Destroy(parent);
                target.AttachUIComponent(label.gameObject);
                target.AttachUIComponent(textField.gameObject);
            }
        }
        
        void CheckApplicableModButtonPositionValue()
        {
            bool valid = true;
            valid &= float.TryParse(modButtonPositionXTextField.text, out _);
            valid &= float.TryParse(modButtonPositionYTextField.text, out _);
            modButtonPositionApplyButton.isEnabled = valid;
            modButtonPositionApplyButton.tooltip = valid ? null : T[k.TIP_DISABLED_APPLYBTN];
        }
        
        void CheckApplicablePanelPositionValue()
        {
            bool valid = true;
            valid &= float.TryParse(panelPositionXTextField.text, out _);
            valid &= float.TryParse(panelPositionYTextField.text, out _);
            panelPositionApplyButton.isEnabled = valid;
            panelPositionApplyButton.tooltip = valid ? null : T[k.TIP_DISABLED_APPLYBTN];
        }

        var group = helper.AddGroup(" ") as UIHelper;

        (group.AddButton(T[k.STEAM_WORKSHOP_PAGE], 
            () => ColossalFramework.PlatformServices.PlatformService.ActivateGameOverlayToWebPage(@"https://steamcommunity.com/sharedfiles/filedetails/?id=2077102792")
        ) as UIButton).NewStyle();
        (group.AddButton(T[k.GITHUB_REPOSITORY], () => System.Diagnostics.Process.Start("https://github.com/neinnew/ADCloudReplacer")) as UIButton).NewStyle();
        (group.AddButton(T[k.DISCORD_SERVER], () => System.Diagnostics.Process.Start("https://discord.gg/PX8y27EQyb")) as UIButton).NewStyle();
        group.AddSpace(10);
        (group.self as UIPanel).AddUIComponent<UILabel>().text = $"{Mod.Instance.Name} {Mod.Instance.Version}";
        (group.self as UIPanel).AddUIComponent<UILabel>().text =
#if DEBUG
            "Debug"
#endif
#if RELEASE
            "Release"
#endif
#if WORKSHOP
            "Workshop"
#endif
            + " Build " + nnCitiesShared.Utilities.AssemblyUtils.ThisAssembly.GetName().Version.ToString().Split('.')[3];
        UnityEngine.Object.Destroy((group.self as UIPanel).parent.GetComponentInChildren<UILabel>().gameObject);
    }
}