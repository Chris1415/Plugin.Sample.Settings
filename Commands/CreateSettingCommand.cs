namespace Plugin.Sample.Settings.Commands
{
    using System;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Entities;
    using Plugin.Sample.Settings.Models;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.ManagedLists;

    public class CreateSettingCommand : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public CreateSettingCommand(CommerceCommander commerceCommander, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _commerceCommander = commerceCommander;
        }

        public async Task<Setting> Process(CommerceContext commerceContext, CreateSettingArg arg)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var setting = new Setting
                {
                    Id = arg.Name.ToEntityId<Setting>(),
                    DisplayName = arg.DisplayName,
                    Name = arg.Name,
                    IsActive = false
                };

                var listMemberShipComponent = setting.GetComponent<ListMembershipsComponent>();
                if (!listMemberShipComponent.Memberships.Contains(CommerceEntity.ListName<Setting>()))
                {
                    listMemberShipComponent.Memberships.Add(CommerceEntity.ListName<Setting>());
                    setting.SetComponent(listMemberShipComponent);
                }

                var transientListMembershipComponent = setting.GetComponent<TransientListMembershipsComponent>();
                if (!transientListMembershipComponent.Memberships.Contains(CommerceEntity.ListName<Setting>()))
                {
                    transientListMembershipComponent.Memberships.Add(CommerceEntity.ListName<Setting>());
                    setting.SetComponent(transientListMembershipComponent);
                }

                var persistResult = await _commerceCommander.PersistEntity(commerceContext, setting);

                return persistResult
                    ? setting
                    : null;
            }
        }
    }
}