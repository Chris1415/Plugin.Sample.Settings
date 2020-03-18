namespace Plugin.Sample.Settings.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Commands;
    using Plugin.Sample.Settings.Models;
    using Plugin.Sample.Settings.Policies;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionAddSetting")]
    public class DoActionAddSetting : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionAddSetting(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals(context.GetPolicy<SettingsUiPolicy>().AddSettingActionName, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                string name = entityView.Properties.FirstOrDefault(element => element.Name.Equals("Name"))?.Value ?? string.Empty; ;
                string displayName = entityView.Properties.FirstOrDefault(element => element.Name.Equals("Display Name"))?.Value ?? string.Empty;

                if (!string.IsNullOrEmpty(name)
                    && !string.IsNullOrEmpty(displayName))
                {
                    await _commerceCommander.Command<CreateSettingCommand>().Process(context.CommerceContext, new CreateSettingArg(name, displayName));
                }
            }

            catch (Exception ex)
            {
                context.Logger.LogError($"{this.Name}.PathNotFound: Message={ex.Message}");
            }

            return entityView;
        }
    }
}