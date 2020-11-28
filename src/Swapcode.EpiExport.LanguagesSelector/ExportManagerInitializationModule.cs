using System;
using System.Linq;
using EPiServer.DataAbstraction;
using EPiServer.Enterprise;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;

namespace Swapcode.EpiExport.LanguagesSelector
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ExportManagerInitializationModule : IInitializableModule
    {
        // Note, the episerver export admin UI allows to start multiple exports BUT at least if same content root is used
        // the export will blow up with various exceptions, so based on that, we should assume that multiple simultaneous
        // exports are not actually supported by Episerver but there are no checks to prevent that
        // so based on that it is ok to have the ExportSettings intance field set in export Starting and use it until a new export starts

        // It woud have been nice to have a separate "manager" class to handle the actual modifications to the export
        // but then we would have a problem how to unregister the export events per manager instance as there is
        // no event for export complete/ended, so this initialization module will work as the manager as it can un-register
        // the handlers in the Uninitialize method so we don't leak the handlers and/or cause created manager instances to stay
        // in memory and block carbage collector not being able to collect the objects as there is dependecy from export events
        // to the object(s)

        /// <summary>
        /// Logger reference.
        /// </summary>
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(ExportManagerInitializationModule));

        /// <summary>
        /// Reference to exxport settings loader service.
        /// </summary>
        private IExportSettingsLoader _exportSettingsLoader;

        /// <summary>
        /// Reference to current export settings.
        /// </summary>
        private ExportSettings _exportSettings;

        /// <summary>
        /// Is this module initialized.
        /// </summary>
        private bool _isInitialized;

        public void Initialize(InitializationEngine context)
        {
            try
            {
                if (!_isInitialized)
                {
                    var locator = context.Locate.Advanced;

                    _exportSettingsLoader = locator.GetInstance<IExportSettingsLoader>();

                    var exportEvents = locator.GetInstance<IDataExportEvents>();

                    exportEvents.Starting += ExportStarting;
                    exportEvents.ContentExporting += ContentExporting;

                    _isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Initialization failed.", ex);
            }
        }

        private void ContentExporting(EPiServer.Enterprise.Transfer.ITransferContext transferContext, ContentExportingEventArgs e)
        {
            try
            {
                // get the settings to local scope first, so that if something changes the instance member we have our own copy here already
                var exportSettings = _exportSettings;

                // if we don't have settings, log error (don't try to reload them)
                if (exportSettings == null)
                {
                    // log this, so that we can try to track down this issue if it should happen
                    _logger.Error("ExportSettings instance is null. Using Episerver default export logic for content.");
                    return;
                }

                // content should not be filtered based on shortcut type
                if (!exportSettings.ExcludeIfShortcutIsSet)
                {
                    return;
                }

                var rawMasterData = e?.TransferContentData?.RawContentData;

                if (rawMasterData != null && rawMasterData.Property != null && rawMasterData.Property.Length > 0)
                {
                    // get PageShortcutType property, basically only pages have this property
                    var shortCut = rawMasterData.Property.FirstOrDefault(x => string.Equals(MetaDataProperties.PageShortcutType, x.Name, StringComparison.OrdinalIgnoreCase));

                    // Normal page has value 0 as the PageShortcutType (meaning Normal, see EPiServer.Core.PageShortcutType)
                    // so if we have the value and it is not 0 then discard the content from export
                    if (shortCut != null && !"0".Equals(shortCut.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        // the PageShortcutType is language specific, so in theory we should look at the language versions too, to exlude those
                        // but for now, just do it like this, if master is shortcut then filter out all

                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    _logger.Error("ContentExporting raw data is null or empty. Cannot check if content should be filtered or not.");
                }
            }
            catch (Exception ex)
            {
                // we must handle exception in the handler so that we will not cause issues to the whole export
                // if something fails then let Episerver use default export for the content
                _logger.Error("There was an exception when trying to filter out content from export. Episerver default export logic is now used for the content.", ex);
            }
        }

        private void ExportStarting(EPiServer.Enterprise.Transfer.ITransferContext transferContext, DataExporterContextEventArgs e)
        {
            try
            {
                // get content languages list from context
                var languages = transferContext?.ContentLanguages;

                // if there are already languages set, we don't do anything
                if (languages != null && languages.Count == 0)
                {
                    // get the export settings first to local variable and then cache to shared field in this instance
                    // this should always return settings or throw
                    var exportSettings = _exportSettingsLoader.GetExportSettings();
                    _exportSettings = exportSettings;

                    if (_logger.IsInformationEnabled())
                    {
                        _logger.Information($"Setting export languages to '{string.Join(",", exportSettings.Languages)}' and exclude content with shortcut type set '{exportSettings.ExcludeIfShortcutIsSet}'. Note, if there are no entries it means that all languages are exported.");
                    }

                    // add export settings languages to the context languages list
                    foreach (var lang in exportSettings.Languages)
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
                    var locator = context.Locate.Advanced.GetInstance<IDataExportEvents>();

                    locator.Starting -= ExportStarting;
                    locator.ContentExporting -= ContentExporting;

                    _isInitialized = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Un-initialization failed.", ex);
                // if there is exception in uninitialize then set it also to uninitialized state
                // as the framework will not try to re-uninitialize the module
                _isInitialized = false;
            }
        }
    }
}
