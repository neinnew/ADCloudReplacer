using System;
using System.IO;
using UnityEngine;

namespace ADCloudReplacer
{
    /// <summary>
    /// Main mod class.
    /// </summary>
    public static class ADCloudReplacer
    {
        [BackingForSerialize(nameof(ModSettings.ADCloudEnabled))]
        public static bool ADCloudEnabled = true;
        
        public static Material CloudMaterial => m_CloudMaterial;
        public static Material OriginalCloudMaterial => m_OriginalCloudMaterial; 

        private static int ID_CloudSampler = Shader.PropertyToID("_CloudSampler");
        private static DayNightCloudsProperties m_DayNightCloudsProperties;
        private static Texture2D m_CloudTexture = new Texture2D(1, 1);

        private static Material m_CloudMaterial;
        private static Material m_OriginalCloudMaterial;

        public static void Initialize()
        {
            m_DayNightCloudsProperties = UnityEngine.Object.FindObjectOfType<DayNightCloudsProperties>();
            m_CloudMaterial = m_DayNightCloudsProperties.m_CloudMaterial;
            m_OriginalCloudMaterial = new Material(m_CloudMaterial);
            if(!IsVanillaSelected()) LoadCloudTexture(CloudLists.CloudFilePaths[CloudLists.Index]);
            ToggleADCloud();
        }

        public static void LoadCloudTexture(string path)
        {
            m_CloudTexture.LoadImage(File.ReadAllBytes(path));
        }

        public static void ToggleADCloud()
        {
            if (m_DayNightCloudsProperties == null) return;
            
            m_DayNightCloudsProperties.enabled = ADCloudEnabled;
            if (!IsVanillaSelected()) Apply();
        }

        public static void Apply()
        {
            if (m_CloudMaterial == null) return;
            
            m_CloudMaterial.SetTexture(ID_CloudSampler, m_CloudTexture);
        }

        public static void RevertVanilla()
        {
            if (m_CloudMaterial == null) return;

            var material = new Material(m_OriginalCloudMaterial);
            var texture = material.GetTexture(ID_CloudSampler);
            m_CloudMaterial.SetTexture(ID_CloudSampler, texture);
            UnityEngine.Object.Destroy(material);
        }

        private static bool IsVanillaSelected()
        {
            return CloudLists.Index == CloudLists.VanillaIndex;
        }
    }
}
