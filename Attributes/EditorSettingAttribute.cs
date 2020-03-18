using System;

namespace Plugin.Sample.Settings.Attributes
{
    public class EditorSettingAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public EditorSettingAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
