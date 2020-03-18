namespace Plugin.Sample.Settings.Commands
{
    using System;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Entities;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class ActivateSettingCommand : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public ActivateSettingCommand(CommerceCommander commerceCommander, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _commerceCommander = commerceCommander;
        }

        public async Task<bool> Process(CommerceContext commerceContext, string id)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                Setting setting = await _commerceCommander.Command<GetSettingCommand>().Process(commerceContext, id);
                if (setting == null)
                {
                    return false;
                }

                setting.IsActive = true;
                await _commerceCommander.PersistEntity(commerceContext, setting);

                CommerceList<Setting> settings = await _commerceCommander.Command<FindEntitiesInListCommand>().Process<Setting>(commerceContext, CommerceEntity.ListName<Setting>(), 0, int.MaxValue);
                foreach (var listSetting in settings.Items)
                {
                    if (listSetting.Id.Equals(setting.Id))
                    {
                        continue;
                    }

                    listSetting.IsActive = false;
                    await _commerceCommander.PersistEntity(commerceContext, listSetting);
                }
            }

            return true;
        }
    }
}