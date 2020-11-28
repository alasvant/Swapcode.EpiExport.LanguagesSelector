using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Swapcode.EpiExport.LanguagesSelector.DefaultServices;

namespace Swapcode.EpiExport.LanguagesSelector
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ExportSettingsLoaderInitializationModule : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<IExportSettingsLoader, ExportSettingsLoader>();
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
