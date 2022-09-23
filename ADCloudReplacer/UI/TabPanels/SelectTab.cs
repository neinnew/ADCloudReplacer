using System;
using System.IO;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

using static ADCloudReplacer.Translation.Translator;
using static ADCloudReplacer.Translation.KeyStrings;

namespace ADCloudReplacer;

public class SelectTab : SettingTabBase
{
    private const int VanillaIndex = CloudLists.VanillaIndex;

    private UILabel _cloudNameLabel;
    private UILabel _cloudResolutionLabelValue;
    private UIListBox _cloudsListBox;

    private UIPanel _appliedSprite;
    private Vector3 _appliedSpriteBaseRelativePosition;

    private UIPanel _previewPanel;
    private UITextureSprite _previewSprite;
    private Texture2D _previewImage = new Texture2D(384, 96);
        
    public static SelectTab Instance;
        
    public SelectTab()
    {
        Name = T(k.TAB_NAME_SELECT);
    }

    public override void Create(UIHelper helper)
    {
        Instance = this;

        UIHelper group = helper.AddGroup(" ") as UIHelper;
        group.AddCheckbox(T(k.ENABLE_AD_CLOUD), ADCloudReplacer.ADCloudEnabled, OnEnableADCloudChanged);

        UIHelper cloudListGroup = helper.AddGroup(T(k.SELECT_CLOUD)) as UIHelper;
        var panel = cloudListGroup.self as UIPanel;

        var cloudsListPanel = panel.AddUIComponent<UIPanel>();
        const float listWidth = 300f;
        const float listHeight = 450f;
        cloudsListPanel.size = new Vector2(listWidth, listHeight);
            
        _cloudsListBox = cloudsListPanel.AddUIComponent<UIListBox>();
        _cloudsListBox.AlignTo(cloudsListPanel, UIAlignAnchor.TopLeft);
        _cloudsListBox.size = cloudsListPanel.size;
        _cloudsListBox.clipChildren = true;
        _cloudsListBox.normalBgSprite = "UnlockingPanel";
        _cloudsListBox.itemHover = "DLCButtonHovered";
        _cloudsListBox.itemHighlight = "ListItemHover";
        _cloudsListBox.itemHeight = 30;
        _cloudsListBox.itemPadding = new RectOffset(10, 0, 7, 7);
        RefreshCloudsList();
        _cloudsListBox.eventSelectedIndexChanged += OnSelectedCloudChanged;
        _cloudsListBox.eventItemDoubleClicked += (component, value) => OnApplyButtonClick();

        #region Add scrollbar to cloudListBox

        _cloudsListBox.scrollbar = _cloudsListBox.AddUIComponent<UIScrollbar>();
        var scrollbar = _cloudsListBox.scrollbar;
        scrollbar.width = 10f;
        scrollbar.height = _cloudsListBox.height;
        scrollbar.orientation = UIOrientation.Vertical;
        scrollbar.AlignTo(_cloudsListBox, UIAlignAnchor.TopRight);
        scrollbar.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left;
        scrollbar.minValue = 0;
        scrollbar.value = 0;
        scrollbar.autoHide = true;
        scrollbar.eventVisibilityChanged += (component, isVisible) => _appliedSprite.width = isVisible ? listWidth - scrollbar.width : listWidth;

        UISlicedSprite trackSprite = scrollbar.AddUIComponent<UISlicedSprite>();
        trackSprite.relativePosition = Vector2.zero;
        trackSprite.autoSize = true;
        trackSprite.size = trackSprite.parent.size;
        trackSprite.fillDirection = UIFillDirection.Vertical;
        trackSprite.spriteName = "ScrollbarTrack";
        scrollbar.trackObject = trackSprite;

        UISlicedSprite thumbSprite = trackSprite.AddUIComponent<UISlicedSprite>();
        thumbSprite.relativePosition = Vector2.zero;
        thumbSprite.fillDirection = UIFillDirection.Vertical;
        thumbSprite.autoSize = true;
        thumbSprite.width = thumbSprite.parent.width;
        thumbSprite.spriteName = "ScrollbarThumb";
        scrollbar.thumbObject = thumbSprite;

        #endregion

        #region Add sprite to display applied item
            
        _appliedSprite = _cloudsListBox.AddUIComponent<UIPanel>();
        _appliedSprite.isInteractive = false;
        _appliedSprite.height = _cloudsListBox.itemHeight;
        _appliedSprite.backgroundSprite = "DLCButtonHoveredBorderOnly";
        _appliedSprite.AlignTo(_cloudsListBox, UIAlignAnchor.TopLeft);
        _cloudsListBox.scrollbar.eventValueChanged += (component, value) => SetAppliedSpriteRelativePosition();
            
        #endregion
            
        #region Buttons on list

        var refreshListButton = cloudsListPanel.AddUIComponent<UIButton>();
        refreshListButton.size = new Vector2(26f, 26f);
        refreshListButton.atlas = Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(atlas => atlas.name == "InAssetImporter");
        refreshListButton.normalBgSprite = "ToggleRotateX";
        refreshListButton.hoveredBgSprite = "ToggleRotateXHovered";
        refreshListButton.pressedBgSprite = "ToggleRotateXPressed";
        refreshListButton.disabledBgSprite = "ToggleRotateXDisabled";
        refreshListButton.AlignTo(cloudsListPanel, UIAlignAnchor.TopRight);
        refreshListButton.relativePosition -= new Vector3(0f, 26.5f);
        refreshListButton.tooltip = T(k.TIP_REFRESH_BTN);
        refreshListButton.eventClick += (component, param) => RefreshCloudsList();
            
        var openLocalFolderButton = cloudsListPanel.AddUIComponent<UIButton>();
        openLocalFolderButton.size = new Vector2(16f, 16f);
        openLocalFolderButton.atlas = Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(atlas => atlas.name == "InAssetImporter");
        openLocalFolderButton.normalBgSprite = "FolderSelect";
        openLocalFolderButton.hoveredBgSprite = "FolderSelectHovered";
        openLocalFolderButton.pressedBgSprite = "FolderSelectPressed";
        openLocalFolderButton.disabledBgSprite = "FolderSelectDisabled";
        openLocalFolderButton.AlignTo(cloudsListPanel, UIAlignAnchor.TopRight);
        openLocalFolderButton.relativePosition -= new Vector3(30f, 21f);
        openLocalFolderButton.tooltip = T(k.TIP_OPENLOCALFOLDER_BTN);
        openLocalFolderButton.eventClick += (component, param) => System.Diagnostics.Process.Start(CloudLists.LocalDirPath);

        var downloadMoreFromWorkshopButton = cloudsListPanel.AddUIComponent<UIButton>();
        downloadMoreFromWorkshopButton.size = new Vector2(21f, 13f);
        downloadMoreFromWorkshopButton.normalBgSprite = "SteamWorkshop";
        downloadMoreFromWorkshopButton.hoveredColor = new Color32(94, 195, 255, 255);
        downloadMoreFromWorkshopButton.pressedColor = new Color32(212, 237, 255, 255);
        downloadMoreFromWorkshopButton.disabledColor = new Color32(51, 65, 77, 255);
        downloadMoreFromWorkshopButton.AlignTo(cloudsListPanel, UIAlignAnchor.TopRight);
        downloadMoreFromWorkshopButton.relativePosition -= new Vector3(55f, 19f);
        downloadMoreFromWorkshopButton.tooltip = T(k.TIP_DMFWORKSHOP_BTN);
        downloadMoreFromWorkshopButton.eventClick += (component, param) =>
            ColossalFramework.PlatformServices.PlatformService.ActivateGameOverlayToWebPage(
                @"https://steamcommunity.com/workshop/browse/?appid=255710&requiredtags%5B%5D=ad%20clouds");

        #endregion

        var selectedCloudPanel = cloudsListPanel.AddUIComponent<UIPanel>();
        selectedCloudPanel.autoLayout = true;
        selectedCloudPanel.autoLayoutDirection = LayoutDirection.Vertical;
        selectedCloudPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 10);
        selectedCloudPanel.relativePosition = new Vector3(listWidth + 10f, 0f);
        selectedCloudPanel.size = new Vector2(400f, 300f);

