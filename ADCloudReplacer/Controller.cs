using System;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
namespace ADCloudReplacer
{
    public static class Controller
    {
     
        public enum ControlModes
        {
            Vanilla, Custom
        }

        [BackingForSerialize(nameof(ModSettings.ControlMode))]
        public static ControlModes ControlMode = ControlModes.Vanilla;

        private static Material m_Material;

        private static int ID_AlphaSaturation = Shader.PropertyToID("_AlphaSaturation");
        private static int ID_Attenuation = Shader.PropertyToID("_Attenuation");
        private static int ID_StepSize = Shader.PropertyToID("_StepSize");
        private static int ID_RotateSpeed = Shader.PropertyToID("_RotateSpeed");
        private static int ID_SkyColorMultiplier = Shader.PropertyToID("_SkyColorMultiplier");
        private static int ID_SunColorMultiplier = Shader.PropertyToID("_SunColorMultiplier");

        /*
        private static int ID_BumpScale = Shader.PropertyToID("_BumpScale");
        private static int ID_Cutoff = Shader.PropertyToID("_Cutoff");
        private static int ID_DetailNormalMapScale = Shader.PropertyToID("_DetailNormalMapScale");
        private static int ID_DstBlend = Shader.PropertyToID("_DstBlend");
        private static int ID_EmissionScaleUI = Shader.PropertyToID("_EmissionScaleUI");
        private static int ID_Glossiness = Shader.PropertyToID("_Glossiness");
        private static int ID_Mask = Shader.PropertyToID("_Mask");
        private static int ID_Metallic = Shader.PropertyToID("_Metallic");
        private static int ID_Mode = Shader.PropertyToID("_Mode");
        private static int ID_OcclusionStrength = Shader.PropertyToID("_OcclusionStrength");
        private static int ID_Parallax = Shader.PropertyToID("_Parallax");
        private static int ID_SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static int ID_UVSec = Shader.PropertyToID("_UVSec");
        private static int ID_ZWrite = Shader.PropertyToID("_ZWrite");
        private static int ID_Color = Shader.PropertyToID("_Color");
        private static int ID_EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static int ID_EmissionColorUI = Shader.PropertyToID("_EmissionColorUI");
        */

        private static float? m_AlphaSaturation;
        private static float? m_Attenuation;
        private static float? m_StepSize;
        private static float? m_RotateSpeed;
        private static float? m_SkyColorMultiplier;
        private static float? m_SunColorMultiplier;


        public static void Initialize()
        {
            m_Material = ADCloudReplacer.CloudMaterial;
        }

        [BackingForSerialize(nameof(GlobalPropertiesConfig.AlphaSaturation))]
        public static float? AlphaSaturation 
        {
            get { return m_AlphaSaturation; }
            set { m_AlphaSaturation = value; m_Material?.SetFloat(ID_AlphaSaturation, value ?? m_Material.GetFloat(ID_AlphaSaturation)); }
        }

        [BackingForSerialize(nameof(GlobalPropertiesConfig.Attenuation))]
        public static float? Attenuation
        {
            get { return m_Attenuation; }
            set { m_Attenuation = value; m_Material?.SetFloat(ID_Attenuation, value ?? m_Material.GetFloat(ID_Attenuation)); }
        }

        [BackingForSerialize(nameof(GlobalPropertiesConfig.StepSize))]
        public static float? StepSize
        {
            get { return m_StepSize; }
            set { m_StepSize = value; m_Material?.SetFloat(ID_StepSize, value ?? m_Material.GetFloat(ID_StepSize)); }
        }

        [BackingForSerialize(nameof(GlobalPropertiesConfig.RotateSpeed))]
        public static float? RotateSpeed
        {
            get { return m_RotateSpeed; }
            set { m_RotateSpeed = value; m_Material?.SetFloat(ID_RotateSpeed, value ?? m_Material.GetFloat(ID_RotateSpeed)); }
        }

        [BackingForSerialize(nameof(GlobalPropertiesConfig.SkyColorMultiplier))]
        public static float? SkyColorMultiplier
        {
            get { return m_SkyColorMultiplier; }
            set { m_SkyColorMultiplier = value; m_Material?.SetFloat(ID_SkyColorMultiplier, value ?? m_Material.GetFloat(ID_SkyColorMultiplier)); }
        }
        
        [BackingForSerialize(nameof(GlobalPropertiesConfig.SunColorMultiplier))]
        public static float? SunColorMultiplier
        {
            get { return m_SunColorMultiplier; }
            set { m_SunColorMultiplier = value; m_Material?.SetFloat(ID_SunColorMultiplier, value ?? m_Material.GetFloat(ID_SunColorMultiplier)); }
        }

        public static void RevertVanilla()
        {
            var vanillaMaterial = ADCloudReplacer.OriginalCloudMaterial;
            if (vanillaMaterial == null) return;

            AlphaSaturation = vanillaMaterial.GetFloat(ID_AlphaSaturation);
            Attenuation = vanillaMaterial.GetFloat(ID_Attenuation);
            StepSize = vanillaMaterial.GetFloat(ID_StepSize);
            RotateSpeed = vanillaMaterial.GetFloat(ID_RotateSpeed);
            SkyColorMultiplier = vanillaMaterial.GetFloat(ID_SkyColorMultiplier);
            SunColorMultiplier = vanillaMaterial.GetFloat(ID_SunColorMultiplier);
        }
    }
}
