namespace Plugin.Sample.Settings.EntityViews.Base
{
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("EnsureActionBase")]
    public abstract class EnsureActionsBase : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public Task<EntityView> BaseRun(EntityView entityView, CommercePipelineExecutionContext context, string viewName, string actionName)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");
            var mainActionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            var view = entityView.ChildViews.FirstOrDefault(p => p.Name == viewName);
            if (view != null)
            {
                var basicDataViewActionsPolicy = view.GetPolicy<ActionsPolicy>();


                basicDataViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = actionName,
                    DisplayName = "Edit",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = actionName,
                    UiHint = string.Empty,
                    Icon = "edit"
                });
            }

            return Task.FromResult(entityView);
        }
    }
}