        #region Add clouds image preview

        _previewPanel = selectedCloudPanel.AddUIComponent<UIPanel>();
        _previewPanel.backgroundSprite = "WhiteRect";
        _previewPanel.color = new Color32(30, 30, 30, 255);
        _previewPanel.size = new Vector2(400f, 112f);
            
        _previewSprite = _previewPanel.AddUIComponent<UITextureSprite>();
        _previewSprite.size = new Vector2(384f, 96f);
        _previewSprite.CenterToParent();

        #endregion

        _cloudNameLabel = selectedCloudPanel.AddUIComponent<UILabel>();
        _cloudNameLabel.textScale = 1.3f;
        _cloudNameLabel.autoSize = false;
        _cloudNameLabel.autoHeight = true;
        _cloudNameLabel.width = selectedCloudPanel.width;
        _cloudNameLabel.wordWrap = true;

        var cloudResolutionLabel = selectedCloudPanel.AddUIComponent<UILabel>();
        cloudResolutionLabel.text = T(k.CLOUD_SIZE_LABEL);
        cloudResolutionLabel.textColor = new Color32(185, 221, 254, 255); 
        _cloudResolutionLabelValue = _cloudNameLabel.AddUIComponent<UILabel>();
        _cloudResolutionLabelValue.AlignTo(cloudResolutionLabel, UIAlignAnchor.TopLeft);
        _cloudResolutionLabelValue.relativePosition = new Vector3(cloudResolutionLabel.width + 8f, 0f);
            
