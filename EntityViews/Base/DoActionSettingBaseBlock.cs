using Microsoft.Extensions.Logging;
using Plugin.Sample.Settings.Attributes;
using Plugin.Sample.Settings.Commands;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Plugin.Sample.Settings.EntityViews.Base
{
    public abstract class DoActionSettingBaseBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionSettingBaseBlock(
           CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public async Task<EntityView> ExecuteRun<T>(EntityView entityView, CommercePipelineExecutionContext context, string actionName) where T : Policy
        {
            if (entityView == null
                || !entityView.Action.Equals(actionName, StringComparison.OrdinalIgnoreCase))
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

                var importerPolicy = setting.GetPolicy<T>();


                foreach (var property in importerPolicy.GetType().GetProperties())
                {
                    var editorSetting = property.GetCustomAttribute<EditorSettingAttribute>();
                    if (editorSetting == null)
                    {
                        continue;
                    }

                    string formValue = entityView.Properties.FirstOrDefault(element => element.Name.Equals(editorSetting.DisplayName))?.Value ?? string.Empty;

                    var propertyValue = property.GetValue(importerPolicy);
                    if (propertyValue is IList<string>)
                    {
                        property.SetValue(importerPolicy, formValue.Split('|'));
                    }
                    else if (propertyValue is int)
                    {
                        if (int.TryParse(formValue, out int mappedFormValue))
                        {
                            property.SetValue(importerPolicy, mappedFormValue);
                        }
                    }
                    else
                    {
                        property.SetValue(importerPolicy, formValue);
                    }
                }

                bool success = await _commerceCommander.PersistEntity(context.CommerceContext, setting);
            }

            catch (Exception ex)
            {
                context.Logger.LogError($"{this.Name}.PathNotFound: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
