using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ADCloudReplacer
{
    public static class CloudLists
    {
        public const string VanillaIdentifier = ".Vanilla";
        public const string CloudsFolderName = "ADClouds";
        public const int VanillaIndex = 0;
        
        public static readonly string LocalDirPath = Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, CloudsFolderName);
        
        [BackingForSerialize(nameof(ModSettings.SelectedCloud))]
        public static string SelectedCloud = VanillaIdentifier;
        
        private static bool _globalCached = false;
        private static bool _internalCached = false;
        
        private static string[] _localFilePaths;
        private static string[] _workshopFilePaths;
        private static string[] _cloudFilePaths;
        private static string[] _cloudNamesForDisplay;
        private static string[] _cloudNamesForSave;

        public static string[] CloudFilePaths
        {
            get
            {
                if (_globalCached || _internalCached)
                {
                    return _cloudFilePaths;
                }
                
                string[] vanilla = { "VANILLA_PLACEHOLDER" };
                _cloudFilePaths = vanilla
                    .Concat(LocalFilePaths ?? Enumerable.Empty<string>())
                    .Concat(WorkshopFilePaths ?? Enumerable.Empty<string>())
                    .ToArray();
                _internalCached = true;
                return _cloudFilePaths;
            }
        }

        public static string[] CloudNamesForDisplay
        {
            get
            {
                if (_globalCached)
                {
                    return _cloudNamesForDisplay;
                }
                
                string[] vanilla = { (ADCloudReplacer.OriginalCloudMaterial?.name ?? String.Empty) + " (Vanilla)" };
                _cloudNamesForDisplay = vanilla
                    .Concat(CloudFilePaths.Skip(1).Select(Path.GetFileNameWithoutExtension))
                    .ToArray();
                return _cloudNamesForDisplay;
            }
        }

        public static string[] CloudNamesForSave
        {
            get
            {
                if (_globalCached)
                {
                    return _cloudNamesForSave;
                }
                
                string[] vanilla = { VanillaIdentifier };
                _cloudNamesForSave = vanilla
                    .Concat(LocalFilePaths.Select(Path.GetFileNameWithoutExtension))
                    .Concat(WorkshopFilePaths.Select(file => Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(file))) + "." + Path.GetFileNameWithoutExtension(file)) )
                    .ToArray();
                return _cloudNamesForSave;
            }
        }
        
        /// <summary>
        /// Get an index by the name of the currently selected(saved) cloud.
        /// </summary>
        /// <returns> Found index. if cannot found(-1) or is vanilla index(0) then vanilla index(0). </returns>
        public static int Index 
        {
            get
            {
                int index = Array.FindIndex(CloudNamesForSave, name => name == SelectedCloud);
                return index < 1 ? VanillaIndex : index;
            }
        }

        public static bool Cached
        {
            set
            {
                _globalCached = value;
                _internalCached = value;
            }
        }
        
        private static string[] LocalFilePaths
        {
            get
            {
                if (_internalCached && _localFilePaths != null)
                {
                    return _localFilePaths;
                }
                
                if (Directory.Exists(LocalDirPath))
                {
                    _localFilePaths = Directory.GetFiles(LocalDirPath, "*.png");
                    return _localFilePaths;
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(LocalDirPath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    return null;
                }
            }
        }
        
        private static string[] WorkshopFilePaths
        {
            get
            {
                if (_internalCached && _workshopFilePaths != null)
                {
                    return _workshopFilePaths;
                }
                
                _workshopFilePaths = new string[] {};
                
                foreach (var pluginInfo in ColossalFramework.Plugins.PluginManager.instance.GetPluginsInfo())
                {
                    if (Directory.Exists(Path.Combine(pluginInfo.modPath, CloudsFolderName)))
                    {
                        _workshopFilePaths = _workshopFilePaths.Concat(Directory.GetFiles(Path.Combine(pluginInfo.modPath, CloudsFolderName), "*.png")).ToArray();
                    }
                }
                
                return _workshopFilePaths;
            }
        }
    }
}
