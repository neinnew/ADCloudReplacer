using System;
using System.IO;
using System.Xml.Serialization;

namespace ADCloudReplacer;

/// <summary>
/// Global mod settings.
/// </summary>
[XmlRoot("ADCloudReplacer")]
public class ModSettings : IConfigurationData
{
    [XmlIgnore]
    public string SettingsFilePath => Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "ADCloudReplacer.xml");

    // File version.
    [XmlAttribute("Version")]
    public int version = 0;

    [XmlElement("Language")]
    public string Language { get => nnCitiesShared.Translation.Translator.Language; set => nnCitiesShared.Translation.Translator.Language = value; }

    [XmlElement("UseGameLanguage")]
    public bool UseGameLanguage { get => nnCitiesShared.Translation.Translator.UseGameLanguage; set => nnCitiesShared.Translation.Translator.UseGameLanguage = value; }
    
    [XmlElement("ShowModButton")]
    public bool ShowModButton { get => InGameUIManager.ShowModButton; set => InGameUIManager.ShowModButton = value; }
    
    [XmlElement("ModButtonPositionX")]
    public float ModButtonPositionX { get => InGameUIManager.ModButtonPositionX; set => InGameUIManager.ModButtonPositionX = value; }
    
    [XmlElement("ModButtonPositionY")]
    public float ModButtonPositionY { get => InGameUIManager.ModButtonPositionY; set => InGameUIManager.ModButtonPositionY = value; }
    
    [XmlElement("PanelPositionX")]
    public float PanelPositionX { get => InGameUIManager.PanelPositionX; set => InGameUIManager.PanelPositionX = value; }
    
    [XmlElement("PanelPositionY")]
    public float PanelPositionY { get => InGameUIManager.PanelPositionY; set => InGameUIManager.PanelPositionY = value; }
        
    [XmlElement("ADCloudEnabled")]
    public bool ADCloudEnabled { get => ADCloudReplacer.ADCloudEnabled; set => ADCloudReplacer.ADCloudEnabled = value; }

    [XmlElement("SelectedCloud")]
    public string SelectedCloud { get => CloudLists.SelectedCloud; set => CloudLists.SelectedCloud = value; }

    [XmlElement("ControlMode")]
    public Controller.ControlModes ControlMode { get => Controller.ControlMode; set => Controller.ControlMode = value; }
}