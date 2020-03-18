namespace Plugin.Sample.Settings.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Entities;
    using Plugin.Sample.Settings.Models;
    using Plugin.Sample.Settings.Policies;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;

    public class ChildViewSettings : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;
        private const string DefaultSettingsEntity = "Default";

        public ChildViewSettings(
            IServiceProvider serviceProvider,
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this._commerceCommander = commerceCommander;
        }

        public async Task<EntityView> Process(CommerceContext commerceContext, EntityView entityView)
        {
            var settingsuiPolicy = commerceContext.GetPolicy<SettingsUiPolicy>();

            using (CommandActivity.Start(commerceContext, this))
            {
                try
                {
                    var view = new EntityView
                    {
                        EntityId = string.Empty,
                        ItemId = string.Empty,
                        DisplayName = settingsuiPolicy.ViewName,
                        Name = settingsuiPolicy.ViewName,
                        UiHint = "Table",
                        Icon = commerceContext.GetPolicy<SettingsUiPolicy>().Icon
                    };
                    entityView.ChildViews.Add(view);

                    CommerceList<Setting> settings = await _commerceCommander.Command<FindEntitiesInListCommand>().Process<Setting>(commerceContext, CommerceEntity.ListName<Setting>(), 0, int.MaxValue);
                    if (!settings.Items.Any())
                    {

                        Setting setting = await _commerceCommander.Command<GetSettingCommand>().Process(commerceContext, DefaultSettingsEntity);
                        if (setting == null)
                        {
                            setting = await _commerceCommander.Command<CreateSettingCommand>().Process(commerceContext, new CreateSettingArg(DefaultSettingsEntity, DefaultSettingsEntity));
                        }

                        settings = await _commerceCommander.Command<FindEntitiesInListCommand>().Process<Setting>(commerceContext, CommerceEntity.ListName<Setting>(), 0, int.MaxValue);
                    }

                    foreach (var setting in settings.Items)
                    {
                        var basicDataEntityView = new EntityView
                        {
                            EntityId = setting.Id,
                            ItemId = setting.Id,
                            DisplayName = setting.DisplayName,
                            Name = setting.Name,
                            Icon = commerceContext.GetPolicy<SettingsUiPolicy>().Icon
                        };

                        basicDataEntityView.Properties.Add(new ViewProperty { Name = "Name", RawValue = setting.Name, UiType = "EntityLink" });
                        basicDataEntityView.Properties.Add(new ViewProperty { Name = "Is Active", RawValue = setting.IsActive });

                        view.ChildViews.Add(basicDataEntityView);
                    }
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"ChildViewRunningMinions.Exception: Message={ex.Message}");
                }
                return null;
                //return entityView;
            }
        }
    }
}