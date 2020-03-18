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

    [PipelineDisplayName("DummyFormSetting")]
    public class FormDummySetting : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FormDummySetting(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != context.GetPolicy<SettingsUiPolicy>().DeleteSettingActionName
                && entityView.Name != context.GetPolicy<SettingsUiPolicy>().ActivateSettingActionName
                && entityView.Name != context.GetPolicy<SettingsUiPolicy>().DeactivateSettingActionName)
            {
                return entityView;
            }

            try
            {
                entityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "Hidden",
                        IsHidden = true,
                        IsRequired = true,
                        RawValue = "Dummy"
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