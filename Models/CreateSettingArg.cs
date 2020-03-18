namespace Plugin.Sample.Settings.Models
{
    public class CreateSettingArg
    { 
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public CreateSettingArg(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}
