using System;

namespace Plugin.Sample.Settings.Attributes
{
    public class SettingsPolicyAttribute : Attribute
    {
        public SettingsPolicyAttribute(string viewName)
        {
            ViewName = viewName;
        }

        public string ViewName { get; set; }
    }
}
