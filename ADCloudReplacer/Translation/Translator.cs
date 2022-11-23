using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework.Globalization;
using ColossalFramework.Plugins;
using UnityEngine;

namespace ADCloudReplacer.Translation;

public static class Translator
{
    [BackingForSerialize(nameof(ModSettings.UseGameLanguage))]
    public static bool UseGameLanguage = true;
    
    [BackingForSerialize(nameof(ModSettings.Language))]
    public static string Language
    {
        get => _language;
        set
        {
            _language = value;
            _translations = value == DefaultLanguage ? SourceEnglish : GetTranslation(value);
        }
    }
    private static string _language = DefaultLanguage;

    private const string DefaultLanguage = "en-EN";
    
    private static Dictionary<string, string> _translations;
    private static Dictionary<string, string> _sourceEnglish;
    private static List<Dictionary<string, string>> _fallbackTranslations;

    private static string _resourcePath;

    private static List<string> _languageList;
    private static List<string> _languageNativeNames;
    public static IEnumerable<string> LanguageCodeNamePair => _languageList.Select((string code, int index) => $"{_languageNativeNames[index]} ({code})").ToArray();

    public static void Initialize()
    {
        // Add English first.
        _languageList = new List<string> { DefaultLanguage };
        _languageNativeNames = new List<string> { SourceEnglish?["LANGUAGE_NAME"] }; 

        var paths = Directory.GetFiles(ResourcePath, "*.tsv");
        
        foreach (var path in paths)
        {
            string nativeName = null;
            
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var reader = new StreamReader(fileStream))
            {
                while (nativeName is null && !reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line?.Split('\t')[0].Trim('"') == "LANGUAGE_NAME")
                    {
                        nativeName = line.Split('\t')[1].Trim('"');
                    }
                }
                _languageNativeNames.Add(nativeName);
            }
            string code = Path.GetFileNameWithoutExtension(path);
            _languageList.Add(code);
        }
        
        LocaleManager.eventLocaleChanged += OnGameLocaleChanged;
    }

    public static void OnSettingsUI()
    {
        OnGameLocaleChanged();
    }
    
    /// <summary>
    /// Translate to the current language by given key.
    /// Attempt to find translation in order of current language, available fallbacks, and English.
    /// </summary>
    /// <param name="key">Key to translate (in <see cref="KeyStrings"/>)</param>
    /// <returns>Translated text</returns>
    public static string T(string key)
    {
        if (_translations?.ContainsKey(key) ?? false)
        {
            return _translations[key];
        }

        // Try available fallbacks.
        if (FallBackTranslations != null)
        {
            foreach (var fallbackTranslation in FallBackTranslations)
            {
                if (fallbackTranslation.ContainsKey(key))
                {
                    return fallbackTranslation[key];
                }
            }
        }
        
        // Try English source.
        if (SourceEnglish != null)
        {
            if (SourceEnglish.ContainsKey(key))
            {
                return SourceEnglish[key];
            }
        }

        // No translation at all - returns the key
        return key;
    }

    /// <summary>
    /// Index used for language selection. Consider that 0 is 'Use game language' in the settings dropdown.
    /// </summary>
    public static int Index
    {
        get
        {
            if (UseGameLanguage) return 0;
            return _languageList.FindIndex(code => code == Language) + 1;
        }
        set
        {
            if (value < 1)
            {
                UseGameLanguage = true;
                OnGameLocaleChanged();
                UpdateSettingsUI();
            }
            else
            {
                UseGameLanguage = false;
                Language = _languageList[value - 1];
                UpdateAll();
            }
            XMLUtils.Save<ModSettings>();
        }
    }
    
    /// <summary>
    /// Get translation dictionary from tsv file in resource path.
    /// </summary>
    /// <param name="code">language name code</param>
    /// <returns>Translation dictionary, or null if read fails.</returns>
    private static Dictionary<string, string> GetTranslation(string code)
    {
        var path = Path.Combine(ResourcePath, code + ".tsv");
        
        try
        {
            var lines = File.ReadAllLines(path);
            return lines.ToDictionary(key => key.Split('\t')[0].Trim('"'), value => value.Split('\t')[1].Trim('"'));
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return null;
        }
    }

    private static Dictionary<string, string> SourceEnglish
    {
        get
        {
            if (_sourceEnglish != null)
            {
                return _sourceEnglish;
            }

            string fileName = Assembly.GetExecutingAssembly().GetName().Name + "." + nameof(Translation) + "." + "Source.tsv";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);

            if (stream == null)
            {
                Debug.LogWarning("No source string found.");
                return null;
            }
            
            _sourceEnglish = new Dictionary<string, string>();
            using var reader = new StreamReader(stream);
            
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) break;
                _sourceEnglish.Add(line.Split('\t')[0].Trim('"'), line.Split('\t')[1].Trim('"'));
            }

            return _sourceEnglish;
        }
    }
    
    private static List<Dictionary<string, string>> FallBackTranslations
    {
        get
        {
            if (_fallbackTranslations != null)
            {
                return _fallbackTranslations;
            }

            _fallbackTranslations = new List<Dictionary<string, string>>();

            // Add available fallbacks.
            var shortCode = Language.Substring(0, 2);
            foreach (var code in _languageList)
            {
                if (code.Substring(0, 2) == shortCode)
                {
                    _fallbackTranslations.Add(GetTranslation(code));
                }
            }

            if (!_fallbackTranslations.Any())
            {
                _fallbackTranslations = null;
            }

            return _fallbackTranslations;
        }
    }
    
    private static string ResourcePath
    {
        get
        {
            if (_resourcePath != null)
            {
                return _resourcePath;
            }
            
            var thisMod = PluginManager.instance.FindPluginInfo(Assembly.GetExecutingAssembly());
            
            if (thisMod is null)
            {
                var plugins = PluginManager.instance.GetPluginsInfo();
                foreach (var plugin in plugins)
                {
                    if (plugin.GetAssemblies().Any(assembly => assembly == Assembly.GetExecutingAssembly()))
                    {
                        thisMod = plugin;
                        break;
                    }
                }
            }
            if (thisMod is null) return null;

            return _resourcePath = Path.Combine(thisMod.modPath, "Translations");
        }
    }

    private static void OnGameLocaleChanged()
    {
        if (!UseGameLanguage
            || !LocaleManager.exists
            || LocaleManager.instance.language == Language.Substring(0, 2))
        {
            return;
        }

        Language = _languageList
                       .Where(code => LocaleManager.instance.supportedLocaleIDs.Contains(code.Substring(0, 2)))
                       .FirstOrDefault(code => code.Substring(0, 2) == LocaleManager.instance.language)
                   ?? DefaultLanguage;
        
        UpdateCustomUI();
    }

    private static void UpdateAll()
    {
        UpdateSettingsUI();
        UpdateCustomUI();
    }
    
    private static void UpdateSettingsUI()
    {
        SettingsUI.LocaleChanged();
    }

    private static void UpdateCustomUI()
    {
        InGameUIManager.Destroy();
        InGameUIManager.ShowModButton = InGameUIManager.ShowModButton;
    }
}

