namespace Plugin.Sample.Settings.EntityViews
{
    using System;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Policies;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using Plugin.Sample.Settings.Commands;

    [PipelineDisplayName("SettingsDashboard")]
    public class SettingsDashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public SettingsDashboard(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != context.GetPolicy<SettingsUiPolicy>().ViewName)
            {
                return entityView;
            }

            entityView.UiHint = "Flat";
            entityView.Icon = context.GetPolicy<SettingsUiPolicy>().Icon;
            entityView.DisplayName = "Settings";

            try
            {
                await this._commerceCommander.Command< ChildViewSettings>().Process(context.CommerceContext, entityView);
            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex, "Settings.DashBoard.Exception");
            }

            return entityView;
        }
    }
}
