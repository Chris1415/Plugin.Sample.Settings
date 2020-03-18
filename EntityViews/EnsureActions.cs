namespace Plugin.Sample.Settings.EntityViews                                           
{
    using System.Linq;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");
            var settingsuiPolicy = context.GetPolicy<SettingsUiPolicy>();
            var mainActionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            var basicData = entityView.ChildViews.FirstOrDefault(p => p.Name == settingsuiPolicy.BasicDataSettingsViewName);
            if (basicData != null)
            {
                var basicDataViewActionsPolicy = basicData.GetPolicy<ActionsPolicy>();


                basicDataViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = settingsuiPolicy.ActivateSettingActionName,
                    DisplayName = "Activate",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = settingsuiPolicy.ActivateSettingActionName,
                    UiHint = string.Empty,
                    Icon = "navigate_plus"
                });

                basicDataViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = settingsuiPolicy.DeactivateSettingActionName,
                    DisplayName = "Deactivate",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = settingsuiPolicy.DeactivateSettingActionName,
                    UiHint = string.Empty,
                    Icon = "navigate_minus"
                });
            }

            var settingsList = entityView.ChildViews.FirstOrDefault(p => p.Name == settingsuiPolicy.ViewName);
            if (settingsList != null)
            {
                var basicDataViewActionsPolicy = settingsList.GetPolicy<ActionsPolicy>();

                basicDataViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = settingsuiPolicy.AddSettingActionName,
                    DisplayName = "Add",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = settingsuiPolicy.AddSettingActionName,
                    UiHint = string.Empty,
                    Icon = "add"
                });

                basicDataViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = settingsuiPolicy.DeleteSettingActionName,
                    DisplayName = "Remove",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = settingsuiPolicy.DeleteSettingActionName,
                    UiHint = string.Empty,
                    Icon = "delete",
                });

                basicDataViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = settingsuiPolicy.ActivateSettingActionName,
                    DisplayName = "Activate",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = settingsuiPolicy.ActivateSettingActionName,
                    UiHint = string.Empty,
                    Icon = "navigate_plus"
                });

                basicDataViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = settingsuiPolicy.DeactivateSettingActionName,
                    DisplayName = "Deactivate",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = settingsuiPolicy.DeactivateSettingActionName,
                    UiHint = string.Empty,
                    Icon = "navigate_minus"
                });
            }

            return Task.FromResult(entityView);
        }
    }
}
