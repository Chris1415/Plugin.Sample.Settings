namespace Plugin.Sample.Settings.Commands
{
    using System;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Entities;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class DeactivateSettingCommand : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public DeactivateSettingCommand(CommerceCommander commerceCommander, IServiceProvider serviceProvider) : base(serviceProvider)
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

                setting.IsActive = false;
                await _commerceCommander.PersistEntity(commerceContext, setting);
            }

            return true;
        }
    }
}