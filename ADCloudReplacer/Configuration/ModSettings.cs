using System;
using System.IO;
using System.Xml.Serialization;

namespace ADCloudReplacer
{
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

        [XmlElement("ADCloudEnabled")]
        public bool ADCloudEnabled { get => ADCloudReplacer.ADCloudEnabled; set => ADCloudReplacer.ADCloudEnabled = value; }

        [XmlElement("SelectedCloud")]
        public string SelectedCloud { get => CloudLists.SelectedCloud; set => CloudLists.SelectedCloud = value; }

        [XmlElement("ControlMode")]
        public Controller.ControlModes ControlMode { get => Controller.ControlMode; set => Controller.ControlMode = value; }
    }
}