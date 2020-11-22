using System;
using EPiServer.Enterprise;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;

namespace Swapcode.EpiExport.LanguagesSelector
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ExportLanguagesInitializationModule : IInitializableModule
    {
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(ExportLanguagesInitializationModule));

        private bool _isInitialized;

        public void Initialize(InitializationEngine context)
        {
            try
            {
                if (!_isInitialized)
                {
                    context.Locate.Advanced.GetInstance<IDataExportEvents>().Starting += ExportStarting;
                    _isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Initialization failed.", ex);
            }
        }

        private void ExportStarting(EPiServer.Enterprise.Transfer.ITransferContext transferContext, DataExporterContextEventArgs e)
        {
            try
            {
                var languages = transferContext?.ContentLanguages;

                if (languages != null && languages.Count == 0)
                {
                    var languagesToExport = ExportLanguagesSetting.GetSelectedLanguageIds();

                    if (_logger.IsInformationEnabled())
                    {
                        _logger.Information($"Setting export languages to: '{string.Join(",", languagesToExport)}'. Note, if there are no entries it means that all languages are exported.");
                    }

                    foreach (var lang in languagesToExport)
                    {
                        languages.Add(lang);
                    }
                }
                else
                {
                    if (languages == null)
                    {
                        _logger.Error("Export ITransferContext is null or the ITransferContext.ContentLanguages property is null. Cannot set export languages.");
                    }
                    else
                    {
                        // there are already languages
                        _logger.Warning($"Not setting export languages because the ITransferContext.ContentLanguages already has languages set: '{string.Join(",", languages)}'.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to set export languages in ExportStarting event.", ex);
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            try
            {
                if (_isInitialized)
                {
                    context.Locate.Advanced.GetInstance<IDataExportEvents>().Starting -= ExportStarting;
                    _isInitialized = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Un-initialization failed.", ex);
            }
        }
    }
}
