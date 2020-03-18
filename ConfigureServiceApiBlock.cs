namespace Plugin.Sample.Settings
{
    using System.Threading.Tasks;
    using Plugin.Sample.Settings.Entities;
    using Microsoft.AspNetCore.OData.Builder;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("SettingsConfigureServiceApiBlock")]
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument cannot be null.");

            // Add the entities
            modelBuilder.AddEntityType(typeof(Setting));

            // Add the entity sets
            modelBuilder.EntitySet<Setting>("Settings");

            return Task.FromResult(modelBuilder);
        }
    }
}
