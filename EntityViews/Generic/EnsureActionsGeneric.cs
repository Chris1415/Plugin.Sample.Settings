namespace Plugin.Sample.Settings.EntityViews.Generic
{
    using System.Linq;
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Commands;
    using Plugin.Sample.Settings.Extensions;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("EnsureActionsGeneric")]
    public class EnsureActionsGeneric : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        public EnsureActionsGeneric(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");
            var mainActionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            var policies = _commerceCommander.Command<PolicyCollectionCommand>().Process(context.CommerceContext);
            foreach (var policy in policies)
            {
                string viewName = policy.GetViewName();
                string actionName = policy.GetActionName();

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
            }

            return Task.FromResult(entityView);
        }
    }
}
