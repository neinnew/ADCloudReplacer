using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

using static ADCloudReplacer.Translation.Translator;
using static ADCloudReplacer.Translation.KeyStrings;

namespace ADCloudReplacer
{
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
            
            var group = helper.AddGroup(" ") as UIHelper;

            (group.AddButton(T(k.STEAM_WORKSHOP_PAGE_BTN), 
                () => ColossalFramework.PlatformServices.PlatformService.ActivateGameOverlayToWebPage(@"https://steamcommunity.com/sharedfiles/filedetails/?id=2077102792")
                ) as UIButton).NewStyle();
            (group.AddButton(T(k.GITHUB_REPOSITORY_BTN), () => System.Diagnostics.Process.Start("https://github.com/neinnew/ADCloudReplacer")) as UIButton).NewStyle();
            group.AddSpace(10);
            (group.self as UIPanel).AddUIComponent<UILabel>().text = $"{Mod.Instance.Name} {Mod.Instance.Version}";
            UnityEngine.Object.Destroy((group.self as UIPanel).parent.GetComponentInChildren<UILabel>().gameObject);
        }

        string[] LanguageListForDropDown => new string[] { T(k.USE_GAME_LANGUAGE) }.Concat(LanguageCodeNamePair).ToArray();
    }
}
