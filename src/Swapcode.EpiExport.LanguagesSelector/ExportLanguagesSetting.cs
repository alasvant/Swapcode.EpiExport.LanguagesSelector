using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Logging;
using EPiServer.PlugIn;
using Swapcode.EpiExport.LanguagesSelector.WebControls;

namespace Swapcode.EpiExport.LanguagesSelector
{
    /// <summary>
    /// Episerver UI plugin for persisting export languages.
    /// </summary>
    [GuiPlugIn(Area = PlugInArea.None, DisplayName = "Export languages", Description = "Allows selecting what languages to be exported in Episerver Export.")]
    public class ExportLanguagesSetting
    {
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(ExportLanguagesSetting));

        /// <summary>
        /// Gets the selected language ids.
        /// </summary>
        /// <returns>IEnumerable{string} of selected language ids.</returns>
        public static IEnumerable<string> GetSelectedLanguageIds()
        {
            try
            {
                ExportLanguagesSetting setting = new ExportLanguagesSetting();
                PlugInSettings.AutoPopulate(setting);

                if (string.IsNullOrWhiteSpace(setting.SelectedLanguageIds))
                {
                    return Enumerable.Empty<string>();
                }

                // split the string and just return it as IEnumerable<string>
                return setting.SelectedLanguageIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get the selected language ids for Episerver export.", ex);
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Gets or sets the selected language ids as a comma separated list. Sample: en,fi,sv
        /// </summary>
        [PlugInProperty("Language ids", "Select languages to be used in Episerver export", AdminControl = typeof(LanguagesCheckBoxList), AdminControlValue = "SelectedLanguages")]
        public string SelectedLanguageIds { get; set; }
    }
}
