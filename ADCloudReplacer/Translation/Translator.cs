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
    
    private static string _language = "en";

    private static Dictionary<string, string> _translations;
    private static List<Dictionary<string, string>> _fallbackTranslations;

    private static string[] _languageList = {};
    private static string[] _nativeLanguageNames = {};

    public static void Initialize()
    {
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
                        nativeName = line.Split('\t')[1];
                    }
                }
                _nativeLanguageNames = _nativeLanguageNames.Concat(new string[] { nativeName }).ToArray();
            }
            string code = Path.GetFileNameWithoutExtension(path);
            _languageList = _languageList.Concat(new string[] { code }).ToArray();
        }
        
        LocaleManager.eventLocaleChanged += OnGameLocaleChanged;
    }
    
    [BackingForSerialize(nameof(ModSettings.Language))]
    public static string Language
    {
        get => _language;
        set
        {
            _language = value;
            _translations = GetTranslation(value);
        }
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

        // Translation not found - load the fallbacks.
        if (_fallbackTranslations == null)
        {
            LoadFallbackTranslation();
        }

        // Try available fallbacks.
        if (_fallbackTranslations!.Count > 1)
        {
            foreach (var fallbackTranslation in _fallbackTranslations.Skip(1))
            {
                if (fallbackTranslation.ContainsKey(key))
                {
                    return fallbackTranslation[key];
                }
            }
        }
        
        // Try English
        if (_fallbackTranslations.FirstOrDefault() is {  } fallbackEnglish)
        {
            if (fallbackEnglish.ContainsKey(key))
            {
                return fallbackEnglish[key];
            }
        }
        
        // No translation at all - returns the key
        return key;
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

    /// <summary>
    /// Index used for language selection. Consider that 0 is 'Use game language' in the settings dropdown.
    /// </summary>
    public static int Index
    {
        get
        {
            if (UseGameLanguage) return 0;
            return Array.FindIndex(_languageList, code => code == Language) + 1;
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
    
    public static string[] LanguageCodeNamePair => _languageList.Select((string code, int index) => $"{_nativeLanguageNames[index]} ({code})").ToArray();

    private static string ResourcePath => Path.Combine(PluginManager.instance.FindPluginInfo(Assembly.GetExecutingAssembly()).modPath, "Translations");
    
    private static void LoadFallbackTranslation()
    {
        _fallbackTranslations = new List<Dictionary<string, string>> {
            // Add English to the first element.
            GetTranslation("en") };

        // Add available fallbacks.
        var shortCode = Language.Substring(0, 2);
        foreach (var code in _languageList)
        {
            if (code.Substring(0, 2) == shortCode)
            {
                _fallbackTranslations.Add(GetTranslation(code));
            }
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
                   ?? "en";
        
        UpdateCustomUI();
    }

    private static void UpdateAll()
    {
        UpdateSettingsUI();
        UpdateCustomUI();
    }
    
    private static void UpdateSettingsUI()
    {
        //TODO:
    }

    private static void UpdateCustomUI()
    {
        //TODO:
    }
}

