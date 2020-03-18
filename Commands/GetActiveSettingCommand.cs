namespace Plugin.Sample.Settings.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Entities;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class GetActiveSettingCommand : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public GetActiveSettingCommand(CommerceCommander commerceCommander, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _commerceCommander = commerceCommander;
        }

        public async Task<Setting> Process(CommerceContext commerceContext)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                CommerceList<Setting> settings = await _commerceCommander.Command<FindEntitiesInListCommand>().Process<Setting>(commerceContext, CommerceEntity.ListName<Setting>(), 0, int.MaxValue);
                return settings.Items.FirstOrDefault(element => element.IsActive);
            }
        }
    }
}