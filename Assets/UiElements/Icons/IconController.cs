//
// Copyright (C) Microsoft. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace RemoteAssist.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TMP_Text))]
    public class IconController : MonoBehaviour
    {
#if (UNITY_EDITOR)
        // Add another entry to the end of this list whenever a new icon is added
        // Don't reorder the enum. The IconControllerEditor will alphabetize the list for users
        public enum IconKey
        {
            [IconCharacterMapping("")]
            Add,
            [IconCharacterMapping("A", true)]
            Arrow_annotate,
            [IconCharacterMapping("")]
            Arrow_next,
            [IconCharacterMapping("")]
            Arrow_previous,
            [IconCharacterMapping("")]
            At,
            [IconCharacterMapping("J", true)]
            Back,
            [IconCharacterMapping("")]
            Backspace,
            [IconCharacterMapping("")]
            Call,
            [IconCharacterMapping("")]
            Calendar,
            [IconCharacterMapping("")]
            Capture,
            [IconCharacterMapping("B", true)]
            Chat_cutout,
            [IconCharacterMapping("")]
            Chat_dot,
            [IconCharacterMapping("")]
            Chat_lines,
            [IconCharacterMapping("")]
            Chat_nolines,
            [IconCharacterMapping("")]
            Chats,
            [IconCharacterMapping("")]
            Check,
            [IconCharacterMapping("")]
            Chevron_down,
            [IconCharacterMapping("")]
            Chevron_left,
            [IconCharacterMapping("")]
            Chevron_right,
            [IconCharacterMapping("")]
            Chevron_up,
            [IconCharacterMapping("")]
            Close,
            [IconCharacterMapping("")]
            Cloud_download,
            [IconCharacterMapping("")]
            Cloud_upload,
            [IconCharacterMapping("I", true)]
            Colorpicker,
            [IconCharacterMapping("")]
            Contacts,
            [IconCharacterMapping("")]
            Copy,
            [IconCharacterMapping("")]
            Copy_sdf,
            [IconCharacterMapping("")]
            Delete,
            [IconCharacterMapping("C", true)]
            Dock,
            [IconCharacterMapping("D", true)]
            Dock_chat,
            [IconCharacterMapping("")]
            Dynamics,
            [IconCharacterMapping("M", true)]
            Endcall,
            [IconCharacterMapping("")]
            Erase,
            [IconCharacterMapping("")]
            Error_badge,
            [IconCharacterMapping("E", true)]
            External_link,
            [IconCharacterMapping("")]
            Files,
            [IconCharacterMapping("")]
            Folder,
            [IconCharacterMapping("")]
            Folder1,
            [IconCharacterMapping("")]
            Help,
            [IconCharacterMapping("")]
            Hololens,
            [IconCharacterMapping("")]
            Image,
            [IconCharacterMapping("")]
            Incident_triangle,
            [IconCharacterMapping("")]
            Info,
            [IconCharacterMapping("")]
            Leave_chat,
            [IconCharacterMapping("")]
            Link,
            [IconCharacterMapping("")]
            Locked,
            [IconCharacterMapping("")]
            Mic,
            [IconCharacterMapping("F", true)]
            Mic_mute,
            [IconCharacterMapping("")]
            Minimize,
            [IconCharacterMapping("")]
            Move,
            [IconCharacterMapping("H", true)]
            Onedrive,
            [IconCharacterMapping("")]
            Participantlist,
            [IconCharacterMapping("")]
            Pen,
            [IconCharacterMapping("")]
            Pen_palette,
            [IconCharacterMapping("")]
            Pencil,
            [IconCharacterMapping("")]
            Penworkspace,
            [IconCharacterMapping("")]
            People_add,
            [IconCharacterMapping("")]
            Person_add,
            [IconCharacterMapping("")]
            Phone,
            [IconCharacterMapping("")]
            Pin,
            [IconCharacterMapping("")]
            Pin_sideways,
            [IconCharacterMapping("")]
            Poor_wifi,
            [IconCharacterMapping("")]
            Ruler,
            [IconCharacterMapping("")]
            Scale,
            [IconCharacterMapping("")]
            Search,
            [IconCharacterMapping("")]
            Search_user,
            [IconCharacterMapping("")]
            Security_alert,
            [IconCharacterMapping("")]
            Send,
            [IconCharacterMapping("")]
            Settings,
            [IconCharacterMapping("")]
            Share,
            [IconCharacterMapping("")]
            Status_onbreak,
            [IconCharacterMapping("")]
            Status_committed,
            [IconCharacterMapping("")]
            Status_error,
            [IconCharacterMapping("")]
            Status_generic,
            [IconCharacterMapping("")]
            Status_inprogress,
            [IconCharacterMapping("")]
            Status_scheduled,
            [IconCharacterMapping("")]
            Status_traveling,
            [IconCharacterMapping("G", true)]
            Switch_user,
            [IconCharacterMapping("")]
            Three_dimensional_file,
            [IconCharacterMapping("")]
            Undo,
            [IconCharacterMapping("")]
            Undock,
            [IconCharacterMapping("")]
            Unshift_left,
            [IconCharacterMapping("")]
            Unshift_right,
            [IconCharacterMapping("")]
            Uptotop,
            [IconCharacterMapping("K", true)]
            Video,
            [IconCharacterMapping("L", true)]
            Video_mute,
            [IconCharacterMapping("")]
            Warning,
            [IconCharacterMapping("")]
            Wifi_1,
            [IconCharacterMapping("")]
            Wifi_2,
            [IconCharacterMapping("")]
            Wifi_3,
            [IconCharacterMapping("")]
            Wifi_4,
            [IconCharacterMapping("")]
            FreezeMode,
            [IconCharacterMapping("")]
            CaptureAnnotate,
            [IconCharacterMapping("")]
            Record_InnerDot,
            [IconCharacterMapping("")]
            Record_OuterRing,
            [IconCharacterMapping("")]
            Volume,
            [IconCharacterMapping("")]
            Sync,
        }

        public static IReadOnlyDictionary<IconKey, IconCharacterMapping> IconsMap { get; }

        [SerializeField]
        private IconKey icon;
        public IconKey Icon
        {
            get => icon;
            set
            {
                if(icon != value)
                {
                    icon = value;
                    UpdateIconText();
                }
            }
        }

        [SerializeField]
        private TMP_FontAsset officialIconsFont;
        [SerializeField]
        private TMP_FontAsset customIconsFont;

        private TMP_Text textMeshComponent;

        static IconController()
        {
            IconsMap = CreateIconMap();
        }

        private static Dictionary<IconKey, IconCharacterMapping> CreateIconMap()
        {
            Type iconKeyType = typeof(IconKey);
            Dictionary<IconKey, IconCharacterMapping> ret = new Dictionary<IconKey, IconCharacterMapping>();
            foreach (IconKey iconKey in Enum.GetValues(iconKeyType))
            {
                string iconName = Enum.GetName(iconKeyType, iconKey);
                FieldInfo field = iconKeyType.GetField(iconName);
                try
                {
                    IconCharacterMapping mapping = (IconCharacterMapping)Attribute.GetCustomAttribute(field, typeof(IconCharacterMapping));
                    ret.Add(iconKey, mapping);
                }
                catch (TypeLoadException)
                {
                    Debug.LogError("No IconCharacterMapping for " + iconName + ". Add an IconCharacterMapping attribute to the enum field in IconController.cs.");
                }
            }
            return ret;
        }

        void Start()
        {
            textMeshComponent = GetComponent<TMP_Text>();
        }

        private TMP_FontAsset GetFontAsset(string name)
        {
            string assetGuid = AssetDatabase.FindAssets(name).FirstOrDefault();

            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
        }

        public void RestoreBinding()
        {
            if (!string.IsNullOrWhiteSpace(textMeshComponent.text))
            {
                foreach (var item in IconsMap)
                {
                    if (textMeshComponent.text == item.Value.IconCharacter)
                    {
                        Icon = item.Key;
                    }
                }
            }
        }

        private void UpdateIconText()
        {
#if(UNITY_EDITOR)
            Undo.RecordObject(textMeshComponent, "Icon Change");
#endif
            IconCharacterMapping mapping = IconsMap[Icon];
            textMeshComponent.font = mapping.IsCustom ? GetCustomIconFont() : GetOfficialIconFont();
            textMeshComponent.text = mapping.IconCharacter;
#if (UNITY_EDITOR)
            PrefabUtility.RecordPrefabInstancePropertyModifications(textMeshComponent);
#endif
        }

        private TMP_FontAsset GetCustomIconFont()
        {
            if (customIconsFont == null)
            {
                customIconsFont = GetFontAsset("CustomIconsFont");
                if(customIconsFont == null)
                {
                    Debug.Log("Can't set icon for IconController because CustomIconsFont is not set.");
                    return null;
                }
            }
            return customIconsFont;
        }

        private TMP_FontAsset GetOfficialIconFont()
        {
            if (officialIconsFont == null)
            {
                officialIconsFont = GetFontAsset("OfficialIconsFont");
                if(officialIconsFont == null)
                {
                    Debug.Log("Can't set icon for IconController because OfficialIconsFont is not set.");
                    return null;
                }
            }
            return officialIconsFont;
        }

        public class IconCharacterMapping : System.Attribute
        {
            public bool IsCustom { get; }
            public string IconCharacter { get; }

            public IconCharacterMapping(string iconCharacter, bool isCustom = false)
            {
                IsCustom = isCustom;
                IconCharacter = iconCharacter;
            }
        }
#endif
    }
}