        var applyButton = cloudListGroup.AddButton(T(k.APPLY_BTN), OnApplyButtonClick) as UIButton;
        applyButton.NewStyle();
        selectedCloudPanel.AttachUIComponent(applyButton.gameObject);
        applyButton.autoSize = false;
        applyButton.width = selectedCloudPanel.width;
        applyButton.textHorizontalAlignment = UIHorizontalAlignment.Center;

        #region Ensuring

        OnSelectedCloudChanged(_cloudsListBox, _cloudsListBox.selectedIndex);
        _appliedSpriteBaseRelativePosition = new Vector3(0f, _cloudsListBox.itemHeight * _cloudsListBox.selectedIndex);
        SetAppliedSpriteRelativePosition();

        #endregion
    }

    public void RefreshCloudsList()
    {
        CloudLists.Cached = false;
        _cloudsListBox.items = CloudLists.CloudNamesForDisplay;
        _cloudsListBox.selectedIndex = CloudLists.Index;
        SetAppliedSpriteRelativePosition();
        CloudPreviewControl(_cloudsListBox.selectedIndex);
        CloudLists.Cached = true;
    }
    void OnEnableADCloudChanged(bool b)
    {
        ADCloudReplacer.ADCloudEnabled = b;
        ADCloudReplacer.ToggleADCloud();
        XMLUtils.Save<ModSettings>();
    }

    void OnSelectedCloudChanged(UIComponent component, int i)
    {
        if (i == -1) return;
        _cloudNameLabel.text = _cloudsListBox.items[i];
        CloudPreviewControl(i);
    }
        
    void CloudPreviewControl(int selectedCloud)
    {
        if (_previewPanel == null) return;
            
        if (selectedCloud == VanillaIndex) // Vanilla cloud selected
        {
            if (Loading.Loaded) // In-game, vanilla cloud can be loaded.
            {
                if (ADCloudReplacer.OriginalCloudMaterial == null) return;
                var texture = ADCloudReplacer.OriginalCloudMaterial.GetTexture(Shader.PropertyToID("_CloudSampler"));
                _previewSprite.texture = texture;
                _cloudResolutionLabelValue.text = texture.width.ToString() + "x" + texture.height.ToString();
            }
            else // Displays label instead of cloud preview.
            {
                if (_previewPanel.GetComponentInChildren<UILabel>() == null)
                {
                    var label = _previewPanel.AddUIComponent<UILabel>();
                    label.autoSize = false;
                    label.autoHeight = true;
                    label.wordWrap = true;
                    label.width = 350f;
                    label.textAlignment = UIHorizontalAlignment.Center;
                    label.textColor = new Color32(185, 221, 254, 255); 
                    label.text = T(k.TIP_VANILLA_PREVIEW);
                    label.CenterToParent();
                }
                _previewSprite.Hide();
                _previewPanel.GetComponentInChildren<UILabel>().Show();
                _previewSprite.eventTextureChanged += ReappearPreviewImage;
                void ReappearPreviewImage(UIComponent component, Texture value)
                {
                    _previewPanel.GetComponentInChildren<UILabel>().Hide();
                    _previewSprite.Show();
                    _previewSprite.eventTextureChanged -= ReappearPreviewImage;
                }
                _cloudResolutionLabelValue.text = string.Empty;
            }
        }
        else // Non-vanilla cloud selected
        {
            // Note that eventTextureChanged will not be called by simply replacing it, so destroy and assign a new one.
            UnityEngine.Object.Destroy(_previewImage);
            _previewImage = new Texture2D(384, 96);
            _previewImage.LoadImage(File.ReadAllBytes(CloudLists.CloudFilePaths[selectedCloud]));
            _previewSprite.texture = _previewImage;
            _cloudResolutionLabelValue.text = _previewImage.width.ToString() + "x" + _previewImage.height.ToString();
        }
    }

    void OnApplyButtonClick()
    {
        CloudLists.SelectedCloud = CloudLists.CloudNamesForSave[_cloudsListBox.selectedIndex];
        XMLUtils.Save<ModSettings>();

        _appliedSpriteBaseRelativePosition = new Vector3(0f, _cloudsListBox.itemHeight * _cloudsListBox.selectedIndex);
        SetAppliedSpriteRelativePosition();
            
        if (_cloudsListBox.selectedIndex == VanillaIndex)
        {
            ADCloudReplacer.RevertVanilla();
        }
        else
        {
            ADCloudReplacer.LoadCloudTexture(CloudLists.CloudFilePaths[_cloudsListBox.selectedIndex]);
            ADCloudReplacer.Apply();
        }
    }

    void SetAppliedSpriteRelativePosition()
    {
        if (_appliedSprite == null) return;
        _appliedSprite.relativePosition = _appliedSpriteBaseRelativePosition - new Vector3(0f, _cloudsListBox.scrollPosition);
    }
}