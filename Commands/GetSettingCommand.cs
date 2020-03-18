namespace Plugin.Sample.Settings.Commands
{
    using System;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Entities;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class GetSettingCommand : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public GetSettingCommand(CommerceCommander commerceCommander, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _commerceCommander = commerceCommander;
        }

        public async Task<Setting> Process(CommerceContext commerceContext, string id)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var rawEntitiy = await _commerceCommander.Pipeline<IFindEntityPipeline>().Run(new FindEntityArgument(typeof(Setting), id.ToEntityId<Setting>()), commerceContext.PipelineContextOptions);
                if (rawEntitiy == null)
                {
                    return null;
                }
                else
                {
                    return rawEntitiy as Setting;
                }
            }
        }
    }
}