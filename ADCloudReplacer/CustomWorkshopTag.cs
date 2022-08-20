using System;
using System.IO;
using System.Reflection;
using ColossalFramework.UI;
using ColossalFramework.PlatformServices;
using UnityEngine;

namespace ADCloudReplacer
{
    
    /// <summary>
    /// Class for adding 'AD Clouds' tag to workshop item. Implemented without Harmony.
    /// Side effect: Workshop update occurs twice. (Of course, only for AD clouds pack mods)
    /// </summary>
    public static class CustomWorkshopTag
    {
        private const string CustomTagString = "AD Clouds";
        
        private static WorkshopModUploadPanel InstanceWorkshopModUploadPanel => UIView.GetAView().FindUIComponent<UIComponent>("WorkshopModUploadPanel(Clone)").GetComponent<WorkshopModUploadPanel>();

        public static void Initialize()
        {
            using (var topLevelComponents = typeof(UIView)
                  .GetMethod("GetTopLevelComponents", BindingFlags.NonPublic | BindingFlags.Instance)
                  ?.Invoke(UIView.GetAView(), null)
                  as ColossalFramework.PoolList<UIComponent>)
            {
                foreach (var uiComponent in topLevelComponents)
                {
                    if (uiComponent.name == "(Library) WorkshopModUploadPanel")
                    {
                        var modShareButton = uiComponent.Find<UIButton>("Share");
                        modShareButton.eventClick += OnShareCustom;
                    }
                }
            }
            PlatformService.workshop.eventCreateItem += OnItemCreatedCustom;
        }

        /// <summary>
        /// modified of <see cref="WorkshopModUploadPanel.OnShare"/>. 
        /// </summary>
        static void OnShareCustom(UIComponent component, UIMouseEventParameter p)
        {
            var m_PublishedFileId = (PublishedFileId)typeof(WorkshopModUploadPanel)
                .GetField("m_PublishedFileId", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(InstanceWorkshopModUploadPanel);
            
            if (m_PublishedFileId == PublishedFileId.invalid)
            {
                // We have nothing to do here.
            }
            else
            {
                // WorkshopModUploadPanel.UpdateItem() here in original method.
                AddADCloudsTag();
            }
        }

        /// <summary>
        /// modified of <see cref="WorkshopModUploadPanel.OnItemCreated"/>. 
        /// </summary>
        static void OnItemCreatedCustom(CreateItemResult res, bool ioError)
        {
            if (InstanceWorkshopModUploadPanel.component.isVisible && res.result == Result.OK)
            {
                // WorkshopModUploadPanel.UpdateItem() here in original method.
                AddADCloudsTag();
            }
        }

        /// <summary>
        /// Add "AD Clouds" to the tag when it have ADClouds Folder.
        /// context of <see cref="ColossalFramework.PlatformServices.Workshop.UpdateItem(PublishedFileId, string, string, string, string, string, string[])"/>
        /// </summary>
        static void AddADCloudsTag()
        {
            var m_ContentPath = (string)typeof(WorkshopModUploadPanel)
                .GetField("m_ContentPath", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(InstanceWorkshopModUploadPanel);
            
            // Don't do anything unless it's an AD Clouds pack.
            if (!Directory.Exists(Path.Combine(m_ContentPath, CloudLists.CloudsFolderName)))
            {
                return;
            };

            var m_PublishedFileId = (PublishedFileId)typeof(WorkshopModUploadPanel)
                .GetField("m_PublishedFileId", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(InstanceWorkshopModUploadPanel);
            
            var m_Title = (UITextField)typeof(WorkshopModUploadPanel)
                .GetField("m_Title", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(InstanceWorkshopModUploadPanel);
            
            var m_Desc = (UITextField)typeof(WorkshopModUploadPanel)
                .GetField("m_Desc", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(InstanceWorkshopModUploadPanel);
            
            var m_ChangeNote = (UITextField)typeof(WorkshopModUploadPanel)
                .GetField("m_ChangeNote", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(InstanceWorkshopModUploadPanel);
            
            var m_PreviewPath = (string)typeof(WorkshopModUploadPanel)
                .GetField("m_PreviewPath", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(InstanceWorkshopModUploadPanel);
            
            string text = m_Title.text;
            string text2 = m_Desc.text;
            string text3 = m_ChangeNote.text;
            
            try
            {
                var m_CurrentHandle = PlatformService.workshop.UpdateItem(m_PublishedFileId, text, text2, text3, m_PreviewPath, m_ContentPath, new string[2] { "Mod", CustomTagString});
            
                typeof(WorkshopModUploadPanel)
                    .GetField("m_CurrentHandle", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.SetValue(InstanceWorkshopModUploadPanel, m_CurrentHandle);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError(Mod.Instance.Name + ": Custom tag creation failed | " + e.Message);
            }
            
            
        }
    }
}
    