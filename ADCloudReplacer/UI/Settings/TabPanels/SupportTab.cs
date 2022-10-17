using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

using static ADCloudReplacer.Translation.Translator;
using k = ADCloudReplacer.Translation.KeyStrings;

namespace ADCloudReplacer;

public class SupportTab : SettingTabBase
{
    public SupportTab()
    {
        Name = T(k.TAB_NAME_SUPPORT);
    }

    public override void Create(UIHelper helper)
    {
        var localizationGroup = helper.AddGroup(T(k.GROUP_LOCALIZATION)) as UIHelper;

        var languageDropdown = localizationGroup.AddDropdown(T(k.SELECT_LANGUAGE), LanguageListForDropDown, Translation.Translator.Index, i => Translation.Translator.Index = i) as UIDropDown;
        languageDropdown.width = 300f;
        var crowdinLink = (localizationGroup.self as UIPanel).AddUIComponent<UILabel>();
        crowdinLink.text = T(k.CROWDIN_LINK_DESC);
        crowdinLink.textColor = new Color32(192, 255, 255, 255);
        crowdinLink.eventMouseHover += (_, _) => { crowdinLink.textColor = Color.cyan; };
        crowdinLink.eventMouseLeave += (_, _) => { crowdinLink.textColor = new Color32(192, 255, 255, 255); };
        crowdinLink.eventClick += (_, _) => { System.Diagnostics.Process.Start("https://crowdin.com/project/ad-cloud-replacer"); };

        var inGameUIGroup = helper.AddGroup(T(k.GROUP_INGAMEUI)) as UIHelper;
        inGameUIGroup.AddCheckbox(T(k.SHOW_MOD_BUTTON), InGameUIManager.ShowModButton, sel => { InGameUIManager.ShowModButton = sel; XMLUtils.Save<ModSettings>(); });
        
        inGameUIGroup.AddSpace(10);
        
        var inGameUIGroupPanel = inGameUIGroup.self as UIPanel;
        
        var modButtonPositionLabel = inGameUIGroupPanel.AddUIComponent<UILabel>();
        modButtonPositionLabel.text = T(k.MOD_BUTTON_POSITION);
        var modButtonPositionPanel = inGameUIGroupPanel.AddUIComponent<UIPanel>();
        modButtonPositionPanel.autoFitChildrenHorizontally = true;
        modButtonPositionPanel.autoFitChildrenVertically = true;
        modButtonPositionPanel.autoLayout = true;
        modButtonPositionPanel.autoLayoutDirection = LayoutDirection.Horizontal;
        modButtonPositionPanel.autoLayoutPadding.left = 15;

        var modButtonPositionXTextField = inGameUIGroup.AddTextfield("X:", float.IsNaN(InGameUIManager.ModButtonPositionX) ? T(k.NOT_SET) : InGameUIManager.ModButtonPositionX.ToString(), _ => { }, _ => { }) as UITextField;
        modButtonPositionXTextField.width = 125;
        var modButtonPositionYTextField = inGameUIGroup.AddTextfield("Y:", float.IsNaN(InGameUIManager.ModButtonPositionY) ? T(k.NOT_SET) : InGameUIManager.ModButtonPositionY.ToString(), _ => { }, _ => { }) as UITextField;
        modButtonPositionYTextField.width = 125;

        AttachTextFields(modButtonPositionPanel, modButtonPositionXTextField, modButtonPositionYTextField);

        var modButtonPositionApplyButton = (inGameUIGroup.AddButton(T(k.APPLY_BTN), () =>
        {
            InGameUIManager.ModButtonPositionX = float.Parse(modButtonPositionXTextField.text);
            InGameUIManager.ModButtonPositionY = float.Parse(modButtonPositionYTextField.text);
            InGameUIManager.Destroy();
            InGameUIManager.ShowModButton = InGameUIManager.ShowModButton;
            XMLUtils.Save<ModSettings>();
        }) as UIButton).NewStyle();
        modButtonPositionPanel.AttachUIComponent(modButtonPositionApplyButton.gameObject);
        
        var modButtonPositionResetButton = (inGameUIGroup.AddButton(T(k.RESET_BTN), () =>
        {
            InGameUIManager.ModButtonPositionX = float.NaN;
            InGameUIManager.ModButtonPositionY = float.NaN;
            InGameUIManager.Destroy();
            InGameUIManager.ShowModButton = InGameUIManager.ShowModButton;
            modButtonPositionXTextField.text = T(k.NOT_SET);
            modButtonPositionYTextField.text = T(k.NOT_SET);
            XMLUtils.Save<ModSettings>();
        }) as UIButton).NewStyle();
        modButtonPositionPanel.AttachUIComponent(modButtonPositionResetButton.gameObject);
        
        
        var panelPositionLabel = inGameUIGroupPanel.AddUIComponent<UILabel>();
        panelPositionLabel.text = T(k.PANEL_POSITION);
        var panelPositionPanel = inGameUIGroupPanel.AddUIComponent<UIPanel>();
        panelPositionPanel.autoFitChildrenHorizontally = true;
        panelPositionPanel.autoFitChildrenVertically = true;
        panelPositionPanel.autoLayout = true;
        panelPositionPanel.autoLayoutDirection = LayoutDirection.Horizontal;
        panelPositionPanel.autoLayoutPadding.left = 15;
        
        var panelPositionXTextField = inGameUIGroup.AddTextfield("X:", float.IsNaN(InGameUIManager.PanelPositionX) ? T(k.NOT_SET) : InGameUIManager.PanelPositionX.ToString(), _ => { }, _ => { }) as UITextField;
        panelPositionXTextField.width = 125;
        var panelPositionYTextField = inGameUIGroup.AddTextfield("Y:", float.IsNaN(InGameUIManager.PanelPositionY) ? T(k.NOT_SET) : InGameUIManager.PanelPositionY.ToString(), _ => { }, _ => { }) as UITextField;
        panelPositionYTextField.width = 125;
        
        AttachTextFields(panelPositionPanel, panelPositionXTextField, panelPositionYTextField);
        
        var panelPositionApplyButton = (inGameUIGroup.AddButton(T(k.APPLY_BTN), () =>
        {
            InGameUIManager.PanelPositionX = float.Parse(panelPositionXTextField.text);
            InGameUIManager.PanelPositionY = float.Parse(panelPositionYTextField.text);
            InGameUIManager.Destroy();
            InGameUIManager.ShowModButton = InGameUIManager.ShowModButton;
            XMLUtils.Save<ModSettings>();
        }) as UIButton).NewStyle();
        panelPositionPanel.AttachUIComponent(panelPositionApplyButton.gameObject);
        
        var panelPositionResetButton = (inGameUIGroup.AddButton(T(k.RESET_BTN), () =>
        {
            InGameUIManager.PanelPositionX = float.NaN;
            InGameUIManager.PanelPositionY = float.NaN;
            InGameUIManager.Destroy();
            InGameUIManager.ShowModButton = InGameUIManager.ShowModButton;
            panelPositionXTextField.text = T(k.NOT_SET);
            panelPositionYTextField.text = T(k.NOT_SET);
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
            modButtonPositionApplyButton.tooltip = valid ? null : T(k.TIP_DISABLED_APPLYBTN);
        }
        
        void CheckApplicablePanelPositionValue()
        {
            bool valid = true;
            valid &= float.TryParse(panelPositionXTextField.text, out _);
            valid &= float.TryParse(panelPositionYTextField.text, out _);
            panelPositionApplyButton.isEnabled = valid;
            panelPositionApplyButton.tooltip = valid ? null : T(k.TIP_DISABLED_APPLYBTN);
        }

        var group = helper.AddGroup(" ") as UIHelper;

        (group.AddButton(T(k.STEAM_WORKSHOP_PAGE_BTN), 
            () => ColossalFramework.PlatformServices.PlatformService.ActivateGameOverlayToWebPage(@"https://steamcommunity.com/sharedfiles/filedetails/?id=2077102792")
        ) as UIButton).NewStyle();
        (group.AddButton(T(k.GITHUB_REPOSITORY_BTN), () => System.Diagnostics.Process.Start("https://github.com/neinnew/ADCloudReplacer")) as UIButton).NewStyle();
        (group.AddButton(T(k.DISCORD_BTN), () => System.Diagnostics.Process.Start("https://discord.gg/PX8y27EQyb")) as UIButton).NewStyle();
        group.AddSpace(10);
        (group.self as UIPanel).AddUIComponent<UILabel>().text = $"{Mod.Instance.Name} {Mod.Instance.Version}";
        UnityEngine.Object.Destroy((group.self as UIPanel).parent.GetComponentInChildren<UILabel>().gameObject);
    }

    

    string[] LanguageListForDropDown => new string[] { T(k.USE_GAME_LANGUAGE) }.Concat(LanguageCodeNamePair).ToArray();
}