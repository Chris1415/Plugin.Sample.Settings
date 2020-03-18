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

    [PipelineDisplayName("FormAddSetting")]
    public class FormAddSetting : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FormAddSetting(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != context.GetPolicy<SettingsUiPolicy>().AddSettingActionName)
            {
                return entityView;
            }

            try
            {
                entityView.Properties.Add(
                   new ViewProperty
                   {
                       Name = "Name",
                       IsHidden = false,
                       IsRequired = true,
                       RawValue = string.Empty
                   });
                entityView.Properties.Add(
                   new ViewProperty
                   {
                       Name = "Display Name",
                       IsHidden = false,
                       IsRequired = true,
                       RawValue = string.Empty
                   });
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Content.SynchronizeContentPath.PathNotFound: Message={ex.Message}");
            }

            return await Task.FromResult(entityView);
        }
    }
}