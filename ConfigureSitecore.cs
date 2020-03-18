﻿namespace Plugin.Sample.Settings
{
    using System.Reflection;
    using Plugin.Sample.Settings.EntityViews;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IBizFxNavigationPipeline>(d =>
                {
                    d.Add<EnsureNavigationView>();
                })
               .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
               {
                   d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
               })
               .ConfigurePipeline<IGetEntityViewPipeline>(d =>
               {
                   d.Add<SettingsDashboard>().Before<IFormatEntityViewPipeline>();
                   d.Add<EntityViewSettings>().Before<IFormatEntityViewPipeline>();
                   d.Add<FormAddSetting>().Before<IFormatEntityViewPipeline>();
                   d.Add<FormDummySetting>().Before<IFormatEntityViewPipeline>();
               })
               .ConfigurePipeline<IDoActionPipeline>(c =>
               {
                   c.Add<DoActionActivateSetting>().After<ValidateEntityVersionBlock>();
                   c.Add<DoActionDeactivateSetting>().After<ValidateEntityVersionBlock>();
                   c.Add<DoActionAddSetting>().After<ValidateEntityVersionBlock>();
                   c.Add<DoActionDeleteSetting>().After<ValidateEntityVersionBlock>();
               })
               .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}