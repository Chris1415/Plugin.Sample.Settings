namespace Plugin.Sample.Settings.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Plugin.Sample.Settings.Attributes;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class PolicyCollectionCommand : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public PolicyCollectionCommand(CommerceCommander commerceCommander, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _commerceCommander = commerceCommander;
        }

        public IEnumerable<Type> Process(CommerceContext commerceContext)
        {
            var typesWithMyAttribute =
                from assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                from type in assembly.GetTypes()
                let attributes = type.GetCustomAttributes(typeof(SettingsPolicyAttribute), true)
                where attributes != null && attributes.Length > 0
                select type;

            return typesWithMyAttribute.AsEnumerable();
        }
    }
}