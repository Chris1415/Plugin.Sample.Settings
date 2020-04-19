using Plugin.Sample.Settings.Attributes;
using System;
using System.Linq;

namespace Plugin.Sample.Settings.Extensions
{
    public static class TypeExtensions
    {
        public static string GetViewName(this Type input)
        {
            var settingsPolicyAttributes = input.GetCustomAttributes(typeof(SettingsPolicyAttribute), true);
            object settingsPolicyAttribute = settingsPolicyAttributes.FirstOrDefault();
            if (settingsPolicyAttribute == null)
            {
                return string.Empty;
            }

            SettingsPolicyAttribute mappedSettingsPolicyAttribute = settingsPolicyAttribute as SettingsPolicyAttribute;

            return string.IsNullOrEmpty(mappedSettingsPolicyAttribute.ViewName)
                ? $"{input.Name}-View"
                : mappedSettingsPolicyAttribute.ViewName;
        }

        public static string GetActionName(this Type input)
        {
            return $"{input.Name}-Action";
        }
    }
}
