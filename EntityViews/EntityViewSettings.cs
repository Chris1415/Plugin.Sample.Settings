namespace Plugin.Sample.Settings.EntityViews
{
    using System;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Commands;
    using Plugin.Sample.Settings.Entities;
    using Plugin.Sample.Settings.Policies;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("EntityViewSettings")]
    public class EntityViewSettings : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ViewCommander _viewCommander;

        public EntityViewSettings(ViewCommander viewCommander)
        {
            this._viewCommander = viewCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null"); ;

            if (string.IsNullOrEmpty(entityView.EntityId))
            {
                return await Task.FromResult(entityView);
            }

            var entity = await _viewCommander.Command<GetSettingCommand>().Process(context.CommerceContext, entityView.EntityId);
            if(entity == null)
            {
                return entityView;
            }

            if (!string.IsNullOrEmpty(entityView.Action))
            {
                return entityView;
            }

            var entityViewArgument = _viewCommander.CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;

            var name = entityView.EntityId;

            try
            {
                entityViewArgument.Entity = entity;
                this.AddBasicData(entityView, context.CommerceContext, entity);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Content.SynchronizeContentPath.PathNotFound: Message={ex.Message}");
            }

            return entityView;
        }

        private void AddBasicData(EntityView entityView, CommerceContext commerceContext, Setting entity)
        {
            if (entity == null)
            {
                return;
            }

            var settingsuiPolicy = commerceContext.GetPolicy<SettingsUiPolicy>();

            var promotionEntityView = new EntityView
            {
                EntityId = string.Empty,
                ItemId = entity.Id,
                DisplayName = settingsuiPolicy.BasicDataSettingsViewName,
                Name = settingsuiPolicy.BasicDataSettingsViewName
            };

            promotionEntityView.Properties
                .Add(new ViewProperty { Name = "Is Active", RawValue = entity.IsActive });

            entityView.ChildViews.Add(promotionEntityView);
        }
    }
}
