using System;
using System.IO;
using System.Xml.Serialization;

namespace ADCloudReplacer;

public interface IConfigurationData
{
    string SettingsFilePath { get; }
}

public class BackingForSerializeAttribute : Attribute
{
    public BackingForSerializeAttribute(string xmlElement)
    {
    }
}
    
public static class XMLUtils
{
    public static void Load<T>() where T : IConfigurationData, new()
    {
        try
        {
            T data = new();

            // Check to see if configuration file exists.
            if (File.Exists(data.SettingsFilePath))
            {
                // Read it.
                using StreamReader reader = new StreamReader(data.SettingsFilePath);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                if (!(xmlSerializer.Deserialize(reader) is T settingsFile))
                {
                    UnityEngine.Debug.Log(Mod.Instance.Name + ": couldn't deserialize settings file");
                }
            }
            else
            {
                UnityEngine.Debug.Log(Mod.Instance.Name + ": no settings file found");
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }

    public static void Save<T>() where T : IConfigurationData, new()
    {
        try
        {
            T data = new();

            using StreamWriter writer = new StreamWriter(data.SettingsFilePath);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(writer, new T());
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }
}