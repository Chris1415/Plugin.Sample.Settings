namespace Plugin.Sample.Settings.Policies
{
    using Sitecore.Commerce.Core;

    public class SettingsUiPolicy : Policy
    {
        public string ViewName { get; set; }
        public string Icon { get; set; }
        public string ActivateSettingActionName { get; set; }
        public string DeactivateSettingActionName { get; set; }
        public string BasicDataSettingsViewName { get; set; }
        public string DeleteSettingActionName { get; set; }
        public string AddSettingActionName { get; set; }

        public SettingsUiPolicy()
        {
            ViewName = "Settings";
            Icon = "edit";
            ActivateSettingActionName = "ActivateSetting";
            DeactivateSettingActionName = "DeactivateSetting";
            BasicDataSettingsViewName = "SettingsBasicData";
            AddSettingActionName = "AddSetting";
            DeleteSettingActionName = "DeleteSetting";
        }
    }
}
