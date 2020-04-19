namespace Plugin.Sample.Settings.EntityViews.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Plugin.Sample.Settings.Attributes;
    using Plugin.Sample.Settings.Commands;
    using Plugin.Sample.Settings.Extensions;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("FormEditSettingGenericBlock")]
    public class FormEditSettingGenericBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FormEditSettingGenericBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public async override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var policies = _commerceCommander.Command<PolicyCollectionCommand>().Process(context.CommerceContext);
            foreach (var policy in policies)
            {
                string actionName = policy.GetActionName();

                if (entityView.Name != actionName)
                {
                    continue;
                }

                string targetId = string.IsNullOrEmpty(entityView.EntityId)
                   ? entityView.ItemId
                   : entityView.EntityId;

                var setting = await _commerceCommander.Command<GetSettingCommand>().Process(context.CommerceContext, targetId);
                if (setting == null)
                {
                    return entityView;
                }

                var policyFromSetting = setting.EntityPolicies.FirstOrDefault(element => element.GetType().FullName.Equals(policy.FullName));
                if (policyFromSetting == null)
                {
                    policyFromSetting = Activator.CreateInstance(policy) as Policy;
                }

                try
                {
                    foreach (var property in policyFromSetting.GetType().GetProperties())
                    {
                        var editorSetting = property.GetCustomAttribute<EditorSettingAttribute>();
                        if (editorSetting == null)
                        {
                            continue;
                        }

                        object value = property.GetValue(policyFromSetting);
                        string valueToUse = string.Empty;
                        if (value is IList<string>)
                        {
                            valueToUse = string.Join("|", value as IList<string>);
                        }
                        else if (value is int)
                        {
                            try
                            {
                                int intValue = Convert.ToInt32(value);
                                entityView.Properties.Add(
                                 new ViewProperty
                                 {
                                     Name = editorSetting.DisplayName,
                                     IsHidden = false,
                                     IsRequired = false,
                                     RawValue = intValue
                                 });
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (value is bool)
                        {
                            try
                            {
                                bool boolValue = Convert.ToBoolean(value);
                                entityView.Properties.Add(
                                new ViewProperty
                                {
                                    Name = editorSetting.DisplayName,
                                    IsHidden = false,
                                    IsRequired = false,
                                    RawValue = boolValue
                                });
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            entityView.Properties.Add(
                            new ViewProperty
                            {
                                Name = editorSetting.DisplayName,
                                IsHidden = false,
                                IsRequired = false,
                                RawValue = value?.ToString() ?? string.Empty
                            });
                        }
                    }

                    return await Task.FromResult(entityView);
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"Content.SynchronizeContentPath.PathNotFound: Message={ex.Message}");
                }
            }

            return await Task.FromResult(entityView);
        }
    }
}