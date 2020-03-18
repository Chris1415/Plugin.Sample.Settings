namespace Plugin.Sample.Settings.EntityViews
{
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("EnsureNavigationView")]
    public class EnsureNavigationView : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public EnsureNavigationView(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "ToolsNavigation")
            {
                return Task.FromResult(entityView);
            }

            var cartsView = new EntityView
            {
                Name = context.GetPolicy<SettingsUiPolicy>().ViewName,
                DisplayName = "Settings",
                UiHint = "extension",
                Icon = context.GetPolicy<SettingsUiPolicy>().Icon,
                ItemId = context.GetPolicy<SettingsUiPolicy>().ViewName
            };

            entityView.ChildViews.Add(cartsView);

            return Task.FromResult(entityView);
        }
    }
}
