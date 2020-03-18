using Plugin.Sample.Settings.Commands;
using Sitecore.Commerce.Core;
using System.Threading.Tasks;

namespace Plugin.Sample.Settings.Extensions
{
    public static class ContextExtensions
    {
        public static async Task<T> GetSettingPolicy<T>(this CommercePipelineExecutionContext context, CommerceCommander commander) where T : Policy
        {
            var activeSetting = await commander.Command<GetActiveSettingCommand>().Process(context.CommerceContext);
            return activeSetting?.HasPolicy<T>() ?? false
                ? activeSetting.GetPolicy<T>()
                : context.GetPolicy<T>();
        }
    }
}
