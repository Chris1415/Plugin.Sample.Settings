using Microsoft.Extensions.Logging;
using Plugin.Sample.Settings.Attributes;
using Plugin.Sample.Settings.Commands;
using Plugin.Sample.Settings.Extensions;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Plugin.Sample.Settings.EntityViews.Generic
{
    public class DoActionSettingGenericBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionSettingGenericBlock(
           CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public async override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            string targetId = string.IsNullOrEmpty(entityView.EntityId)
             ? entityView.ItemId
             : entityView.EntityId;

            var setting = await _commerceCommander.Command<GetSettingCommand>().Process(context.CommerceContext, targetId);
            if (setting == null)
            {
                return entityView;
            }

            var policies = _commerceCommander.Command<PolicyCollectionCommand>().Process(context.CommerceContext);
            foreach (var policy in policies)
            {
                string actionName = policy.GetActionName();
                if (entityView == null || !entityView.Action.Equals(actionName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var policyFromSetting = setting.EntityPolicies.FirstOrDefault(element => element.GetType().FullName.Equals(policy.FullName));
                    if (policyFromSetting == null)
                    {
                        policyFromSetting = Activator.CreateInstance(policy) as Policy;
                    }

                    foreach (var property in policyFromSetting.GetType().GetProperties())
                    {
                        var editorSetting = property.GetCustomAttribute<EditorSettingAttribute>();
                        if (editorSetting == null)
                        {
                            continue;
                        }

                        string formValue = entityView.Properties.FirstOrDefault(element => element.Name.Equals(editorSetting.DisplayName))?.Value ?? string.Empty;

                        var propertyValue = property.GetValue(policyFromSetting);
                        if (propertyValue is IList<string>)
                        {
                            property.SetValue(policyFromSetting, formValue.Split('|'));
                        }
                        else if (propertyValue is int)
                        {
                            if (int.TryParse(formValue, out int mappedFormValue))
                            {
                                property.SetValue(policyFromSetting, mappedFormValue);
                            }
                        }
                        else if (propertyValue is bool)
                        {
                            if (bool.TryParse(formValue, out bool mappedFormValue))
                            {
                                property.SetValue(policyFromSetting, mappedFormValue);
                            }
                        }
                        else
                        {
                            property.SetValue(policyFromSetting, formValue);
                        }
                    }

                    setting.SetPolicy(policyFromSetting);
                    bool success = await _commerceCommander.PersistEntity(context.CommerceContext, setting);
                    return entityView;
                }

                catch (Exception ex)
                {
                    context.Logger.LogError($"{this.Name}.PathNotFound: Message={ex.Message}");
                }
            }

            return entityView;
        }
    }
}
