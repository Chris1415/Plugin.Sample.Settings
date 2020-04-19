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

    [PipelineDisplayName("EntityViewSettingsGenericBlock")]
    public class EntityViewSettingsGenericBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ViewCommander _viewCommander;

        public EntityViewSettingsGenericBlock(ViewCommander viewCommander)
        {
            this._viewCommander = viewCommander;
        }

        public async override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
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

            var policies = _viewCommander.Command<PolicyCollectionCommand>().Process(context.CommerceContext);
            foreach (var policy in policies)
            {
                string viewName = policy.GetViewName();

                try
                {
                    var policyFromSetting = entity.EntityPolicies.FirstOrDefault(element => element.GetType().FullName.Equals(policy.FullName));
                    if (policyFromSetting == null)
                    {
                        policyFromSetting = Activator.CreateInstance(policy) as Policy;
                    }

                    var importerSettingView = new EntityView
                    {
                        EntityId = string.Empty,
                        ItemId = entity.Id,
                        DisplayName = viewName,
                        Name = viewName
                    };

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
            }

            return entityView;
        }
    }
}
