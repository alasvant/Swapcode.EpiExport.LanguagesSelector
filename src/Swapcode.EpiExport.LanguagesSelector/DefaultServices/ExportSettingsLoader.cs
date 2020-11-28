using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.PlugIn;

namespace Swapcode.EpiExport.LanguagesSelector.DefaultServices
{
    /// <summary>
    /// Service to load <see cref="ExportSettings"/>.
    /// </summary>
    public class ExportSettingsLoader : IExportSettingsLoader
    {
        /// <inheritdoc/>
        public virtual ExportSettings GetExportSettings()
        {
            // load the epi plugin settings class and create the exposed settings class using the values
            ExportLanguagesSetting setting = new ExportLanguagesSetting();
            PlugInSettings.AutoPopulate(setting);

            IEnumerable<string> languages = null;

            if (!string.IsNullOrWhiteSpace(setting.SelectedLanguageIds))
            {
                // split the string and just return it as IEnumerable<string>
                languages = setting.SelectedLanguageIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable();
            }

            return new ExportSettings(languages, setting.ExcludeContentWithShortcut);
        }
    }
}
