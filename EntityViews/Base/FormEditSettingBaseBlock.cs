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

    [PipelineDisplayName("FormEditPimImporterPolicy")]
    public abstract class FormEditSettingBaseBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FormEditSettingBaseBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public async Task<EntityView> ExecuteRun<T>(EntityView entityView, CommercePipelineExecutionContext context, string actionName) where T : Policy
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != actionName)
            {
                return entityView;
            }

            string targetId = string.IsNullOrEmpty(entityView.EntityId)
               ? entityView.ItemId
               : entityView.EntityId;

            var setting = await _commerceCommander.Command<GetSettingCommand>().Process(context.CommerceContext, targetId);
            if (setting == null)
            {
                return entityView;
            }

            var importerPolicy = setting.GetPolicy<T>();

            try
            {

                foreach (var property in importerPolicy.GetType().GetProperties())
                {
                    var editorSetting = property.GetCustomAttribute<EditorSettingAttribute>();
                    if (editorSetting == null)
                    {
                        continue;
                    }

                    object value = property.GetValue(importerPolicy);
                    string valueToUse = string.Empty;
                    if (value is IList<string>)
                    {
                        valueToUse = string.Join("|", value as IList<string>);
                    }
                    else
                    {
                        valueToUse = value?.ToString() ?? string.Empty;
                    }

                    entityView.Properties.Add(
                      new ViewProperty
                      {
                          Name = editorSetting.DisplayName,
                          IsHidden = false,
                          IsRequired = false,
                          RawValue = valueToUse
                      });
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Content.SynchronizeContentPath.PathNotFound: Message={ex.Message}");
            }

            return await Task.FromResult(entityView);
        }
    }
}