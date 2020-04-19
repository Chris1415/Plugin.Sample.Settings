namespace Plugin.Sample.Settings.EntityViews.Base
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Plugin.Sample.Settings.Attributes;
    using Plugin.Sample.Settings.Commands;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("EntityViewSettingsBaseBlock")]
    public abstract class EntityViewSettingsBaseBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ViewCommander _viewCommander;

        public EntityViewSettingsBaseBlock(ViewCommander viewCommander)
        {
            this._viewCommander = viewCommander;
        }

        public async Task<EntityView> ExecuteRun<T>(EntityView entityView, CommercePipelineExecutionContext context, string viewName) where T : Policy
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null"); ;

            if (string.IsNullOrEmpty(entityView.EntityId))
            {
                return await Task.FromResult(entityView);
            }

            var entity = await _viewCommander.Command<GetSettingCommand>().Process(context.CommerceContext, entityView.EntityId);
            if (entity == null)
            {
                return entityView;
            }


            if (!string.IsNullOrEmpty(entityView.Action))
            {
                return entityView;
            }

            try
            {
                var importerSetting = entity.GetPolicy<T>();

                var importerSettingView = new EntityView
                {
                    EntityId = string.Empty,
                    ItemId = entity.Id,
                    DisplayName = viewName,
                    Name = viewName
                };

                foreach (var property in importerSetting.GetType().GetProperties())
                {
                    var editorSetting = property.GetCustomAttribute<EditorSettingAttribute>();
                    if (editorSetting == null)
                    {
                        continue;
                    }

                    object value = property.GetValue(importerSetting);
                    string valueToUse = string.Empty;
                    if (value is IList<string>)
                    {
                        valueToUse = string.Join("|", value as IList<string>);
                    }
                    else
                    {
                        valueToUse = value?.ToString() ?? string.Empty;
                    }

                    importerSettingView.Properties
                        .Add(new ViewProperty { Name = editorSetting.DisplayName, RawValue = valueToUse });
                }

                entityView.ChildViews.Add(importerSettingView);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Content.SynchronizeContentPath.PathNotFound: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
