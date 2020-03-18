namespace Plugin.Sample.Settings.EntityViews
{
    using System;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Commands;
    using Plugin.Sample.Settings.Policies;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionDeactivateSetting")]
    public class DoActionDeactivateSetting : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionDeactivateSetting(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals(context.GetPolicy<SettingsUiPolicy>().DeactivateSettingActionName, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            string targetId = string.IsNullOrEmpty(entityView.EntityId)
                ? entityView.ItemId
                : entityView.EntityId;

            try
            {
                var setting = await _commerceCommander.Command<GetSettingCommand>().Process(context.CommerceContext, targetId);
                if (setting == null)
                {
                    return entityView;
                }

                await _commerceCommander.Command<DeactivateSettingCommand>().Process(context.CommerceContext, setting.Id);
            }

            catch (Exception ex)
            {
                context.Logger.LogError($"{this.Name}.PathNotFound: Message={ex.Message}");
            }

            return entityView;
        }
    }
}