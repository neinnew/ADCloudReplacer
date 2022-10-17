global using ModSprites = ADCloudReplacer.TextureAtlasUtils.SpriteNames;
global using ModAtlases = ADCloudReplacer.TextureAtlasUtils.AtlasNames;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework.UI;
using UnityEngine;

namespace ADCloudReplacer;

public static class TextureAtlasUtils
{
    #region Mod Specific
    // TODO: Maybe I should have this as a separate class?
    
    public static class AtlasNames
    {
        public const string ADCloudReplacerAtlas = "ADCloudReplacerAtlas";
        public const string MinimizeButtonAtlas = "MinimizeButtonAtlas";
    }
    
    public static class SpriteNames
    {
        public const string ADCloudReplacerIcon = "ADCloudReplacerIcon";
        
        public const string MinimizeNormal = "MinimizeNormal";
        public const string MinimizeHovered = "MinimizeHovered";
        public const string MinimizePressed = "MinimizePressed";
    }

    static TextureAtlasUtils()
    {
        CreateTextureAtlas(AtlasNames.ADCloudReplacerAtlas, new string[] { SpriteNames.ADCloudReplacerIcon }, ResourceAssemblyPath);
        CreateTextureAtlas(AtlasNames.MinimizeButtonAtlas, new string[] { SpriteNames.MinimizeNormal, SpriteNames.MinimizeHovered, SpriteNames.MinimizePressed }, ResourceAssemblyPath);
    }

    private const string ResourceAssemblyPath = "ADCloudReplacer.Sprites.";

    #endregion
    
    /// Cache the atlas created in this mod.
    private static readonly List<UITextureAtlas> ModAtlases = new List<UITextureAtlas>();
    
    public static UITextureAtlas InGameAtlas => _inGameAtlas ??= FindTextureAtlas("Ingame");
    private static UITextureAtlas _inGameAtlas;
    
    public static UITextureAtlas InAssetImporterAtlas => _inAssetImporterAtlas ??= FindTextureAtlas("InAssetImporter");
    private static UITextureAtlas _inAssetImporterAtlas;

    public static Texture2D LoadTextureFromAssembly(string path)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
        var array = new byte[stream.Length];
        stream.Read(array, 0, array.Length);
        var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture.LoadImage(array);
        
        return texture;
    }
    
    public static UITextureAtlas FindTextureAtlas(string name) 
        => ModAtlases.Find(atlas => atlas.name == name) 
           ?? Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(atlas => atlas.name == name);
    
    public static UITextureAtlas CreateTextureAtlas(string atlasName, string[] spriteNames, string AssemblyPath)
    {
        int maxSize = 1024;
        var texture2D = new Texture2D(maxSize, maxSize, TextureFormat.ARGB32, false);
        var textures = new Texture2D[spriteNames.Length];

        for (int i = 0; i < spriteNames.Length; i++)
        {
            textures[i] = LoadTextureFromAssembly(AssemblyPath + spriteNames[i] + ".png");
        }

        Rect[] regions = texture2D.PackTextures(textures, 2, maxSize);

        UITextureAtlas textureAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
        Material material = Object.Instantiate(UIView.GetAView().defaultAtlas.material);
        material.mainTexture = texture2D;
        textureAtlas.material = material;
        textureAtlas.name = atlasName;

        for (int i = 0; i < spriteNames.Length; i++)
        {
            UITextureAtlas.SpriteInfo item = new UITextureAtlas.SpriteInfo
            {
                name = spriteNames[i],
                texture = textures[i],
                region = regions[i],
            };
            textureAtlas.AddSprite(item);
        }

        ModAtlases.Add(textureAtlas);
        return textureAtlas;
    }
}