using System;
using System.IO;
using System.Xml.Serialization;

namespace ADCloudReplacer;

/// <summary>
/// Global properties config.
/// </summary>
[XmlRoot("ADCloudReplacer")]
public class GlobalPropertiesConfig : IConfigurationData
{
    [XmlIgnore]
    public string SettingsFilePath => Path.Combine(CloudLists.LocalDirPath, "GlobalPropertiesConfig.xml");

    // File version.
    [XmlAttribute("Version")]
    public int version = 0;

    [XmlElement("AlphaSaturation")]
    public float? AlphaSaturation { get => Controller.AlphaSaturation; set => Controller.AlphaSaturation = value; }

    [XmlElement("Attenuation")]
    public float? Attenuation { get => Controller.Attenuation; set => Controller.Attenuation = value; }

    [XmlElement("StepSize")]
    public float? StepSize { get => Controller.StepSize; set => Controller.StepSize = value; }

    [XmlElement("RotateSpeed")]
    public float? RotateSpeed { get => Controller.RotateSpeed; set => Controller.RotateSpeed = value; }

    [XmlElement("SkyColorMultiplier")]
    public float? SkyColorMultiplier { get => Controller.SkyColorMultiplier; set => Controller.SkyColorMultiplier = value; }

    [XmlElement("SunColorMultiplier")]
    public float? SunColorMultiplier { get => Controller.SunColorMultiplier; set => Controller.SunColorMultiplier = value; }
}