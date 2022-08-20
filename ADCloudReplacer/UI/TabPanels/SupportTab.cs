using ColossalFramework.UI;

namespace ADCloudReplacer
{
    public class SupportTab : SettingTabBase
    {
        public SupportTab()
        {
            Name = "Support";
        }

        public override void Create(UIHelper helper)
        {
            var group = helper.AddGroup(" ") as UIHelper;

            (group.AddButton("Steam Workshop Page", 
                () => ColossalFramework.PlatformServices.PlatformService.ActivateGameOverlayToWebPage(@"https://steamcommunity.com/sharedfiles/filedetails/?id=2077102792")
                ) as UIButton).NewStyle();
            (group.AddButton("Github Repository", () => System.Diagnostics.Process.Start("https://steamcommunity.com/sharedfiles/filedetails/?id=2077102792")) as UIButton).NewStyle();
            group.AddSpace(10);
            (group.self as UIPanel).AddUIComponent<UILabel>().text = $"{Mod.Instance.Name} {Mod.Instance.Version}";

        }
    }
}
